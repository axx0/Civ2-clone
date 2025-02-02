using Civ2engine;

namespace Model.Core;

public interface IScriptEngine
{
    void Execute(string command);
    string Log { get; }
    void Connect(IInterfaceCommands playerUi);
    void RunScript(string scriptFile);
    void RunPlayerScript(IPlayer player);
}