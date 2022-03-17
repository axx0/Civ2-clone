
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