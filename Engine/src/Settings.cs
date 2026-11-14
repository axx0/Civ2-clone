using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using Civ2engine.Enums;
using Model.Utils;

namespace Civ2engine
{
    public class Settings
    {
        private static string SettingsFilePath => Path.Combine(ApplicationDataFolder, SettingsFileName);
        
        private static string ApplicationDataFolder => Path.Combine(GetLocalAppDataFolder(), "AxxCiv");
        
        private const string SettingsFileName = "appsettings.json";

        // Game settings from App.config
        public static string Civ2Path { get; private set; }
        
        public static string[] SearchPaths { get; internal set; }

        /// <summary>
        /// How map graphics are filtered when scaled by zoom (Graphic Options).
        /// </summary>
        public static MapScaling MapScaling { get; private set; } = MapScaling.Smooth;

        /// <summary>
        /// Change the map scaling and remember it for the next start
        /// </summary>
        public static void SetMapScaling(MapScaling scaling)
        {
            if (MapScaling == scaling) return;
            MapScaling = scaling;
            Save();
        }

        public static bool LoadConfigSettings()
        {
            if (File.Exists(SettingsFilePath))
            {
                LoadSettings(SettingsFilePath);
                if (!string.IsNullOrWhiteSpace(Civ2Path) && IsValidRoot(Civ2Path))
                {
                    return true;
                }
            }
            var alternativePath = Path.Combine(BasePath, SettingsFileName);

            LoadSettings(alternativePath);

            return !string.IsNullOrWhiteSpace(Civ2Path) && IsValidRoot(Civ2Path);
        }

        public static string BasePath => AppDomain.CurrentDomain.BaseDirectory;

        private static void LoadSettings(string? settingsFilePath)
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
                    Civ2Path = civ2Path!;
                }
            }

            if (root.TryGetProperty(nameof(SearchPaths), out var searchPathsElement))
            {
                var searchPaths = searchPathsElement.EnumerateArray().Select(e => e.GetString()).Where(IsValidRoot).OfType<string>().Concat(
                        [BasePath])
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
                SearchPaths = [Civ2Path, BasePath];
            }

            ReadMapScaling(root);
        }

        /// <summary>
        /// Read the choice saved from Graphic Options ("Pixelated"/"Smooth"); keeps the current value when there is none
        /// </summary>
        private static void ReadMapScaling(JsonElement root)
        {
            if (root.TryGetProperty(nameof(MapScaling), out var scaling) &&
                scaling.ValueKind == JsonValueKind.String &&
                Enum.TryParse<MapScaling>(scaling.GetString(), ignoreCase: true, out var parsed) &&
                Enum.IsDefined(parsed))
            {
                MapScaling = parsed;
            }
        }

        public static bool IsValidRoot(string? civ2Path)
        {
            try
            {
                return !string.IsNullOrWhiteSpace(civ2Path) && Directory.Exists(civ2Path) && FileUtilities.GetFile(civ2Path, RulesFile) != null;
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

            if (string.IsNullOrWhiteSpace(Civ2Path) || !IsValidRoot(Civ2Path))
            {
                Civ2Path = path;
                SearchPaths = new[] { path, BasePath };
            }
            else
            {
                SearchPaths = SearchPaths.Append(path).ToArray();
            }
            Save();// This overwrites the appsettings.
            return true;
        }

        public static void Save()
        {
            if (!Directory.Exists(ApplicationDataFolder))
            {
                Directory.CreateDirectory(ApplicationDataFolder);
            }
            using var stream = File.Create(SettingsFilePath);
            using var writer = new Utf8JsonWriter(stream);
            writer.WriteStartObject();
            writer.WriteString(nameof(Civ2Path),Civ2Path);
            writer.WriteStartArray(nameof(SearchPaths));
            foreach (var searchPath in SearchPaths ?? [])
            {
                writer.WriteStringValue(searchPath);
            }
            writer.WriteEndArray();
            writer.WriteString(nameof(MapScaling), MapScaling.ToString());
            writer.WriteEndObject();
            writer.Flush();
        }
        
        private static string GetLocalAppDataFolder() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Environment.GetEnvironmentVariable("LOCALAPPDATA");
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Environment.GetEnvironmentVariable("XDG_DATA_HOME") ?? Path.Combine(Environment.GetEnvironmentVariable("HOME"),".local","share");
            } 
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), "Library", "Application Support");
            }
            throw new NotImplementedException("Unknown OS Platform");
        }
    }
}
