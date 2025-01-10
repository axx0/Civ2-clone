using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.OriginalSaves;
using Model;

namespace Civ2engine.IO
{
    public class EventsLoader : IFileHandler
    {
        Rules _rules;
        ILoadedGameObjects _gameObjects;
        List<ScenarioEvent> _scenarios = new ();

        private EventsLoader()
        {
        }

        public static List<ScenarioEvent> LoadEvents(IEnumerable<string> paths, Rules rules, ILoadedGameObjects objects)
        {
            var filePath = Utils.GetFilePath("events.txt", paths);
            var loader = new EventsLoader();
            loader._rules = rules;
            loader._gameObjects = objects;
            TextFileParser.ParseFile(filePath, loader);
            return loader._scenarios;
        }

        public void ProcessSection(string section, List<string>? contents)
        {
            contents = contents.Select(str => str.TrimEnd(' ')).ToList();  // remove whitespaces at end

            if (section != "IF") return;

            ITrigger? trigger = default;
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
                        UnitKilledId = StringEquals(unitKilled, "ANYUNIT") ? -2 : _rules.UnitTypes.ToList().FindIndex(u => StringEquals(u.Name, unitKilled)),
                        AttackerCivId = StringEquals(attackerCiv, "ANYBODY") ? 0 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, attackerCiv)),
                        DefenderCivId = StringEquals(defenderCiv, "ANYBODY") ? 0 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, defenderCiv)),
                    };
                    break;
                case "CITYTAKEN":
                    attackerCiv = ReadString(contents, "attacker");
                    defenderCiv = ReadString(contents, "defender");
                    trigger = new CityTaken
                    {
                        City = _gameObjects.Cities.Find(c => StringEquals(c.Name, ReadString(contents, "city"))),
                        AttackerCivId = StringEquals(attackerCiv, "ANYBODY") ? 0 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, attackerCiv)),
                        DefenderCivId = StringEquals(defenderCiv, "ANYBODY") ? 0 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, defenderCiv)),
                    };
                    break;
                case "TURN":
                    int turn = -2;
                    var str = ReadString(contents, "turn");
                    if (!Int32.TryParse(ReadString(contents, "turn"), out turn))
                    {
                        if (StringEquals(str, "EVERY"))
                            turn = -1;
                    }
                    trigger = new TurnTrigger { Turn = turn };
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
                    if (StringEquals(ReadString(contents, "talkertype"), "Human"))
                    {
                        talkerType = 1;
                    }
                    else if (StringEquals(ReadString(contents, "talkertype"), "Computer"))
                    {
                        talkerType = 2;
                    }
                    else
                    {
                        talkerType = 4;
                    }
                    if (StringEquals(ReadString(contents, "listenertype"), "Human"))
                    {
                        listenerType = 1;
                    }
                    else if (StringEquals(ReadString(contents, "listenertype"), "Computer"))
                    {
                        listenerType = 2;
                    }
                    else
                    {
                        listenerType = 4;
                    }
                    trigger = new Negotiation1
                    {
                        TalkerCivId = StringEquals(talkerCiv, "ANYBODY") ? -2 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, talkerCiv)),
                        ListenerCivId = StringEquals(listenerCiv, "ANYBODY") ? -2 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, listenerCiv)),
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
                        CivId = StringEquals(defenderCiv, "ANYBODY") ? -2 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, defenderCiv)),
                    };
                    break;
                case "RECEIVEDTECHNOLOGY":
                    receiverCiv = ReadString(contents, "receiver");
                    int civId;
                    if (StringEquals(receiverCiv, "ANYBODY"))
                    {
                        civId = -2;
                    }
                    else if (StringEquals(receiverCiv, "TRIGGERRECEIVER"))
                    {
                        civId = -1;
                    }
                    else
                    {
                        civId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, receiverCiv));
                    }
                    var futureTechInString = contents.FirstOrDefault(s => s.Contains("futuretech"));
                    trigger = new ReceivedTechnology
                    {
                        TechnologyId = futureTechInString == null ? Int32.Parse(ReadString(contents, "technology")) :
                                                                    Int32.Parse(ReadString(contents, "futuretech")),
                        IsFutureTech = futureTechInString != null,
                        ReceiverCivId = civId,
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
                        if (StringEquals(civ, "TRIGGERRECEIVER") 
                            || StringEquals(civ, "TRIGGERDEFENDER"))
                        {
                            civId = -4;
                        }
                        else if (StringEquals(civ, "TRIGGERATTACKER"))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, civ));
                        }
                        number = ReadString(contents, "numbertomove", indx + 1);
                        var arr = contents[indx + 4].Split(',', '.').Select(val => Int32.Parse(val)).ToArray();
                        action = new MoveUnit
                        {
                            UnitMovedId = StringEquals(unitMoved, "ANYUNIT") ? -2 : 
                                _rules.UnitTypes.ToList().FindIndex(u => StringEquals(u.Name, unitMoved)),
                            OwnerCivId = civId,
                            MapCoords = new int[4, 2] { { arr[0], arr[1] }, { arr[2], arr[3] },
                                                        { arr[4], arr[5] }, { arr[6], arr[7] }},
                            MapDest = contents[indx + 6].Split(',', '.').Select(val => Int32.Parse(val)).ToArray(),
                            NumberToMove = StringEquals(number, "ALL") ? -2 : Int32.Parse(number)
                        };
                        break;
                    case "CREATEUNIT":
                        civ = ReadString(contents, "owner", indx + 1);
                        civId = 0;
                        if (StringEquals(civ, "TRIGGERRECEIVER") 
                            || StringEquals(civ, "TRIGGERDEFENDER"))
                        {
                            civId = -4;
                        }
                        else if (StringEquals(civ, "TRIGGERATTACKER") || !_gameObjects.Civilizations.Any(c => StringEquals(c.TribeName, civ)))
                        {
                            civId = -3;
                        }
                        else if (StringEquals(civ, "barbarians"))
                        {
                            civId = 0;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, civ));
                        }
                        int[,] locations;
                        if (contents.Contains("incapital"))
                        {
                            locations = new int[,] { { 0 } , { 0 } };    // TODO
                        }
                        else
                        {
                            int locationsStart = contents.IndexOf("locations");
                            int locationsNo = contents.IndexOf("endlocations") - locationsStart - 1;
                            locations = new int[locationsNo, 2];
                            for (int i = 0; i < locationsNo; i++)
                            {
                                var line = contents[locationsStart + 1 + i].Split(',', '.');
                                line = line.Select(s => s.Replace('o', '0')).ToArray(); // if inadvertedly o was pressed
                                var ar = line.Select(val => Int32.Parse(val)).ToArray();
                                locations[i, 0] = ar[0];
                                locations[i, 1] = ar[1];
                            }
                        }
                        action = new CreateUnit
                        {
                            CreatedUnitId = _rules.UnitTypes.ToList().FindIndex(u => StringEquals(u.Name, ReadString(contents, "unit", indx + 1))),
                            OwnerCivId = civId,
                            Veteran = StringEquals(ReadString(contents, "veteran", indx + 1), "yes"),
                            HomeCity = _gameObjects.Cities.Find(c => StringEquals(c.Name, ReadString(contents, "homecity", indx + 1))),
                            Locations = locations
                        };
                        break;
                    case "CHANGEMONEY":
                        civ = ReadString(contents, "receiver", indx + 1);//contents[indx + 1][9..];
                        civId = 0;
                        if (StringEquals(civ, "TRIGGERRECEIVER") 
                            || StringEquals(civ, "TRIGGERDEFENDER"))
                        {
                            civId = -4;
                        }
                        else if (StringEquals(civ, "TRIGGERATTACKER"))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, civ));
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
                            WhoCivId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, ReadString(contents, "who", indx + 1))),
                            WhomCivId = StringEquals("triggerreceiver", ReadString(contents, "whom", indx + 1)) ? 
                            -1 : _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, ReadString(contents, "whom", indx + 1))),
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
                        var arr1 = contents[contents.IndexOf("maprect") + 1].Split(',', '.').Select(val => Int32.Parse(val)).ToArray();
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
                        if (StringEquals(civ, "TRIGGERRECEIVER") 
                            || StringEquals(civ, "TRIGGERDEFENDER"))
                        {
                            civId = -4;
                        }
                        else if (StringEquals(civ, "TRIGGERATTACKER"))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, civ));
                        }
                        action = new DestroyCiv
                        {
                            CivId = civId,
                        };
                        break;
                    case "GIVETECHNOLOGY":
                        civ = ReadString(contents, "receiver", indx + 1);
                        civId = 0;
                        if (StringEquals(civ, "TRIGGERRECEIVER")
                            || StringEquals(civ, "TRIGGERDEFENDER"))
                        {
                            civId = -4;
                        }
                        else if (string.Equals(civ, "TRIGGERATTACKER", StringComparison.OrdinalIgnoreCase))
                        {
                            civId = -3;
                        }
                        else
                        {
                            civId = _gameObjects.Civilizations.FindIndex(c => StringEquals(c.TribeName, civ));
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

        private static string ReadString(List<string>? strings, string keyword, int startIndex = 1)
        {
            foreach (string s in strings.Skip(startIndex))
            {
                if (s.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                {
                    return s[(keyword.Length + 1)..].Trim();
                }
            }

            return "";
        }

        private bool StringEquals(string s1, string s2) => string.Equals(s1, s2, StringComparison.OrdinalIgnoreCase);
    }
}