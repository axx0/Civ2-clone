using System.Collections.Generic;

namespace Civ2engine.IO
{
    public class NameLoader : IFileHandler
    {
        private NameLoader()
        {
            this.CityNames = new Dictionary<string, List<string>?>();
        }

        private Dictionary<string, List<string>?> CityNames { get; set; }


        public static Dictionary<string, List<string>?> LoadCityNames(IEnumerable<string> paths)
        {
            var filePath = Utils.GetFilePath("city.txt", paths);
            var loader = new NameLoader();
            TextFileParser.ParseFile(filePath,loader);
            return loader.CityNames;
        }
        public void ProcessSection(string section, List<string>? contents)
        {
            if (contents.Count > 0 && !CityNames.ContainsKey(section))  // Skip duplicate entries
            {
                CityNames.Add(section, contents);
            }
        }
    }
}