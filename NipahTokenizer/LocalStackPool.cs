using System.Collections.Generic;

namespace NipahTokenizer;

public class LocalStackPool<T>
{
    readonly Queue<Stack<T>> pool = new(32);

    public Stack<T> Get(int defCapacity = 32)
    {
        return pool.Count is 0
            ? (new(defCapacity))
            : pool.Dequeue();
    }
    public void Return(Stack<T> builder)
    {
        builder.Clear();
        pool.Enqueue(builder);
    }
    public string BuildAndReturn(Stack<T> builder)
    {
        string product = builder.ToString();
        builder.Clear();
        pool.Enqueue(builder);

        return product;
    }
}