using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Civ2engine.Scripting.ScriptObjects;
using Model.Core;
using Neo.IronLua;

namespace Civ2engine.Scripting
{
    public class ScriptEngine : IDisposable, IScriptEngine
    {
        private readonly Game _game;
        private readonly Lua _lua;
        private readonly LuaGlobal _environment;
        private readonly List<string> _scriptPaths;
        private readonly CivScripts _civScripts;
        private readonly StringBuilder _log; 

        public ScriptEngine(Game game, string[] paths)
        {
            _game = game;
            _scriptPaths = paths.ToList();
            _scriptPaths.Add(Environment.CurrentDirectory + Path.DirectorySeparatorChar + "Scripts"); 
            _lua = new Lua();
            _environment = _lua.CreateEnvironment();
            dynamic dg = _environment;
            _log = new StringBuilder();
            _log.AppendLine(_environment.Version);
            _civScripts = new CivScripts(_log, game);
            dg.print = new Action<string>(s => _log.AppendLine(s));
            dg.civ = _civScripts;
            dg.AiEvent = new AiEventMap();
            dg.AiRoleType = new AiRoleTypeMap();
        }

        public void Connect(IInterfaceCommands interfaceCommands)
        {
            _civScripts.Connect(interfaceCommands);
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

        public void RunPlayerScript(IPlayer player)
        {
            if (player is AiPlayer aiPlayer)
            {
                aiPlayer.AI = new AiInterface(aiPlayer, _game);
                var scriptName = aiPlayer.AIScript;
                if (!scriptName.EndsWith(".lua"))
                {
                    scriptName += ".lua";
                }

                dynamic dg = _environment;
                dg.ai = aiPlayer.AI;
                RunScript(scriptName);
                dg.ai = null;
            }
        }
    }
}