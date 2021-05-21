
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
            
            string line;
            string section = null;
            List<string> contents = null;
            var reading = false;
            while ((line = file.ReadLine()) != null)
            {
                if (reading)
                {
                    if (allowBlanks && string.IsNullOrWhiteSpace(line))
                    {
                        line = file.ReadLine();
                        if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
                        {
                            handler.ProcessSection(section, contents);
                            reading = false;
                        }
                        else if (line.StartsWith('@'))
                        {
                            handler.ProcessSection(section, contents);
                            section = line[1..];
                            contents = new List<string>();
                        }
                        else
                        {
                            contents.Add("");
                            contents.Add(line);
                        }
                    }else if (string.IsNullOrWhiteSpace(line) || line.StartsWith(';'))
                    {
                        handler.ProcessSection(section, contents);
                        reading = false;
                    }
                    else
                    {
                        contents.Add(line);
                    }
                }
                else
                {
                    if (!line.StartsWith('@')) continue;
                    section = line[1..];
                    reading = true;
                    contents = new List<string>();
                }
            }

            if (reading && contents.Count > 0)
            {
                handler.ProcessSection(section, contents);
            }
        }
    }

    public interface IFileHandler
    {
        void ProcessSection(string section, List<string> contents);
    }
}