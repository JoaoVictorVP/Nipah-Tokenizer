using System.Collections.Generic;
using System.Text;

namespace NipahTokenizer;

public class LocalStringBuilderPool
{
    readonly Queue<StringBuilder> pool = new(32);

    public StringBuilder Get(int defCapacity = 32)
    {
        return pool.Count is 0
            ? (new(defCapacity))
            : pool.Dequeue();
    }
    public void Return(StringBuilder builder)
    {
        builder.Clear();
        pool.Enqueue(builder);
    }
    public string BuildAndReturn(StringBuilder builder)
    {
        string product = builder.ToString();
        builder.Clear();
        pool.Enqueue(builder);

        return product;
    }
}