using System;
using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.IO
{
    public class ScenarioIntroLoader : IFileHandler
    {
        private PopupBox _box = new();

        private ScenarioIntroLoader()
        {
            _box.Title = "SCENINTRO";
        }

        public static PopupBox LoadIntro(IEnumerable<string> paths, string fileName)
        {
            var filePath = Utils.GetFilePath(fileName, paths);
            var loader = new ScenarioIntroLoader();
            TextFileParser.ParseFile(filePath, loader);
            return loader._box;
        }

        public void ProcessSection(string section, List<string>? contents)
        {
            var str = contents.FirstOrDefault(s => s.Contains("@width", StringComparison.OrdinalIgnoreCase));
            if (str != null)
            {
                _box.Width = Int32.Parse(str[7..]);
                contents.Remove(str);
            }

            str = contents.FirstOrDefault(s => s.Contains("@title", StringComparison.OrdinalIgnoreCase));
            if (str != null)
            {
                _box.Title = str[7..];
                contents.Remove(str);
            }

            _box.Text = contents;
        }
    }
}