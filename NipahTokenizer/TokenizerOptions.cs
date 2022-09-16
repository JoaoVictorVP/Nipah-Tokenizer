using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using S = NipahTokenizer.Separator;

namespace NipahTokenizer;

public record TokenizerOptions(Separator[] Separators, Scope[] Scopes, EndOfLine[] EOFs)
{
    public static readonly Separator[] DefaultSeparators = new[]
    {
        new S(" ", IncludeMode.None), new S("\t", IncludeMode.None),

        new S("\\*"), new S("\\/"), new S("\\+"), new S("\\-"),
        new S("\\("), new S("\\)"), new S("\n"), new S("\\,"), new S("\\;"),
        new S("\\="), new S("\\{"), new S("\\}"),
        new S("\\["), new S("\\]"), new S("\\:"), new S("\\<"), new S("\\>"),
        new S("\\&"), new S("\\|"), new S("\\$"), new S("\\@"), new S("\\."), new S("\\#"),
        new S("\\!")
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

    public static readonly TokenizerOptions Default = new(DefaultSeparators, DefaultScopes, DefaultEndOfLines);
}

public class Separator
{
    public readonly Regex Match;
    public readonly IncludeMode Include;

    public Separator(string match, IncludeMode include = IncludeMode.Separate)
    {
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