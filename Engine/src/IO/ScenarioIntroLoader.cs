using System;
using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.IO
{
    public class ScenarioIntroLoader : IFileHandler
    {
        private PopupBox box = new();

        private ScenarioIntroLoader()
        {
            box.Title = "SCENINTRO";
        }

        public static PopupBox LoadIntro(IEnumerable<string> paths, string fileName)
        {
            var filePath = Utils.GetFilePath(fileName, paths);
            var loader = new ScenarioIntroLoader();
            TextFileParser.ParseFile(filePath, loader);
            return loader.box;
        }

        public void ProcessSection(string section, List<string> contents)
        {
            var str = contents.FirstOrDefault(s => s.Contains("@width", StringComparison.OrdinalIgnoreCase));
            if (str != null)
            {
                box.Width = Int32.Parse(str[7..]);
                contents.Remove(str);
            }

            str = contents.FirstOrDefault(s => s.Contains("@title", StringComparison.OrdinalIgnoreCase));
            if (str != null)
            {
                box.Title = str[7..];
                contents.Remove(str);
            }

            box.Text = contents;
        }
    }
}