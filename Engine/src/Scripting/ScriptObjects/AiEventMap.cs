// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// Disabling warnings as members are used in Lua
namespace Civ2engine.Scripting.ScriptObjects;

public class AiEventMap
{
    public string Turn_Start = AiEvent.TurnStart;
    public string Turn_End = AiEvent.TurnEnd;
    public string Unit_Orders_Needed = AiEvent.UnitOrdersNeeded;
    public string Research_Complete = AiEvent.ResearchComplete;
}