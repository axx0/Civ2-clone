// ReSharper disable InconsistentNaming
namespace Civ2engine.Scripting.ScriptObjects;

public class Leader(Civilization civ)
{
    public string name => civ.LeaderName;
    public string title => civ.LeaderTitle;
    public int gender => civ.LeaderGender;
}
