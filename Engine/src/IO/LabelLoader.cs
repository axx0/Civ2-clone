using System.Collections.Generic;

namespace Civ2engine
{
    public class LabelLoader : IFileHandler
    {
        public void ProcessSection(string section, List<string>? contents)
        {
            switch (section)
            {
                case "POPUPS":
                    Labels.Ok = contents[0];
                    Labels.Help = contents[1];
                    Labels.Cancel = contents[2];
                    Labels.Custom = contents[3];
                    break;
                case "LABELS":
                    Labels.Items = contents.ToArray();
                    break;
            }
        }
    }
}