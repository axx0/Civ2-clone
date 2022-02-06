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

        public ScriptEngine()
        {
            _lua = new Lua();
            _environment = _lua.CreateEnvironment();
            dynamic dg = _environment;
            _log = new StringBuilder();
            _log.AppendLine(_environment.Version);
            dg.print = new Action<string>((s => _log.AppendLine(s)));
        }

        public string Log => _log.ToString();

        public void Execute(string script)
        {
            try
            {
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