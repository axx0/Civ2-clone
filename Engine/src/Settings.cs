using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace Civ2engine
{
    public class Settings
    {
        private const string SettingsFileName = "appsettings.json";

        // Game settings from App.config
        public static string Civ2Path { get; private set; }
        
        public static string[] SearchPaths { get; private set; }

        public static bool LoadConfigSettings()
        {
            var settingsFilePath = Path.Combine(BasePath, SettingsFileName);

            LoadSettings(settingsFilePath);

            return !string.IsNullOrWhiteSpace(Civ2Path) && IsValidRoot(Civ2Path);
        }

        public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        private static void LoadSettings(string settingsFilePath)
        {
            if (!File.Exists(settingsFilePath)) return;

            var contents = File.ReadAllText(settingsFilePath, Encoding.UTF8);

            var settingsDoc = JsonDocument.Parse(contents);

            var root = settingsDoc.RootElement;

            if (root.TryGetProperty(nameof(Civ2Path), out var civ2PathElement))
            {
                var civ2Path = civ2PathElement.GetString();
                if (IsValidRoot(civ2Path))
                {
                    Civ2Path = civ2Path;
                }
            }

            if (root.TryGetProperty(nameof(SearchPaths), out var searchPathsElement))
            {
                var searchPaths = searchPathsElement.EnumerateArray().Select(e => e.GetString()).Where(IsValidRoot).Concat(new [] {BasePath})
                    .ToArray();
                if (!string.IsNullOrWhiteSpace(Civ2Path))
                {
                    SearchPaths = !searchPaths.Contains(Civ2Path) ? new[] { Civ2Path }.Concat(searchPaths).ToArray() : searchPaths;
                }
                else if(searchPaths.Length > 0)
                {
                    Civ2Path = searchPaths[0];
                    SearchPaths = searchPaths;
                }
                
            }else if (!string.IsNullOrWhiteSpace(Civ2Path))
            {
                SearchPaths = new[] { Civ2Path, BasePath };
            }
        }

        public static bool IsValidRoot(string civ2Path)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(civ2Path) && Directory.Exists(civ2Path) &&
                       (File.Exists(Path.Combine(civ2Path, RulesFile)) ||
                        File.Exists(Path.Combine(civ2Path, RulesFile.ToUpper())));
            }
            catch
            {
                return false;
            }
        }

        private const string RulesFile = "rules.txt";

        public static bool AddPath(string path)
        {
            if (!IsValidRoot(path))
            {
                var dir = Path.GetDirectoryName(path);
                if (!IsValidRoot(dir)) return false;
                path = dir;
            }

            if (string.IsNullOrWhiteSpace(Civ2Path))
            {
                Civ2Path = path;
                SearchPaths = new[] { path, BasePath };
            }
            else
            {
                SearchPaths = SearchPaths.Append(path).ToArray();
            }
            Save();
            return true;
        }

        public static void Save()
        {
            using var writer = new Utf8JsonWriter(File.OpenWrite(SettingsFileName));
            writer.WriteStartObject();
            writer.WriteString(nameof(Civ2Path),Civ2Path);
            writer.WriteStartArray(nameof(SearchPaths));
            foreach (var searchPath in SearchPaths)
            {
                writer.WriteStringValue(searchPath);
            }
            writer.WriteEndArray();
            writer.WriteEndObject();
            writer.Flush();
        }
    }
}
