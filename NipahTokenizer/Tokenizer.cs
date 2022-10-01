using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using S = NipahTokenizer.Separator;

#nullable enable

namespace NipahTokenizer
{
    public class Tokenizer
    {
        public static bool IsValue(TokenType token)
        {
            switch (token)
            {
                case TokenType.IntegerLiteral: return true;
                case TokenType.FloatLiteral: return true;
                case TokenType.TrueLiteral: return true;
                case TokenType.FalseLiteral: return true;
                case TokenType.StringLiteral: return true;
                case TokenType.ID: return true;
            }
            return false;
        }
        public static bool IsValueGross(TokenType token)
        {
            switch (token)
            {
                case TokenType.IntegerLiteral: return true;
                case TokenType.FloatLiteral: return true;
                case TokenType.TrueLiteral: return true;
                case TokenType.FalseLiteral: return true;
                case TokenType.StringLiteral: return true;
                case TokenType.NullLiteral: return true;
            }
            return false;
        }
        public static bool IsComparisson(TokenType token)
        {
            switch (token)
            {
                case TokenType.Equal: return true;
                case TokenType.Different: return true;
                case TokenType.Larger: return true;
                case TokenType.Lower: return true;
                case TokenType.LargerOrEqual: return true;
                case TokenType.LowerOrEqual: return true;
            }
            return false;
        }
        public static bool IsConditional(TokenType token)
        {
            switch (token)
            {
                case TokenType.And: return true;
                case TokenType.Or: return true;
            }
            return false;
        }
        public static bool IsOperator(TokenType token)
        {
            switch (token)
            {
                case TokenType.Plus: return true;
                case TokenType.Minus: return true;
                case TokenType.Divide: return true;
                case TokenType.Multiply: return true;
                case TokenType.OpenParenthesis: return true;
                case TokenType.CloseParenthesis: return true;
            }
            return false;
        }
        public static bool IsMathOperator(TokenType token)
        {
            switch (token)
            {
                case TokenType.Plus: return true;
                case TokenType.Minus: return true;
                case TokenType.Divide: return true;
                case TokenType.Multiply: return true;
            }
            return false;
        }
        public static List<char> begins = new List<char>()
        {
			//'"', '<', '[', '(', '{', '\''
			'"', '\'', '§'
        };
        public static List<char> ends = new List<char>()
        {
			//'"', '>', ']', ')', '}', '\''
			'"', '\'', '§'
        };
        public static List<char> special = new List<char>()
        {
            //'<','>','(',')','[',']','{','}'
        };
        public static List<bool> ables = new List<bool>()
        {
            true, true, true, true, true, true
        };
        public static List<bool> intern = new List<bool>()
        {
            true, true, true, true, true, true
        };
        public static Tokenizer Single => _single;
        static readonly Tokenizer _single = new Tokenizer();
        public event Action<List<Token>>? TokensProcessor;
        public event Action<List<SplitItem>>? SplitProcessor;
        public event Action<Token>? TokenProcessor;
        public List<Token> Tokenize(string entry, TokenizerOptions options)
        {
            List<Token> TokenizeNormal()
            {
                entry = entry.Replace("\r", "");
                var sbpool = StringBuilderPool.Pool;
                var pieces = SplitString(entry, options, sbpool);
                var tokens = new List<Token>(pieces.Count);
                SplitProcessor?.Invoke(pieces);
                var spieces = CollectionsMarshal.AsSpan(pieces);
                foreach (ref var piece in spieces)
                {
                    Token token;
                    if (options.LetRawTokens)
                        token = Token.BuildRaw(in piece);
                    else
                    {
                        token = Token.Build(in piece);
                        TokenProcessor?.Invoke(token);
                        // Further
                        string? str = token.Value.TrySolve<string>().Solve();
                        if (str is not null)
                        {
                            str = str.Replace("''", "\"");
                            str = str.Replace('£', '\'');
                            token = token with { Value = str };
                        }
                    }
                    tokens.Add(token);
                }
                if (options.LetRawTokens is false)
                    TokensProcessor?.Invoke(tokens);

                return tokens;
            }

            List<Token> TokenizeParallel()
            {
                entry = entry.Replace("\r", "");

                // Splits the text at chunks in parallel
                int cores = Environment.ProcessorCount;
                string[] entries = SplitChunks(entry, cores);
                var piecesChunks = new Dictionary<int, List<SplitItem>>(cores);
                Parallel.ForEach(entries, (entry, state, index) =>
                {
                    var sbpool = new LocalStringBuilderPool();
                    var pieces = SplitString(entry, options, sbpool);
                    SplitProcessor?.Invoke(pieces);
                    lock (piecesChunks) { piecesChunks.Add((int)index, pieces); }
                });

                // Adjust the lines of the chunk pieces
                static int findLargestLine(List<SplitItem> pieces) => pieces.Count > 0 ? pieces[^1].line : 0;

                int lineMatcher = 0;
                for (int i = 0; i < piecesChunks.Count; i++)
                {
                    List<SplitItem>? pieces = piecesChunks[i];
                    if (i > 0)
                    {
                        int largest = findLargestLine(piecesChunks[i - 1]);
                        lineMatcher += largest;
                        var spanPieces = CollectionsMarshal.AsSpan(pieces);
                        foreach (ref var piece in spanPieces)
                            piece = new SplitItem(piece.text, piece.position, piece.line + lineMatcher);
                    }
                }

                // Processes the split items as token outputs, then aggregates them and returns the result
                var outputs = new Dictionary<int, List<Token>>(32);
                Parallel.ForEach(piecesChunks, pieces =>
                {
                    var tokens = new List<Token>(32);
                    var spieces = CollectionsMarshal.AsSpan(pieces.Value);
                    foreach (ref var piece in spieces)
                    {
                        Token token;
                        if (options.LetRawTokens)
                            token = Token.BuildRaw(in piece);
                        else
                        {
                            token = Token.Build(in piece);
                            TokenProcessor?.Invoke(token);
                            // Further
                            string? str = token.Value.TrySolve<string>().Solve();
                            if (str is not null)
                            {
                                str = str.Replace("''", "\"");
                                str = str.Replace('£', '\'');
                                token = token with { Value = str };
                            }
                        }
                        tokens.Add(token);
                    }
                    if (options.LetRawTokens)
                        TokensProcessor?.Invoke(tokens);
                    lock (outputs) { outputs.Add(pieces.Key, tokens); }
                });

                var tokens = new List<Token>(320);
                for (int i = 0; i < outputs.Count; i++)
                    tokens.AddRange(outputs[i]);

                return tokens;
            }
            return options.Parallel 
                ? TokenizeParallel() 
                : TokenizeNormal();
        }

