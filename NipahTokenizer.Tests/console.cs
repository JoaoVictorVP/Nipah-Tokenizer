global using static console;
public static class console
{
    public static void print(object value) => Console.Write(value);
    public static void printf(object value) => Console.WriteLine(value);
    public static void pause() => Console.ReadKey(true);

    public static void clear() => Console.Clear();
    public static string read(string? label = null)
    {
        if (label != null)
            Console.Write(label);
        return Console.ReadLine()!;
    }
}