using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipahTokenizer;

static class GSPooler<T>
{
    public static readonly Queue<T> Pool = new(32);
}

/// <summary>
/// Represents a generic static pool for any desired type
/// </summary>
public static class GSPool
{
    public static T Get<T>() where T : new()
    {
        return GSPooler<T>.Pool.TryDequeue(out var t) is false 
            ? new() 
            : t;
    }
    public static T Get<T>(Func<T> factory)
    {
        ArgumentNullException.ThrowIfNull(factory);
        return GSPooler<T>.Pool.TryDequeue(out var t) is false
            ? factory()
            : t;
    }
    public static void Return<T>(T instance)
    {
        GSPooler<T>.Pool.Enqueue(instance);
    }
    public static void Return<T>(T instance, Action<T> cleaning)
    {
        ArgumentNullException.ThrowIfNull(cleaning);
        cleaning(instance);
        GSPooler<T>.Pool.Enqueue(instance);
    }
}
