using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.IO
{
    public class MenuLoader : IFileHandler
    {
        public static void LoadMenus(Ruleset ruleset)
        {
            _elements = new Dictionary<string, List<MenuElement>>();
            var filePath = Utils.GetFilePath("Menu.txt", ruleset.Paths);
            TextFileParser.ParseFile(filePath, new MenuLoader ());
        }

        private static IDictionary<string, List<MenuElement>> _elements; 

        public void ProcessSection(string section, List<string> contents)
        {
            _elements[section] = contents.Select(s =>
                {
                    var parts = s.Split("|");
                    return new MenuElement(parts[0], parts.Length > 1 ? parts[1] : "");
                }
            ).ToList();
        }

        public static IEnumerable<MenuElement> For(string section)
        {
            return _elements.TryGetValue(section, out var element) ? element : Enumerable.Empty<MenuElement>() ;
        }

        public static IList<string> Menus => _elements.Keys.ToList();
    }

    public record MenuElement(string MenuText, string Shortcut);
}