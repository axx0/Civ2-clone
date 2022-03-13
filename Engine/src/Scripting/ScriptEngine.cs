using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Neo.IronLua;

namespace Civ2engine.Scripting
{
    public class ScriptEngine : IDisposable
    {
        private readonly Lua _lua;
        private readonly LuaGlobal _environment;
        private readonly List<string> _scriptPaths;
        private StringBuilder _log { get; }

        public ScriptEngine(IInterfaceCommands uInterfaceCommands, Game game, string[] paths)
        {
            _scriptPaths = paths.ToList();
            _scriptPaths.Add(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Scripts"); 
            _lua = new Lua();
            _environment = _lua.CreateEnvironment();
            dynamic dg = _environment;
            _log = new StringBuilder();
            _log.AppendLine(_environment.Version);
            dg.print = new Action<string>(s => _log.AppendLine(s));
            dg.civ = new CivScripts(uInterfaceCommands, _log, game);
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

        public void RunScript(string scriptFileName)
        {
            var filePath = Utils.GetFilePath(scriptFileName, _scriptPaths);
            if (filePath == null)
            {
                _log.AppendLine($"Failed to locate script file: {scriptFileName}");
                return;
            }

            try
            {
                _environment.DoChunk(filePath);
            }
            catch (Exception e)
            {
                _log.AppendLine($"Exception running script {scriptFileName}");
                _log.AppendLine(e.Message);
            }
        }
    }
}