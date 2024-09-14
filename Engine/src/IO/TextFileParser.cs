
using System.Collections.Generic;
using System.IO;

namespace Civ2engine
{
    public static class TextFileParser
    {
        public static void ParseFile(string filePath, IFileHandler handler, bool allowBlanks = false)
        {
            if(string.IsNullOrWhiteSpace(filePath)) return;
            
            using var file = new StreamReader(filePath);
            
            string? line;
            string? section = null;
            List<string> contents = new();
            while ((line = file.ReadLine()) != null)
            {
                if (section != null)
                {
                    if (allowBlanks && string.IsNullOrWhiteSpace(line))
                    {
                        line = file.ReadLine();
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
                        {
                            handler.ProcessSection(section, contents);
                            section = null;
                        }
                        else if (line.StartsWith('@'))
                        {
                            handler.ProcessSection(section, contents);
                            section = line[1..];
                            contents.Clear();
                        }
                        else
                        {
                            contents.Add("");
                            contents.Add(line);
                        }
                    }else if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
                    {
                        handler.ProcessSection(section, contents);
                        section = null;
                    }
                    else
                    {
                        contents.Add(line);
                    }
                }
                else
                {
                    if (!line.StartsWith('@')) continue;
                    section = line[1..].TrimEnd();
                    contents = new List<string>();
                }
            }

            if (section != null && contents.Count > 0)
            {
                handler.ProcessSection(section, contents);
            }
        }
    }
}