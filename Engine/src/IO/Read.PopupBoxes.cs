using System;
using System.Collections.Generic;
using Civ2engine.IO;

namespace Civ2engine
{
    public class PopupBoxReader : IFileHandler
    {
        // Read popupbox data from txt file
        public static Dictionary<string, PopupBox?> LoadPopupBoxes(string[] paths, string fileName)
        {
            var boxes = new Dictionary<string, PopupBox?>();
            var filePath = Utils.GetFilePath(fileName, paths);
            TextFileParser.ParseFile(filePath, new PopupBoxReader { Boxes = boxes }, true);
            return boxes;
        }

        public Dictionary<string, PopupBox?> Boxes { get; set; }

        public void ProcessSection(string section, List<string>? contents)
        {
            var popupBox = new PopupBox {Name = section, Checkbox = false};
            Action<string> contentHandler;
            var addOkay = true;

            void TextHandler(string line)
            {
                if (string.IsNullOrWhiteSpace(line) && popupBox.Text?.Count > 0 && popupBox.Options == null && section != "SCENARIO")   // No options in scenario intro text
                {
                    contentHandler = (line) =>
                    {
                        if (string.IsNullOrWhiteSpace(line)) return;
                        (popupBox.Options ??= new List<string>()).Add(line);
                    };
                    return;
                }

                popupBox.AddText(line);
            }

            contentHandler = TextHandler;

            var optionsHandler = new Action<string>((line) =>
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    contentHandler = TextHandler;
                }
                else
                {
                    (popupBox.Options ??= new List<string>()).Add(line);
                }
            });
            var optionsStart = false;
            foreach (var line in contents)
            {
                if (line.StartsWith("@"))
                {
                    var parts = line.Split(new[] { '@', '=' }, 2,
                        StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 0)
                    {
                        continue;
                    }

                    switch (parts[0])
                    {
                        case "width":
                            popupBox.Width = int.Parse(parts[1]);
                            break;
                        case "title":
                            popupBox.Title = parts[1];
                            break;
                        case "default":
                            popupBox.Default = int.Parse(parts[1]);
                            break;
                        case "x":
                            popupBox.X = int.Parse(parts[1]);
                            break;
                        case "y":
                            popupBox.Y = int.Parse(parts[1]);
                            break;
                        case "options":
                            contentHandler = optionsHandler;
                            break;
                        case "checkbox":
                            popupBox.Checkbox = true;
                            break;
                        case "listbox":
                            popupBox.Listbox = true;
                            popupBox.ListboxLines = parts.Length > 1 ? int.Parse(parts[1]) : 16;
                            break;
                        case "button":
                            (popupBox.Button ??= new List<string>()).Add(parts[1]);
                            break;
                    }
                }

                else
                {
                    contentHandler(line);
                }
            }

            popupBox.Button ??= new List<string>();
            popupBox.Button.Add("OK");
            // Add cancel buttons if @options exist
            if (popupBox.Options != null)
            {
                popupBox.Button.Add("Cancel");
            }


            Boxes[popupBox.Name] = popupBox;

        }
    }
}
