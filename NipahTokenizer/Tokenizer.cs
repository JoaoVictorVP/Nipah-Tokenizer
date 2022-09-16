using System;
using System.Collections.Generic;
using System.Text;
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
		public static event SplitProcessor? FinalSplitProcessor;
		public List<Token> Tokenize(string entry, TokenizerOptions options, bool removeLineBreaks = true)
		{
			//entry = entry.Replace("\n","");
			entry = entry.Replace("\r", "");
			var tokens = new List<Token>();
			var pieces = SplitString(entry, options);
			SplitProcessor?.Invoke(pieces);
			foreach (var piece in pieces)
			{
				var token = Token.Build(piece);
				TokenProcessor?.Invoke(token);
				tokens.Add(token);
			}
			tokens.ForEach(token =>
			{
				string? str = token.value.TrySolve<string>().Solve();
				if (str != null)
				{
					str = str.Replace("''", "\"");
					str = str.Replace('£', '\'');
					token.value = str;
				}
			});
			if (removeLineBreaks)
				tokens.RemoveAll(token => token.type == TokenType.LineBreak);
			TokensProcessor?.Invoke(tokens);
			return tokens;
		}
		public static void GeneralizeValue(List<Token> tokens)
		{
			tokens.ForEach(token =>
			{
				if (IsValue(token.type))
					token.type = TokenType.Value;
			});
		}
		public static void GeneralizeValueGross(List<Token> tokens)
		{
			tokens.ForEach(token =>
			{
				if (IsValueGross(token.type))
					token.type = TokenType.Value;
			});
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

        static void SplitStringEscapedMode(string text, ref int index, ref int position, ref int line, TokenizerOptions options, List<SplitItem> list, Stack<long> scopes, StringBuilder current)
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
        static void SplitStringScopedMode(string text, ref int index, ref int position, ref int line, TokenizerOptions options, List<SplitItem> list, Stack<long> scopes)
		{
            var scopeDefs = options.Scopes;
            var eofs = options.EOFs;

            int count = text.Length;
			var current = new StringBuilder(320);

			int selfIndex = index;
			for(index = selfIndex; index < count; index++)
			{
				var c = text[index];

				current.Append(c);
				if (current.Length is 1)
					continue;

				// Check for end of scope
				foreach (var scoper in scopeDefs)
				{
					if(scoper.End == c)
					{
						var item = new SplitItem(current.ToString(), position, line);
						list.Add(item);
                        ProcessPositionAndEOF(c, ref position, ref line, eofs);
						return;
                    }
				}
				// Check for escaping
				if (c is '\\')
				{
					current.Remove(current.Length - 1, 1);
					SplitStringEscapedMode(text, ref index, ref position, ref line, options, list, scopes, current);
				}

                ProcessPositionAndEOF(c, ref position, ref line, eofs);
            }
			if(current.Length > 0)
				list.Add(new(current.ToString(), position, line));
		}
		static void SplitStringNormalMode(string text, ref int index, ref int position, ref int line, TokenizerOptions options, List<SplitItem> list, Stack<long> scopes)
		{
            var separators = options.Separators;
            var scopeDefs = options.Scopes;
            var eofs = options.EOFs;

            int count = text.Length;

            var current = new StringBuilder(320);

			int selfIndex = index;
            for (index = selfIndex; index < count; index++)
            {
                var c = text[index];

                current.Append(c);

                // Check for separators
                foreach (var sep in separators)
                {
                    var match = sep.Match.Match(current.ToString());
                    if (match.Success)
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
                                var item = new SplitItem(current.ToString()[..match.Index], (position - match.Length), line);
                                var sepItem = new SplitItem(match.Value, position, line);
                                list.Add(item);
                                list.Add(sepItem);
								current.Clear();
                                break;
                            }
                            case IncludeMode.None:
                            {
                                var item = new SplitItem(current.ToString()[..match.Index], (position - match.Length), line);
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
					if(scoper.Begin == c)
					{
						current.Clear();
						SplitStringScopedMode(text, ref index, ref position, ref line, options, list, scopes);
						break;
					}
                }
                // Check for escaping
                if (c is '\\')
                {
                    current.Remove(current.Length - 1, 1);
                    SplitStringEscapedMode(text, ref index, ref position, ref line, options, list, scopes, current);
                }

                ProcessPositionAndEOF(c, ref position, ref line, eofs);
            }
            if (current.Length > 0)
                list.Add(new(current.ToString(), position, line));
        }

		public static List<SplitItem> SplitString(string text, TokenizerOptions options)
		{
			var list = new List<SplitItem>(32);

			int index = 0;
			int position = 0;
			int line = 0;

			var scopes = new Stack<long>(32);

			SplitStringNormalMode(text, ref index, ref position, ref line, options, list, scopes);

			list.RemoveAll(x => x.text is "");

			ApplyList(list);
			return list;
		}
		public static bool AcceptSeparatedID = false;
		static void ApplyList(List<SplitItem> list)
		{
			Queue<SplitItem> tokens = new Queue<SplitItem>(list);
			list.Clear();
			SplitItem? back = null;
			while (tokens.Count > 0)
			{
				SplitItem token = tokens.Dequeue();
				if (tokens.Count > 0)
				{
					SplitItem next = tokens.Peek();
					if (token == "=" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("==", token.position, token.line));
						continue;
					}
					if (token == "/" && next == "/")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("//", token.position, token.line));
						continue;
					}
					if (token == "/" && next == "*")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("/*", token.position, token.line));
						continue;
					}
					if (token == "*" && next == "/")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("*/", token.position, token.line));
						continue;
					}
					if (token == "!" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("!=", token.position, token.line));
						continue;
					}
					if (token == ">" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem(">=", token.position, token.line));
						continue;
					}
					if (token == "<" && next == "=")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("<=", token.position, token.line));
						continue;
					}
					if (token == "-" && next == ">")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("->", token.position, token.line));
						continue;
					}

					if (token == "&" && next == "&")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("&&", token.position, token.line));
						continue;
					}
					if (token == "|" && next == "|")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("||", token.position, token.line));
						continue;
					}
					if (FinalSplitProcessor != null)
					{
						if (FinalSplitProcessor(token, next, out FinalSplit final))
						{
							back = null;
							tokens.Dequeue();
							list.Add(new SplitItem(final.result, token.position, token.line));
						}
					}
					/* Optional Negative Number Parser */
					if (token == "-")
					{
						if (long.TryParse(next, out long num))
						{
							back = null;
							tokens.Dequeue();
							list.Add(token + next);
							continue;
						}
					}
					if (token == "=" && next == ">")
					{
						back = null;
						tokens.Dequeue();
						list.Add(new SplitItem("=>", token.position, token.line));
						continue;
					}
					/* Optional Dotted Number Parser */
					if (token == ".")
					{
						if (long.TryParse(back, out long num))
						{
							if (long.TryParse(next, out num))
							{
								tokens.Dequeue();
								list.RemoveAt(list.Count - 1);
								list.Add(back + token + next);
								continue;
							}
						}
					}
					/* Optional Separated-By-Character ID */
					if (AcceptSeparatedID)
					{
						if (token == "-" && back != "-" && next != "-" &&
						   Token.IsValidIdentifier(back) && Token.IsValidIdentifier(next))
						{
							tokens.Dequeue();
							list.RemoveAt(list.Count - 1);
							list.Add(back + token + next);
							back = list[list.Count - 1];
							continue;
						}
					}
				}
				back = token;
				list.Add(token);
			}
		}
	}
	public class SplitItem
	{
		public string text;
		public int position;
		public int line;

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
