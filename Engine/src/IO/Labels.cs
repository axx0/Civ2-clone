
namespace Civ2engine
{
    public static class Labels
    {
        public static string Ok { get; set; }
        public static string Cancel { get; set; }
        public static string Help { get; set; }
        public static string Custom { get; set; }

        public static string[] Items { get; set; }

        public static string For(LabelIndex index)
        {
            return Items[(int)index];
        }
        
        public static string For(LabelIndex index, params string[] strings)
        {
            var label = Items[(int)index];
            for (int i = 0; i < strings.Length; i++)
            {
                var rep = "%STRING" + i;
                if (label.Contains(rep))
                {
                    label = label.Replace(rep, strings[i]);
                }
            }

            return label.Split("|")[0];
        }

        private static string _currentPath;
        
        public static void UpdateLabels(Ruleset rules)
        {
            var labelPath = rules != null ? Utils.GetFilePath("labels.txt", rules.Paths) : Utils.GetFilePath("labels.txt");
            if (labelPath == _currentPath) return;
            
            _currentPath = labelPath;
            TextFileParser.ParseFile(labelPath, new LabelLoader());
        }
    }
}