using System.Collections.Generic;

namespace Civ2engine;

public interface IFileHandler
{
    void ProcessSection(string section, List<string>? contents);
}