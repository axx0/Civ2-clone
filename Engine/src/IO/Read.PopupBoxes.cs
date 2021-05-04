using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace Civ2engine
{
    public partial class Read
    {
        // Read Game.txt
        public static List<PopupBox> PopupBoxes(string root)
        {
            string filePath_ = root + Path.DirectorySeparatorChar + "Game.txt";
            string filePath = null;
            if (File.Exists(filePath_))
            {
                filePath = filePath_;
            }
            else
            {
                Console.WriteLine("Game.txt not found!");
            }

            var popupBoxList = new List<PopupBox>();

            // Find starting indexes of popubox definitions in Game.txt
            var startIndexList = new List<int>();
            int count = 0;
            foreach (var line in File.ReadLines(filePath))
            {
                if (line.Length > 1 && line[0] == '@' && !line.Contains("@width") && !line.Contains("@title") && !line.Contains("@options") && !line.Contains("@checkbox") && !line.Contains("@button") && !line.Contains("@height") && !line.Contains("@columns") && !line.Contains("@default") && !line.Contains("@listbox") && !line.Contains("@x") && !line.Contains("@y"))
                    startIndexList.Add(count);
                count += 1;
            }

            var startIndex = startIndexList.ToArray();
            var lines = File.ReadAllLines(filePath);
            for (int group = 0; group < startIndex.Length - 1; group++) // Group = one popupbox definition
            {
                var popupBox = new PopupBox { Name = lines[startIndex[group]].Remove(0, 1) };
                bool optionsStart = false;
                for (int row = startIndex[group]; row < startIndex[group + 1]; row++)
                {
                    if (lines[row].Contains("@@") || lines[row].Contains("@ ")) continue;

                    if (lines[row].Contains("@width")) popupBox.Width = Int32.Parse(Regex.Match(lines[row], @"\d+").Value); // Extract number from string
                    if (lines[row].Contains("@title")) popupBox.Title = lines[row].Substring(lines[row].LastIndexOf('=') + 1);
                    if (lines[row].Contains("@default")) popupBox.Default = Int32.Parse(Regex.Match(lines[row], @"\d+").Value);
                    if (lines[row].Contains("@x")) popupBox.X = Int32.Parse(Regex.Match(lines[row], @"\d+").Value);
                    if (lines[row].Contains("@y")) popupBox.Y = Int32.Parse(Regex.Match(lines[row], @"\d+").Value);
                    if (lines[row].Length > 1 && lines[row].Substring(0, 1) == "^" && lines[row].Substring(1, 1) != "^")
                    {
                        if (popupBox.LeftText == null) popupBox.LeftText = new List<string>();
                        popupBox.LeftText.Add(lines[row].Substring(lines[row].LastIndexOf('^') + 1));
                    }
                    if (lines[row].Length > 2 && lines[row].Substring(0, 2) == "^^")
                    {
                        if (popupBox.CenterText == null) popupBox.CenterText = new List<string>();
                        popupBox.CenterText.Add(lines[row].Substring(lines[row].LastIndexOf('^') + 1));
                    }
                    if (lines[row].Contains("@button")) 
                    {
                        if (popupBox.Button == null) popupBox.Button = new List<string>();
                        popupBox.Button.Add(lines[row].Substring(lines[row].LastIndexOf('=') + 1)); 
                    }
                    if (lines[row].Contains("@options")) optionsStart = true;
                    if (lines[row].Length > 1 && lines[row][0] != '@' && optionsStart)
                    {
                        if (popupBox.Options == null) popupBox.Options = new List<string>();
                        popupBox.Options.Add(lines[row]);
                    }
                }

                // Add OK/cancel buttons if @options exist
                if (popupBox.Options != null)
                {
                    if (popupBox.Button == null) popupBox.Button = new List<string>();
                    popupBox.Button.Add("OK");
                    popupBox.Button.Add("Cancel");
                }
                // Otherwise just add OK
                else
                {
                    if (popupBox.Button == null) popupBox.Button = new List<string>();
                    popupBox.Button.Add("OK");
                }

                popupBoxList.Add(popupBox);
            }

            return popupBoxList;
        }
    }
}