        static string[] SplitChunks(string text, int chunks)
        {
        recalc:
            int size = text.Length;
            int chunkSize = size / chunks;
            if (chunkSize * chunks != size)
            {
                chunks -= 1;
                goto recalc;
            }

            string[] realChunks = new string[chunks];

            for (int i = 0; i < chunks; i++)
            {
                int fromIndex = i * chunkSize;
                int toIndex = (i + 1) * chunkSize;
                realChunks[i] = text[fromIndex..toIndex];
            }

            return realChunks;
        }

        public static void GeneralizeValue(List<Token> tokens)
        {
            var sptokens = CollectionsMarshal.AsSpan(tokens);
            foreach (ref var token in sptokens)
            {
                if (IsValue(token.Type))
                    token = token with { Type = TokenType.Value };
            }
        }
        public static void GeneralizeValueGross(List<Token> tokens)
        {
            var sptokens = CollectionsMarshal.AsSpan(tokens);
            foreach (ref var token in sptokens)
            {
                if (IsValueGross(token.Type))
                    token = token with { Type = TokenType.Value };
            }
        }

        static void ProcessPositionAndEOF(char c, ref int position, ref int line, EndOfLine[] eofs)
        {
            // Controls position and line
            position++;
            if (Array.Exists(eofs, x => x.EOF == c))
            {
                position = 0;
                line++;
            }
        }

        static void SplitStringEscapedMode(string text, ref int index, ref int position, ref int line, TokenizerOptions options, StringBuilder current)
        {
            char c = text[index];
            if (c is '\\')
            {
                index++;
                char n = text[index];
                string cur;
                if (n is 'n') cur = "\n";
                else if (n is 't') cur = "\t";
                else if (n is 'r') cur = "\r";
                else cur = n.ToString();
                current.Append(cur);
                ProcessPositionAndEOF(c, ref position, ref line, options.EOFs);
            }
            else
                throw new Exception("Out of scaping context");
        }
        static void SplitStringScopedMode(string text, ref int index, ref int position, ref int line, TokenizerOptions options, List<SplitItem> list, Scope currentScope, LocalStringBuilderPool sbpool)
        {
            var eofs = options.EOFs;

            int count = text.Length;
            var current = sbpool.Get(320);

            int selfIndex = index;
            for (index = selfIndex; index < count; index++)
            {
                var c = text[index];

                current.Append(c);
                if (current.Length is 1)
                    continue;

                // Check for end of scope
                if (currentScope.End == c)
                {
                    var item = new SplitItem(current.ToString(), position, line);
                    list.Add(item);
                    ProcessPositionAndEOF(c, ref position, ref line, eofs);
                    sbpool.Return(current);
                    return;
                }
                // Check for escaping
                if (c is '\\')
                {
                    current.Remove(current.Length - 1, 1);
                    SplitStringEscapedMode(text, ref index, ref position, ref line, options, current);
                }

                ProcessPositionAndEOF(c, ref position, ref line, eofs);
            }
            if (current.Length > 0)
                list.Add(new(current.ToString(), position, line));
            sbpool.Return(current);
        }
        static void SplitStringNormalMode(string text, ref int index, ref int position, ref int line, TokenizerOptions options, List<SplitItem> list, LocalStringBuilderPool sbpool)
        {
            var separators = options.Separators;
            var scopeDefs = options.Scopes;
            var eofs = options.EOFs;

            int count = text.Length;

            var current = sbpool.Get(320);

            int selfIndex = index;
            for (index = selfIndex; index < count; index++)
            {
                var c = text[index];

                current.Append(c);

                // Check for separators
                foreach (var sep in separators)
                {
                    bool isMatch = sep.Match == c;
                    if (isMatch)
                    {
                        switch (sep.Include)
                        {
                            case IncludeMode.Aggregate:
                            {
                                var item = new SplitItem(current.ToString(), position, line);
                                list.Add(item);
                                current.Clear();
                                break;
                            }
                            case IncludeMode.Separate:
                            {
                                var item = new SplitItem(current.ToString()[..^1], (position - 1), line);
                                var sepItem = new SplitItem(sep.MatchAsString, position, line);
                                list.Add(item);
                                list.Add(sepItem);
                                current.Clear();
                                break;
                            }
                            case IncludeMode.None:
                            {
                                var item = new SplitItem(current.ToString()[..^1], (position - 1), line);
                                list.Add(item);
                                current.Clear();
                                break;
                            }
                        }
                        break;
                    }
                }
                // Check for scopes
                foreach (var scoper in scopeDefs)
                {
                    if (scoper.Begin == c)
                    {
                        current.Clear();
                        SplitStringScopedMode(text, ref index, ref position, ref line, options, list, scoper, sbpool);
                        break;
                    }
                }
                // Check for escaping
                if (c is '\\')
                {
                    current.Remove(current.Length - 1, 1);
                    SplitStringEscapedMode(text, ref index, ref position, ref line, options, current);
                }

                ProcessPositionAndEOF(c, ref position, ref line, eofs);
            }
            if (current.Length > 0)
                list.Add(new(current.ToString(), position, line));
            sbpool.Return(current);
        }

