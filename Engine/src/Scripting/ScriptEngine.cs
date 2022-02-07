using System;
using System.Text;
using Neo.IronLua;

namespace Civ2engine.Scripting
{
    public class ScriptEngine : IDisposable
    {
        private readonly Lua _lua;
        private readonly LuaGlobal _environment;
        private StringBuilder _log { get; }

        public ScriptEngine(IInterfaceCommands uInterfaceCommands)
        {
            _lua = new Lua();
            _environment = _lua.CreateEnvironment();
            dynamic dg = _environment;
            _log = new StringBuilder();
            _log.AppendLine(_environment.Version);
            dg.print = new Action<string>(s => _log.AppendLine(s));
            dg.civ = new LuaTable();
            dg.civ.ui = BuildUiCommands(uInterfaceCommands, _log);
        }

        private static LuaTable BuildUiCommands(IInterfaceCommands uInterfaceCommands, StringBuilder log)
        {
            dynamic ui = new LuaTable();
            ui.text = new Action<string>((text)  =>
            {
                var pop = new PopupBox
                {
                    Button = new [] { "OK" },
                    Text = new [] { text },
                    LineStyles = new [] { TextStyles.Left }
                };
                uInterfaceCommands.ShowDialog(pop);
            });
            ui.createDialog = new Func<Dialog>(() => new Dialog(uInterfaceCommands, log));
            return ui;
        }

        public string Log => _log.ToString();

        public void Execute(string script)
        {
            try
            {
                _log.AppendLine("> " + script);
                _environment.DoChunk(script, "immediate.lua");
            }
            catch (Exception e)
            {
                _log.AppendLine(e.Message);
            }
        }

        public void Dispose()
        {
            _lua?.Dispose();
        }
    }
}