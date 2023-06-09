namespace PackForIP
{
    public static class Product
    {
        private const string PlatformStr = "{\n    \"platform\": \"%PL\"\n}";

        public static string GetProductString(string platform)
        {
            return PlatformStr.Replace("%PL", platform);
        }

    }
}