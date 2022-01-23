namespace NipahTokenizer.NanoDesu
{
    public static class NipahRuntime
    {
        public static DataStructure DATA;
        public static void Error(object error)
        {
            throw new System.Exception(error?.ToString() ?? "UNKNOWN ERROR");
        }
    }
}
