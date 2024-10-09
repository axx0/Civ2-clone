using Civ2engine;
using Civ2engine.IO;
using Model;
using Model.Menu;
using Raylib_CSharp.Interact;

namespace Civ2.Menu
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

        public void ProcessSection(string section, List<string>? contents)
        {
            _elements[section] = contents.Select(s =>
                {
                    var idx = s.IndexOf("&", StringComparison.Ordinal);
                    if (idx == -1 || idx >= s.Length - 1 ||
                        !Enum.TryParse<KeyboardKey>( s.Substring(idx + 1, 1), true, out var hotkey))
                    {
                        hotkey = KeyboardKey.Null;
                    }

                    idx = s.IndexOf("|", StringComparison.Ordinal);
                    var shortcut = idx == -1 ? Shortcut.None : Shortcut.Parse(s[idx..]);

                    return new MenuElement(s, shortcut, hotkey);
                }
            ).ToList();
        }

        public static List<MenuElement> For(string section)
        {
            return _elements.TryGetValue(section, out var element) ? element : Enumerable.Empty<MenuElement>().ToList() ;
        }

        public static IList<string> Menus => _elements.Keys.ToList();
    }
}