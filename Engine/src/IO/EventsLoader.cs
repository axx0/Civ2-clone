using System;
using System.Collections.Generic;
using System.Linq;

namespace Civ2engine.IO
{
    public class EventsLoader : IFileHandler
    {
        Rules _rules;
        LoadedGameObjects _gameObjects;
        List<ScenarioEvent> _scenarios = new ();

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
            return loader._scenarios;
        }

        public void ProcessSection(string section, List<string> contents)
        {
            if (section != "IF") return;

            ITrigger trigger = default;
            string unitKilled, attackerCiv, defenderCiv, talkerCiv, listenerCiv, receiverCiv = string.Empty;
            int talkerType, listenerType;
            switch (contents[0])
            {
                case "UNITKILLED":
                    unitKilled = ReadString(contents, "unit");
                    attackerCiv = ReadString(contents, "attacker");
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new UnitKilled
                    {
                        UnitKilledId = string.Equals(unitKilled, "ANYUNIT", StringComparison.OrdinalIgnoreCase) ? -2 : Int32.Parse(unitKilled),
                        AttackerCivId = string.Equals(attackerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == attackerCiv).Id,
                        DefenderCivId = string.Equals(defenderCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == defenderCiv).Id,
                    };
                    break;
                case "CITYTAKEN":
                    attackerCiv = ReadString(contents, "attacker");
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new CityTaken
                    {
                        City = _gameObjects.Cities.Find(c => c.Name == ReadString(contents, "city")),
                        AttackerCivId = string.Equals(attackerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == attackerCiv).Id,
                        DefenderCivId = string.Equals(defenderCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? 0 : _gameObjects.Civilizations.Find(c => c.TribeName == defenderCiv).Id,
                    };
                    break;
                case "TURN":
                    trigger = new TurnTrigger
                    {
                        Turn = Int32.Parse(ReadString(contents, "turn"))
                    };
                    break;
                case "TURNINTERVAL":
                    trigger = new TurnInterval
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
                    trigger = new Negotiation1
                    {
                        TalkerCivId = string.Equals(talkerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == talkerCiv).Id,
                        ListenerCivId = string.Equals(listenerCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == listenerCiv).Id,
                        TalkerType = talkerType,
                        ListenerType = listenerType
                    };
                    break;
                case "SCENARIOLOADED":
                    trigger = new ScenarioLoaded { };
                    break;
                case "RANDOMTURN":
                    trigger = new RandomTurn 
                    {
                        Denominator = Int32.Parse(ReadString(contents, "denominator"))
                    };
                    break;
                case "NOSCHISM":
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new NoSchism
                    {
                        CivId = string.Equals(defenderCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == defenderCiv).Id,
                    };
                    break;
                case "RECEIVEDTECHNOLOGY":
                    receiverCiv = ReadString(contents, "receiver");
                    trigger = new ReceivedTechnology
                    {
                        TechnologyId = Int32.Parse(ReadString(contents, "technology")),
                        ReceiverCivId = string.Equals(receiverCiv, "ANYBODY", StringComparison.OrdinalIgnoreCase) ? -2 : _gameObjects.Civilizations.Find(c => c.TribeName == receiverCiv).Id,
                    };
                    break;
                default:
                    break;
            }

            var actions = new List<IScenarioAction>();
            string[] actionList = new string[12] { "TEXT", "MOVEUNIT", "CREATEUNIT",
                "CHANGEMONEY", "PLAYWAVEFILE", "MAKEAGGRESSION", "JUSTONCE", "PLAYCDTRACK",
                "DONTPLAYWONDERS", "CHANGETERRAIN", "DESTROYACIVILIZATION", "GIVETECHNOLOGY" };
            foreach ( var actionName in actionList)
            {
                var indx = contents.IndexOf(actionName);
                if (indx == -1) continue;

                IScenarioAction? action = default;
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
                        action = new TextAction
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
                        var arr = contents[indx + 4].Split(',').Select(val => Int32.Parse(val)).ToArray();
                        action = new MoveUnit
                        {
                            UnitMovedId = string.Equals(unitMoved, "ANYUNIT", StringComparison.OrdinalIgnoreCase) ? -2 : 
                                _rules.UnitTypes.ToList().FindIndex(u => u.Name == unitMoved),
                            OwnerCivId = civId,
                            MapCoords = new int[4, 2] { { arr[0], arr[1] }, { arr[2], arr[3] },
                                                        { arr[4], arr[5] }, { arr[6], arr[7] }},
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
                        int[,] locations = new int[locationsNo, 2];
                        for (int i = 0; i < locationsNo; i++)
                        {
                            var ar = contents[indx + 6 + i].Split(',').Select(val => Int32.Parse(val)).ToArray();
                            locations[i, 0] = ar[0];
                            locations[i, 1] = ar[1];
                        }
                        action = new CreateUnit
                        {
                            CreatedUnitId = Int32.Parse(ReadString(contents, "unit", indx + 1)),
                            OwnerCivId = civId,
                            Veteran = string.Equals(ReadString(contents, "veteran", indx + 1), "yes", StringComparison.Ordinal),
                            HomeCity = _gameObjects.Cities.Find(c => c.Name == ReadString(contents, "homecity", indx + 1)),
                            Locations = locations
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
                        action = new ChangeMoney
                        {
                            ReceiverCivId = civId,
                            Amount = Int32.Parse(ReadString(contents, "amount", indx + 1))
                        };
                        break;
                    case "PLAYWAVEFILE":
                        action = new PlayWav
                        {
                            File = contents[indx + 1]
                        };
                        break;
                    case "MAKEAGGRESSION":
                        action = new MakeAggression
                        {
                            WhoCivId = _gameObjects.Civilizations.Find(c => c.TribeName == ReadString(contents, "who", indx + 1)).Id,
                            WhomCivId = _gameObjects.Civilizations.Find(c => c.TribeName == ReadString(contents, "whom", indx + 1)).Id,
                        };
                        break;
                    case "JUSTONCE":
                        //action = new AJustOnce { };
                        break;
                    case "PLAYCDTRACK":
                        action = new PlayCDtrack
                        {
                            TrackNo = Int32.Parse(contents[indx + 1])
                        };
                        break;
                    case "DONTPLAYWONDERS":
                        action = new DontplayWonders { };
                        break;
                    case "CHANGETERRAIN":
                        var arr1 = contents[indx + 3].Split(',').Select(val => Int32.Parse(val)).ToArray();
                        action = new ChangeTerrain
                        {
                            TerrainTypeId = Int32.Parse(ReadString(contents, "terraintype", indx + 1)),
                            MapCoords = new int[4, 2] { { arr1[0], arr1[1] }, { arr1[2], arr1[3] },
                                                        { arr1[4], arr1[5] }, { arr1[6], arr1[7] }},
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
                        action = new DestroyCiv
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
                        action = new GiveTech
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

            _scenarios.Add(new ScenarioEvent
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