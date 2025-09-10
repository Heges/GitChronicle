namespace Utils.Utils
{
    public class ArgumentParser
    {
        public Dictionary<string, string> MappedArgs => _map;

        private readonly Dictionary<string, string> _map = new(StringComparer.OrdinalIgnoreCase);

        public ArgumentParser(string[] args) 
        {
            for (int i = 0; i < args.Length; i++)
            {
                var a = args[i];
                if (!a.StartsWith("-")) continue;

                if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    _map[a.ToLower().TrimStart('-')] = args[i + 1];
                    i++;
                }
                else
                {
                    _map[a] = "true";
                }
            }
        }

        public bool TryGet(string key, out string value) => _map.TryGetValue(key, out value!);
        public bool Has(string key) => _map.ContainsKey(key);
    }
}
