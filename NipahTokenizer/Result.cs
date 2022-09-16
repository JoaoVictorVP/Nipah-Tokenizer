using System;

#nullable enable

namespace NipahTokenizer;

public struct Result<T>
{
    public static readonly Result<T> None = new(false);

    readonly T? value;
    readonly bool valid;

    public bool Valid => valid;

    public T? Solve() => valid ? value! : default;
    public T Solve(T defaultValue) => valid ? value! : defaultValue;

    public Result(T value)
    {
        this.value = value;
        valid = true;
    }
    Result(bool valid)
    {
        value = default;
        this.valid = valid;
    }

    public static implicit operator Result<T>(T value) => new(value);
    public static implicit operator Result<T>(DBNull _) => new(false);

    public override string ToString()
        => valid switch
        {
            true => value?.ToString() ?? "<null>",
            false => "<no value>"
        };
}