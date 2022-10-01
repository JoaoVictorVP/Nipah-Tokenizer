using System.Collections.Generic;

namespace NipahTokenizer;

public class LocalListPool<T>
{
    readonly Queue<List<T>> pool = new(32);

    public List<T> Get(int defCapacity = 32)
    {
        return pool.Count is 0
            ? (new(defCapacity))
            : pool.Dequeue();
    }
    public void Return(List<T> builder)
    {
        builder.Clear();
        pool.Enqueue(builder);
    }
    public string BuildAndReturn(List<T> builder)
    {
        string product = builder.ToString();
        builder.Clear();
        pool.Enqueue(builder);

        return product;
    }
}