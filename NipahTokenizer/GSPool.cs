using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipahTokenizer;

public static class GSPool<T> where T : new()
{
    static readonly Queue<T> pool = new(32);

    public static T Get()
    {
        return pool.TryDequeue(out var t) is false 
            ? new() 
            : t;
    }
    public static void Return(T instance)
    {
        pool.Enqueue(instance);
    }
}
