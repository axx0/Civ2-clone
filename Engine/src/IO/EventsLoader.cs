using System;
using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.IO
{
    public class EventsLoader : IFileHandler
    {
        Rules _rules;
        LoadedGameObjects _gameObjects;
        List<ScenarioEvent> Scenarios = new List<ScenarioEvent>();

        private EventsLoader()
        {
        }

        public static List<ScenarioEvent> LoadEvents(IEnumerable<string> paths, Rules rules, LoadedGameObjects objects)
        {
            var filePath = Utils.GetFilePath("events.txt", paths);
            var loader = new EventsLoader();
            loader._rules = rules;
            loader._gameObjects = objects;
            TextFileParser.ParseFile(filePath, loader);
            return loader.Scenarios;
        }

        public void ProcessSection(string section, List<string> contents)
        {
            if (section != "IF") return;

            ITrigger trigger = default;
            string attackerCiv, defenderCiv, talkerCiv, listenerCiv, receiverCiv = string.Empty;
            int talkerType, listenerType;
            switch (contents[0])
            {
                case "UNITKILLED":
                    attackerCiv = ReadString(contents, "attacker");
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new TUnitKilled
                    {
                        UnitKilled = _rules.UnitTypes.ToList().Find(t => t.Name == ReadString(contents, "unit")).Type,
                        AttackerCivId = string.Equals(attackerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == attackerCiv).Id,
                        DefenderCivId = string.Equals(defenderCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == defenderCiv).Id,
                    };
                    break;
                case "CITYTAKEN":
                    attackerCiv = ReadString(contents, "attacker");
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new TCityTaken
                    {
                        City = _gameObjects.Cities.Find(c => c.Name == ReadString(contents, "city")),
                        AttackerCivId = string.Equals(attackerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == attackerCiv).Id,
                        DefenderCivId = string.Equals(defenderCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == defenderCiv).Id,
                    };
                    break;
                case "TURN":
                    trigger = new TTurn
                    {
                        Turn = Int32.Parse(ReadString(contents, "turn"))
                    };
                    break;
                case "TURNINTERVAL":
                    trigger = new TTurnInterval
                    {
                        Interval = Int32.Parse(ReadString(contents, "interval"))
                    };
                    break;
                case "NEGOTIATION":
                    talkerCiv = ReadString(contents, "talker");
                    listenerCiv = ReadString(contents, "listener");
                    if (string.Equals(ReadString(contents, "talkertype"), "Human", StringComparison.OrdinalIgnoreCase))
                    {
                        talkerType = 1;
                    }
                    else if (string.Equals(ReadString(contents, "talkertype"), "Computer", StringComparison.OrdinalIgnoreCase))
                    {
                        talkerType = 2;
                    }
                    else
                    {
                        talkerType = 4;
                    }
                    if (string.Equals(ReadString(contents, "listenertype"), "Human", StringComparison.OrdinalIgnoreCase))
                    {
                        listenerType = 1;
                    }
                    else if (string.Equals(ReadString(contents, "listenertype"), "Computer", StringComparison.OrdinalIgnoreCase))
                    {
                        listenerType = 2;
                    }
                    else
                    {
                        listenerType = 4;
                    }
                    trigger = new TNegotiation
                    {
                        TalkerCivId = string.Equals(talkerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == talkerCiv).Id,
                        ListenerCivId = string.Equals(listenerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == listenerCiv).Id,
                        TalkerType = talkerType,
                        ListenerType = listenerType
                    };
                    break;
                case "SCENARIOLOADED":
                    trigger = new TScenarioLoaded { };
                    break;
                case "RANDOMTURN":
                    trigger = new TRandomTurn 
                    {
                        Denominator = Int32.Parse(ReadString(contents, "denominator"))
                    };
                    break;
                case "NOSCHISM":
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new TNoSchism
                    {
                        CivId = string.Equals(defenderCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == defenderCiv).Id,
                    };
                    break;
                case "RECEIVEDTECHNOLOGY":
                    receiverCiv = ReadString(contents, "receiver");
                    trigger = new TReceivedTechnology
                    {
                        TechnologyId = Int32.Parse(ReadString(contents, "technology")),
                        ReceiverCivId = string.Equals(receiverCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == receiverCiv).Id,
                    };
                    break;
                default:
                    break;
            }

            var actions = new List<IAction>();
            string[] actionList = new string[12] { "TEXT", "MOVEUNIT", "CREATEUNIT",
                "CHANGEMONEY", "PLAYWAVEFILE", "MAKEAGGRESSION", "JUSTONCE", "PLAYCDTRACK",
                "DONTPLAYWONDERS", "CHANGETERRAIN", "DESTROYACIVILIZATION", "GIVETECHNOLOGY" };
            foreach ( var actionName in actionList)
            {
                var indx = contents.IndexOf(actionName);
                if (indx == -1) continue;

                IAction? action = default;
                string unitMoved, civ, number;
                int civId;
                switch (actionName)
                {
                    case "TEXT":
                        var indxEnd = contents.IndexOf("ENDTEXT");
                        var txtList = new List<string>();
                        for (int i = indx + 1; i < indxEnd; i++)
                        {
                            txtList.Add(contents[i]);
                        }
                        action = new AText
                        {
                            Strings = new List<string>(txtList)
                        };
                        break;
                    case "MOVEUNIT":
                        unitMoved = ReadString(contents, "unit", indx + 1);
                        civ = ReadString(contents, "owner", indx + 1);
                        civId = 0;
                        if (string.Equals(civ, "TRIGGERRECEIVER", StringComparison.OrdinalIgnoreCase) 
                            || string.Equals(civ, "TRIGGERDEFENDER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -4;
                        }
                        else if (string.Equals(civ, "TRIGGERATTACKER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.Find(c => c.TribeName == civ).Id;
                        }
                        number = ReadString(contents, "numbertomove", indx + 1);
                        action = new AMoveUnit
                        {
                            UnitMovedId = string.Equals(unitMoved, "ANYUNIT", StringComparison.OrdinalIgnoreCase) ? -2 : 
                                _rules.UnitTypes.ToList().FindIndex(u => u.Name == unitMoved),
                            OwnerCivId = civId,
                            MapCoords = contents[indx + 4].Split(',').Select(val => Int32.Parse(val)).ToArray(),
                            MapDest = contents[indx + 6].Split(',').Select(val => Int32.Parse(val)).ToArray(),
                            NumberToMove = string.Equals(number, "ALL", StringComparison.OrdinalIgnoreCase) ? -2 : Int32.Parse(number)
                        };
                        break;
                    case "CREATEUNIT":
                        civ = ReadString(contents, "owner", indx + 1);
                        civId = 0;
                        if (string.Equals(civ, "TRIGGERRECEIVER", StringComparison.OrdinalIgnoreCase) 
                            || string.Equals(civ, "TRIGGERDEFENDER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -4;
                        }
                        else if (string.Equals(civ, "TRIGGERATTACKER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.Find(c => c.TribeName == civ).Id;
                        }
                        int locationsNo = contents.IndexOf("endlocations") - contents.IndexOf("locations") - 1;
                        int[,] ar = new int[locationsNo, 2];
                        for (int i = 0; i < locationsNo; i++)
                        {
                            var _ar = contents[indx + 6 + i].Split(',').Select(val => Int32.Parse(val)).ToArray();
                            ar[i, 0] = _ar[0];
                            ar[i, 1] = _ar[1];
                        }
                        action = new ACreateUnit
                        {
                            CreatedUnit = _rules.UnitTypes.ToList().Find(t => t.Name == ReadString(contents, "unit", indx + 1)).Type,
                            OwnerCivId = civId,
                            Veteran = string.Equals(ReadString(contents, "veteran", indx + 1), "yes", StringComparison.Ordinal),
                            HomeCity = _gameObjects.Cities.Find(c => c.Name == ReadString(contents, "homecity", indx + 1)),
                            Locations = ar
                        };
                        break;
                    case "CHANGEMONEY":
                        civ = ReadString(contents, "receiver", indx + 1);//contents[indx + 1][9..];
                        civId = 0;
                        if (string.Equals(civ, "TRIGGERRECEIVER", StringComparison.OrdinalIgnoreCase) 
                            || string.Equals(civ, "TRIGGERDEFENDER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -4;
                        }
                        else if (string.Equals(civ, "TRIGGERATTACKER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.Find(c => c.TribeName == civ).Id;
                        }
                        action = new AChangeMoney
                        {
                            ReceiverCivId = civId,
                            Amount = Int32.Parse(ReadString(contents, "amount", indx + 1))
                        };
                        break;
                    case "PLAYWAVEFILE":
                        action = new APlayWAV
                        {
                            File = contents[indx + 1]
                        };
                        break;
                    case "MAKEAGGRESSION":
                        action = new AMakeAggression
                        {
                            WhoCivId = _gameObjects.Civilizations.Find(c => c.TribeName == ReadString(contents, "who", indx + 1)).Id,
                            WhomCivId = _gameObjects.Civilizations.Find(c => c.TribeName == ReadString(contents, "whom", indx + 1)).Id,
                        };
                        break;
                    case "JUSTONCE":
                        action = new AJustOnce { };
                        break;
                    case "PLAYCDTRACK":
                        action = new APlayCDtrack
                        {
                            TrackNo = Int32.Parse(contents[indx + 1])
                        };
                        break;
                    case "DONTPLAYWONDERS":
                        action = new ADontplayWonders { };
                        break;
                    case "CHANGETERRAIN":
                        action = new AChangeTerrain
                        {
                            TerrainTypeId = Int32.Parse(ReadString(contents, "terraintype", indx + 1)),
                            MapCoords = contents[indx + 3].Split(',').Select(val => Int32.Parse(val)).ToArray(),
                        };
                        break;
                    case "DESTROYACIVILIZATION":
                        civ = ReadString(contents, "whom", indx + 1);
                        civId = 0;
                        if (string.Equals(civ, "TRIGGERRECEIVER", StringComparison.OrdinalIgnoreCase) 
                            || string.Equals(civ, "TRIGGERDEFENDER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -4;
                        }
                        else if (string.Equals(civ, "TRIGGERATTACKER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.Find(c => c.TribeName == civ).Id;
                        }
                        action = new ADestroyCiv
                        {
                            CivId = civId,
                        };
                        break;
                    case "GIVETECHNOLOGY":
                        civ = ReadString(contents, "receiver", indx + 1);
                        civId = 0;
                        if (string.Equals(civ, "TRIGGERRECEIVER", StringComparison.OrdinalIgnoreCase)
                            || string.Equals(civ, "TRIGGERDEFENDER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -4;
                        }
                        else if (string.Equals(civ, "TRIGGERATTACKER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.Find(c => c.TribeName == civ).Id;
                        }
                        action = new AGiveTech
                        {
                            CivId = civId,
                            TechId = Int32.Parse(ReadString(contents, "technology", indx + 1))
                        };
                        break;
                    default:
                        break;
                }

                actions.Add(action);
            }

            Scenarios.Add(new ScenarioEvent
            {
                Trigger = trigger,
                Actions = actions
            });
        }

        private static string ReadString(List<string> strings, string keyword, int startIndex = 1)
        {
            foreach (string s in strings.Skip(startIndex))
            {
                if (s.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return s.Substring(keyword.Length + 1).Trim();
                }
            }

            return "";
        }
    }
}