        public static List<SplitItem> SplitString(string text, TokenizerOptions options, LocalStringBuilderPool sbpool)
        {
            var list = new List<SplitItem>(32);

            int index = 0;
            int position = 0;
            int line = 0;

            SplitStringNormalMode(text, ref index, ref position, ref line, options, list, sbpool);

            list.RemoveAll(x => x.text is "");

            // Will apply aggregators until no more changes can occur
            bool anyChanged;
            while (((list, anyChanged) = ApplyAggregators(CollectionsMarshal.AsSpan(list), options.Aggregators, sbpool)).anyChanged)
                continue;

            return list;
        }
        static (List<SplitItem> outputs, bool changedAny) ApplyAggregators(ReadOnlySpan<SplitItem> inputs, ReadOnlySpan<SplitAggregator> aggregators, LocalStringBuilderPool sbpool)
        {
            var changedAny = false;
            var outputs = new List<SplitItem>(inputs.Length);
            var carry = sbpool.Get(32);
            while (inputs.Length > 0)
            {
                bool isMatch = false;
                foreach (var aggregator in aggregators)
                {
                    carry.Clear();
                    if (ApplyAggregator(inputs, aggregator.Detectors, carry, outputs))
                    {
                        inputs = inputs[aggregator.Detectors.Length..];
                        isMatch = true;
                        changedAny = true;
                    }
                }
                if (isMatch is false)
                {
                    outputs.Add(inputs[0]);
                    inputs = inputs[1..];
                }
            }
            sbpool.Return(carry);
            return (outputs, changedAny);
        }

        static bool ApplyAggregator(ReadOnlySpan<SplitItem> inputs, ReadOnlySpan<Predicate<string>> aggregator, StringBuilder carry, List<SplitItem> outputs)
            => ((aggregator.Length is 0) || (aggregator.Length is 0 && inputs.Length is 0) || aggregator[0](inputs[0]))
            && (inputs.Length >= aggregator.Length)
            && aggregator switch
            {
                { Length: > 0 } => ApplyAggregator(inputs[1..], aggregator[1..], carry.Append(inputs[0].text), outputs),
                _ => Add(outputs, new(carry.Length is 0 ? inputs[0].text : carry.ToString(),
                    inputs[0].position, inputs[0].line))
            };
        static bool Add<T>(List<T> list, T item)
        {
            list.Add(item);
            return true;
        }
    }
    public readonly struct SplitItem
    {
        public readonly string text;
        public readonly int position;
        public readonly int line;

        public static implicit operator string(SplitItem item) => item.text;

        public static SplitItem operator +(SplitItem a, SplitItem b)
        {
            var c = new SplitItem(a.text + b.text, a.position > b.position ? a.position : b.position,
                                  a.line > b.line ? a.line : b.line);
            return c;
        }

        public SplitItem(string text, int position, int line)
        {
            this.text = text;
            this.position = position;
            this.line = line;
        }

        public override string ToString()
        {
            return $"\"{text}\", at (Position: {position}, Line: {line})";
        }
    }
    public struct FinalSplit
    {
        public string result;
    }
    public delegate bool SplitProcessor(SplitItem token, SplitItem next,
                                        out FinalSplit result);
}
