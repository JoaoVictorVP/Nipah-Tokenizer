using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipahTokenizer;

public static class StringBuilderPool
{
    public static LocalStringBuilderPool Pool => pool;
    static readonly LocalStringBuilderPool pool = new();

    public static StringBuilder Get(int defCapacity = 32)
    {
        lock (pool)
        {
            return pool.Get(defCapacity);
        }
    }
    public static void Return(StringBuilder builder)
    {
        lock (pool)
        {
            pool.Return(builder);
        }
    }
    public static string BuildAndReturn(StringBuilder builder)
    {
        lock (pool)
        {
            return pool.BuildAndReturn(builder);
        }
    }
}
