using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using S = NipahTokenizer.Separator;

namespace NipahTokenizer;

public record TokenizerOptions(Separator[] Separators, Scope[] Scopes, EndOfLine[] EOFs, SplitAggregator[] Aggregators, bool Parallel)
{
    public static readonly Separator[] DefaultSeparators = new[]
    {
        new S(" ", IncludeMode.None), new S("\t", IncludeMode.None),

        new S("\\*"), new S("\\/"), new S("\\+"), new S("\\-"),
        new S("\\("), new S("\\)"), new S("\n"), new S("\\,"), new S("\\;"),
        new S("\\="), new S("\\{"), new S("\\}"),
        new S("\\["), new S("\\]"), new S("\\:"), new S("\\<"), new S("\\>"),
        new S("\\&"), new S("\\|"), new S("\\$"), new S("\\@"), new S("\\."), new S("\\#"),
        new S("\\!"), new S("\\?")
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
    public readonly char SingleChar;
    public readonly Regex Match;
    public readonly IncludeMode Include;

    public Separator(string match, IncludeMode include = IncludeMode.Separate)
    {
        SingleChar = match.Length is 1 ? match[0] : '\0';
        Match = new Regex(match, RegexOptions.Compiled);
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