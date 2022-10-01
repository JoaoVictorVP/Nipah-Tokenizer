using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using S = NipahTokenizer.Separator;

#nullable enable

namespace NipahTokenizer;

/// <summary>
/// Construct a new instance of tokenizer options
/// </summary>
/// <param name="Separators">The separators to be used by tokenizer</param>
/// <param name="Scopes">The scope delimiters for the tokenizer to use</param>
/// <param name="EOFs">The end of line delimitators</param>
/// <param name="Aggregators">The aggregators to pipeline some tokens and produce others sequentially</param>
/// <param name="LetRawTokens">Should use raw tokens instead of processed ones?<br/>Raw tokens are tokens that had not their values processed in any way, thus they will be always of type Any and their values will be only inexistent.</param>
/// <param name="Parallel">Parallelize the tokenization?<br/>If true you can reach gains up to 50% of performance in larger texts, but at the cost of producing less precise tokenizations as result of the fast text splitting in chunks before tokenization.<br/>If false you can still reach better speeds if the inputs are small because of the inherent overhead of threading.</param>
public record TokenizerOptions(
    Separator[] Separators, 
    Scope[] Scopes, 
    EndOfLine[] EOFs, 
    SplitAggregator[] Aggregators, 
    bool LetRawTokens = false,
    bool Parallel = false)
{
    public static readonly Separator[] DefaultSeparators = new[]
    {
        new S(' ', IncludeMode.None), new S('\t', IncludeMode.None),

        new S('*'), new S('/'), new S('+'), new S('-'),
        new S('('), new S(')'), new S('\n'), new S(','), new S(';'),
        new S('='), new S('{'), new S('}'),
        new S('['), new S(']'), new S(':'), new S('<'), new S('>'),
        new S('&'), new S('|'), new S('$'), new S('@'), new S('.'), new S('#'),
        new S('!'), new S('?')
    };
    public static readonly Scope[] DefaultScopes = new Scope[]
    {
        new('\"', '\"'),
        new('\'', '\'')
    };
    public static readonly EndOfLine[] DefaultEndOfLines = new EndOfLine[]
    {
        new('\n'),
        new('\0')
    };

    public static readonly SplitAggregator[] DefaultAggregators = new SplitAggregator[]
    {
        new("=", "="), new("!", "="), new(">", "="), new("<", "="),
        new("/", "/"), new("/", "*"), new("*", "/"),
        new("-", ">"), new("=", ">"),
        new("&", "&"), new("|", "|"),
        // Floating point numbers (like 3.14 or 9,36)
        new(x => long.TryParse(x, out _), y => y is ".", z => long.TryParse(z, out _)),
        // Negative numbers (like -3000)
        new(x => x is "-", y => long.TryParse(y, out _) || double.TryParse(y, out _))
    };

    public static readonly TokenizerOptions Default = new(DefaultSeparators, DefaultScopes, DefaultEndOfLines, DefaultAggregators, false);
}

public class Separator
{
    public readonly char Match;
    public readonly IncludeMode Include;
    string? matchAsString = null;
    public string MatchAsString => matchAsString ??= Match.ToString();

    public Separator(char match, IncludeMode include = IncludeMode.Separate)
    {
        Match = match;
        Include = include;
    }
}
public enum IncludeMode
{
    /// <summary>
    /// Will not include separator (util for things like ' ')
    /// </summary>
    None,
    /// <summary>
    /// Will aggregate separator with current token (for things like '-' if needed)
    /// </summary>
    Aggregate,
    /// <summary>
    /// Will generate a separated token for separator (for things like '.')
    /// </summary>
    Separate
}

public class Scope
{
    static long gid;

    public readonly long Id;
    public readonly char Begin;
    public readonly char End;

    public Scope(char begin, char end)
    {
        Begin = begin;
        End = end;

        lock(this)
        {
            Id = gid;
            gid++;
        }
    }
}

public class EndOfLine
{
    public readonly char EOF;

    public EndOfLine(char eOF)
    {
        EOF = eOF;
    }
}

public class SplitAggregator
{
    public readonly Predicate<string>[] Detectors;

    public SplitAggregator(params Predicate<string>[] detectors)
    {
        Detectors = detectors;
    }

    public SplitAggregator(params string[] detectors)
    {
        Detectors = detectors.Select(Predicate<string> (x) => (cmp) => x == cmp).ToArray();
    }
}