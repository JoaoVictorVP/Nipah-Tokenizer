using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        new SplitAggregator(x => long.TryParse(x, out _), y => y is ".", z => long.TryParse(z, out _))
            .Named("FloatAggregator"),
        // Negative numbers (like -3000)
        new SplitAggregator(x => x is "-", y => long.TryParse(y, out _) || double.TryParse(y, out _))
            .Named("NegativeNumbersAggregator")
    };

    public static readonly TokenizerOptions Default = new(DefaultSeparators, DefaultScopes, DefaultEndOfLines, DefaultAggregators, false);

    public static TokenizerOptions BuildDefault(DefaultTokenizerOptions options)
    {
        var separators = Array.Empty<S>();
        if(options.HasFlag(DefaultTokenizerOptions.Separators))
            separators = DefaultSeparators;
        var scopes = Array.Empty<Scope>();
        if(options.HasFlag(DefaultTokenizerOptions.Scopes))
            scopes = DefaultScopes;
        var eof = Array.Empty<EndOfLine>();
        if(options.HasFlag(DefaultTokenizerOptions.EndOfLines))
            eof = DefaultEndOfLines;
        var aggregators = Array.Empty<SplitAggregator>();
        if(options.HasFlag(DefaultTokenizerOptions.Aggregators))
            aggregators = DefaultAggregators;

        if(options.HasFlag(DefaultTokenizerOptions.SpacesDoSeparate))
            separators = separators
                .Where(s => s.Match is not ' ')
                .Append(new S(' ', IncludeMode.Separate))
                .ToArray();

        aggregators = (SplitAggregator[])aggregators.Clone();
        if (options.HasFlag(DefaultTokenizerOptions.IgnoreNegativeNumberAggregators))
            aggregators = aggregators.Where(aggr => aggr.Name != "NegativeNumbersAggregator").ToArray();
        if(options.HasFlag(DefaultTokenizerOptions.IgnoreFloatingPointNumberAggregators))
            aggregators = aggregators.Where(aggr => aggr.Name != "FloatAggregator").ToArray();

        return new(separators, scopes, eof, aggregators, false);
    }
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
    public string Name { get; set; } = "Aggregator";
    public readonly Predicate<string>[] Detectors;

    public SplitAggregator Named(string name)
    {
        Name = name;
        return this;
    }

    public SplitAggregator(params Predicate<string>[] detectors)
    {
        Detectors = detectors;
    }

    public SplitAggregator(params string[] detectors)
    {
        Detectors = detectors.Select(Predicate<string> (x) => (cmp) => x == cmp).ToArray();
    }
}

[Flags]
public enum DefaultTokenizerOptions
{
    None = 0,
    Separators = 1 << 0,
    Scopes = 1 << 1,
    EndOfLines = 1 << 2,
    Aggregators = 1 << 3,

    SpacesDoSeparate = 1 << 4,

    IgnoreNegativeNumberAggregators = 1 << 5,

    IgnoreFloatingPointNumberAggregators = 1 << 6,

    Defaults = Separators | Scopes | EndOfLines | Aggregators
}
