using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NipahTokenizer;

public static class StringBuilderPool
{
    static readonly Queue<StringBuilder> pool = new(32);

    public static StringBuilder Get(int defCapacity = 32)
    {
        return pool.Count is 0 
            ? (new(defCapacity)) 
            : pool.Dequeue();
    }
    public static void Return(StringBuilder builder)
    {
        builder.Clear();
        pool.Enqueue(builder);
    }
    public static string BuildAndReturn(StringBuilder builder)
    {
        string product = builder.ToString();
        builder.Clear();
        pool.Enqueue(builder);

        return product;
    }
}
