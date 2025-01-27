using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Model.Core.Advances;
using Model.Core.Cities;
using Raylib_CSharp;
using Path = System.IO.Path;

namespace Civ2engine.IO
{
    public class RulesParser : IFileHandler
    {
        private Rules Rules { get; init; }

        private readonly Dictionary<string, Action<string[]>> _sectionHandlers = new();

        private RulesParser(Rules rules)
        {
            this.Rules = rules; 
            _sectionHandlers.Add("COSMIC", ProcessCosmicRules);
            _sectionHandlers.Add("COSMIC2", ProcessExtraMovementAdjustments);
            _sectionHandlers.Add("CIVILIZE", ProcessTech);
            _sectionHandlers.Add("IMPROVE", ProcessImprovements);
            // ReSharper disable once StringLiteralTypo
            _sectionHandlers.Add("ENDWONDER", ProcessEndWonders);
            _sectionHandlers.Add("UNITS", ProcessUnits);
            _sectionHandlers.Add("GOVERNMENTS", ProcessGovernments);
            _sectionHandlers.Add("LEADERS", ProcessLeaders);
            _sectionHandlers.Add("ORDERS", ProcessOrders);
            _sectionHandlers.Add("CARAVAN", ProcessGoods);
            _sectionHandlers.Add("CIVILIZE2", ProcessAdvanceGroups);
            _sectionHandlers.Add("LEADERS2", ProcessTechGroupAssignments);
            _sectionHandlers.Add("DIFFICULTY", strings => Rules.Difficulty = strings.ToArray() );
            _sectionHandlers.Add("ATTITUDES", strings => Rules.Attitude = strings.ToArray());
            _sectionHandlers.Add("SOUNDS", ProcessAttackSounds);
            _sectionHandlers.Add("UNITS_ADVANCED", ProcessAdvancedUnitFlags);
            _sectionHandlers.Add("SECONDARY_MAPS", SecondaryMaps);
        }

        private void ProcessTechGroupAssignments(string[] values)
        {
            for (var i = 0; i < values.Length; i++)
            {
                if (Rules.Leaders.Length <= i)
                {
                    return;
                }

                var tabOrSpace = values[i].IndexOfAny("\t ".ToCharArray());
                var allowances = values[i][..tabOrSpace];

                Rules.Leaders[i].AdvanceGroups =
                    allowances.Select(c => (AdvanceGroupAccess)int.Parse(c.ToString())).ToArray();
            }
        }

        public static Rules ParseRules(Ruleset ruleset)
        {
            var rules = new Rules();
            _rulesetPaths = ruleset.Paths;
            var filePath = Utils.GetFilePath("RULES.txt", _rulesetPaths);
            TextFileParser.ParseFile(filePath, new RulesParser(rules));
            return rules;
        }

        private readonly Dictionary<string, string> _soundPaths = new();
        
        private void ProcessAttackSounds(string[] values)
        {
            var limit = values.Length < Rules.UnitTypes.Length ? values.Length : Rules.UnitTypes.Length;
            for (var i = 0; i < limit; i++)
            {
                var soundFile = values[i].Split(';', 2, StringSplitOptions.TrimEntries)[0];
                if (string.IsNullOrWhiteSpace(soundFile) || soundFile == "<none>") continue;

                if (!_soundPaths.ContainsKey(soundFile))
                {
                    LocateSoundFile(soundFile);
                }

                Rules.UnitTypes[i].AttackSound = _soundPaths[soundFile];
            }
        }

        private void LocateSoundFile(string soundFile)
        {
            _soundPaths.Add(soundFile, Utils.GetFilePath(soundFile,
                _rulesetPaths.SelectMany(p => new[] {p + Path.DirectorySeparatorChar + "Sound", p})));
        }

        private void SecondaryMaps(string[] values)
        {
            var maps = new List<MapParams>(Rules.Maps);       
            maps.AddRange(values.Select(line =>
            {
                var parts = line.Split(",", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
                var mapParams = new MapParams
                {
                    Type = (MapType) parts[0],
                    BlobSize = parts[1],
                    NumberOfBlobs = parts[2],
                    BridgeLength = parts[3],
                    BridgesPerBlob = parts[4]
                };
                var terrainParams = new List<TerrainParams>();
                if (parts[5] > 0 || parts[8] > 0)
                {
                    terrainParams.Add(new TerrainParams { Type= TerrainType.Mountains, Frequency = parts[5] > parts[8] ? parts[5] : parts[8]});
                }

                if (parts[6] > 0)
                {
                    terrainParams.Add(new TerrainParams { Type= TerrainType.Forest, Frequency = parts[6]});
                }

                if (parts[7] > 0)
                {
                    terrainParams.Add(new TerrainParams { Type = TerrainType.Plains, Frequency = parts[7]});
                }

                for (var i = 11; i < parts.Length; i += 3)
                {
                    if (parts[i] > 0)
                    {
                        terrainParams.Add(new TerrainParams { Type = GetTypeFor(mapParams.Type, (i - 11) / 3), Frequency = parts[i], MinLength = parts[i-1], MeanLength = parts[i-2] });
                    }
                }
                mapParams.TerrainParams = terrainParams.ToArray();
                return mapParams;
            }));
            Rules.Maps = maps.ToArray();
        }

        private readonly IDictionary<MapType, TerrainType[]> _dmfTypesMap = new Dictionary<MapType, TerrainType[]>()
        {
            {MapType.Undersea, new [] {TerrainType.Forest, TerrainType.Jungle}},
            {MapType.Floating, new [] {TerrainType.Mountains, TerrainType.Hills}},
            {MapType.LandDominant, new [] {TerrainType.Forest, TerrainType.Mountains, TerrainType.Hills}},
            {MapType.GasGiant, new [] {TerrainType.Hills}}
        };

        private TerrainType GetTypeFor(MapType mapType, int dmfIndex)
        {
            if (!_dmfTypesMap.ContainsKey(mapType))
            {
                throw new ArgumentException($"No mappings exist for type {mapType}");
            }

            if (_dmfTypesMap[mapType].Length > dmfIndex)
            {
                return _dmfTypesMap[mapType][dmfIndex];
            }
            
            throw new ArgumentException(
                $"There are only {_dmfTypesMap[mapType].Length} mapping for type {mapType}. {dmfIndex} requested!");
        }

        private void ProcessAdvanceGroups(string[] values)
        {
            var limit = values.Length < Rules.Advances.Length ? values.Length : Rules.Advances.Length;
            for (var i = 0; i < limit ; i++)
            {
                Rules.Advances[i].AdvanceGroup = int.Parse(values[i].Split(';', 2, StringSplitOptions.TrimEntries)[0]);
            }
        }

        private static readonly IList<OrderType> Orders = new[]
        {
            OrderType.Fortify, OrderType.Fortified, OrderType.Sleep, OrderType.BuildFortress, OrderType.BuildRoad,
            OrderType.BuildIrrigation, OrderType.BuildMine, OrderType.Transform, OrderType.CleanPollution,
            OrderType.BuildTransport1, OrderType.BuildTransport2, OrderType.BuildTransport3, OrderType.GoTo
        };
        private void ProcessOrders(string[] values)
        {
            Rules.Orders = values.Select((line, id) =>
            {
                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                return new Orders
                {
                    Id = id,
                    Name = parts[0],
                    Key = parts[1],
                    Type = (int)Orders[id]
                };
            }).ToArray();
            Rules.Orders[^1].Type = (int)OrderType.GoTo; //if TOT this is a NOOP otherwise it fixes mislabeling as Transport1
        }

        private void ProcessGoods(string[] values)
        {
            Rules.CaravanCommoditie =
                values.Select((value, id) => new Commodity { Id = id, Name = value.Split(',', StringSplitOptions.TrimEntries)[0]}).ToArray();
        }

        private void ProcessLeaders(string[] values)
        {
            values = values[0..21];

            Rules.Leaders = values.Select((value,id) =>
            {
                var line = value.Split(',', StringSplitOptions.TrimEntries);
                var titles = new List<LeaderTitle>();
                for(var i = 12; i < line.Length;i+=3)
                {
                    titles.Add(new LeaderTitle
                    {
                        Gov = int.TryParse(line[i-2], out int val0) ? val0 : 0,
                        TitleMale = line[i-1],
                        TitleFemale = line[i],
                    });
                }
                return new LeaderDefaults()
                {
                    TribeId = id,
                    NameMale = line[0],
                    NameFemale = line[1],
                    Female = int.Parse(line[2]) == 1,
                    Color = int.TryParse(line[3], out int val) ? val : 0,
                    CityStyle = int.TryParse(line[4], out val) ? val : 0,
                    Plural = line[5],
                    Adjective = line[6],
                    Attack = int.TryParse(line[7], out val) ? val : 0,
                    Expand = int.TryParse(line[8], out val) ? val : 0,
                    Civilize = int.TryParse(line[9], out val) ? val : 0,
                    Titles = titles.ToArray()
                };
            }).ToArray();
        }

        private int SupportFromLevel(int level)
        {
            if (level == 0) return -1;
            if (level == 2) return 0;
            return _freeSupports[_nextGov++];
        }

        private int _nextGov;
        private int[] _freeSupports;

        private void ProcessGovernments(string[] values)
        {
            _freeSupports = new [] { Rules.Cosmic.MonarchyPaysSupport, Rules.Cosmic.CommunismPaysSupport, Rules.Cosmic.FundamentalismPaysSupport };
            
            Rules.Governments = values.Select((value, idx) =>
            {
                var line = value.Split(',', StringSplitOptions.TrimEntries);
                var level = Math.Min((int)Math.Floor((idx+1)/3f),2);
                return new Government
                {
                    Name = line[0],
                    TitleMale = line[1],
                    TitleFemale = line[2],
                    Level = level,
                    NumberOfFreeUnitsPerCity = SupportFromLevel(level),
                    UnitTypesAlwaysFree = idx == 4 ? this.Rules.UnitTypes.Where(u=>u.Flags[3] == '1').Select(u=>u.Type).ToArray() : Array.Empty<int>(),
                    Distance = DefaultDistanceFromIndex(idx),
                    SettlersConsumption = idx > 2 ? this.Rules.Cosmic.SettlersEatFromCommunism : Rules.Cosmic.SettlersEatTillMonarchy
                };
            }).ToArray();
        }

        private int DefaultDistanceFromIndex(int idx)
        {
            switch (idx)
            {
                case 4: //Fundamentalism
                case 6: //Democracy
                    return 0;
                case 3: //Communism
                    return Rules.Cosmic.CommunismEquivalentPalaceDistance;
                default:
                    return -1;
            }
        }

        private void ProcessTerrain(IEnumerable<string>? values)
        {
            var terrains = new List<string>();
            var bonus = new List<string>();
            var mappings = new Dictionary<string, int> {{"yes", -1}, {"no", -2}};
            for (int row = 0; row < 33; row++)
            {
                var parts = values.ElementAt(row).Split(';', StringSplitOptions.TrimEntries);
                if (row < 11)
                {
                    if (!mappings.ContainsKey(parts[1]))
                        mappings.Add(parts[1].Length > 3 ? parts[1][..3] : parts[1], terrains.Count);
                    terrains.Add(parts[0]);
                }
                else
                {
                    bonus.Add(parts[0]);
                }
            }

            //foreach (var t in values)
            //{
            //    var parts = t.Split(';', StringSplitOptions.TrimEntries);
            //    if (t.Split(',', StringSplitOptions.TrimEntries).Length < 8)
            //    {
            //        bonus.Add(parts[0]);
            //    }
            //    else
            //    {
            //        if (!mappings.ContainsKey(parts[1]))
            //            mappings.Add(parts[1].Substring(0, 3), terrains.Count);
            //        terrains.Add(parts[0]);
            //    }
            //}

            Rules.Terrains ??= new List<Terrain[]>();

            Rules.Terrains.Add(terrains.Select((value, type) =>
            {
                var line = value.Split(',', StringSplitOptions.TrimEntries);
                return new Terrain
                {
                    Type = (TerrainType) type,
                    Name = line[0],
                    MoveCost = int.TryParse(line[1], out int val) ? val : 1,
                    Defense = int.TryParse(line[2], out val) ? val : 1,
                    Food = int.TryParse(line[3], out val) ? val : 0,
                    Shields = int.TryParse(line[4], out val) ? val : 0,
                    Trade = int.TryParse(line[5], out val) ? val : 0,
                    CanIrrigate = mappings[line[6]],
                    IrrigationBonus = int.TryParse(line[7], out val) ? val : 0,
                    TurnsToIrrigate = int.TryParse(line[8], out val) ? val : 0,
                    MinGovrnLevelAItoPerformIrrigation = int.TryParse(line[9], out val) ? val : 0,
                    CanMine = mappings[line[10]],
                    MiningBonus = int.TryParse(line[11], out val) ? val : 0,
                    TurnsToMine = int.TryParse(line[12], out val) ? val : 0,
                    MinGovrnLevelAItoPerformMining = int.TryParse(line[13], out val) ? val : 0,
                    Transform = mappings[line[14]],
                    Impassable = line[15] == "yes",
                    RoadBonus = type <= (int)TerrainType.Grassland ? 1:0, 
                    Specials = new[]
                    {
                        MakeSpecial(bonus[type]), MakeSpecial(bonus[type + terrains.Count])
                    }
                };
            }).ToArray());
        }


        private static string[] _rulesetPaths;

        private void ProcessUnits(string[] values)
        {
            LocateSoundFile("CATAPULT.WAV");
            LocateSoundFile("ELEPHANT.WAV");
            
            var defaultAttackSounds = new[] {
                Tuple.Create((int)UnitType.Catapult, _soundPaths["CATAPULT.WAV"]),
                Tuple.Create((int)UnitType.Elephant, _soundPaths["ELEPHANT.WAV"])
            };
            
            Rules.UnitTypes = values.Select((line, type) =>
            {
                var text = line.Split(',', StringSplitOptions.TrimEntries);
                var unit = new UnitDefinition
                {
                    Type = type,
                    Name = text[0],
                    Until = Rules.AdvanceMappings.TryGetValue(text[1], out int value) ? value : -1,
                    Domain = (UnitGas) (int.TryParse(text[2], out value) ? value : 0),
                    Move = Rules.Cosmic.MovementMultiplier * (int.TryParse(text[3].Replace(".", string.Empty), out value) ? value : 0),
                    Range = int.TryParse(text[4], out value) ? value : 0,
                    Attack = int.TryParse(text[5].Replace("a", string.Empty), out value) ? value : 0,
                    Defense = int.TryParse(text[6].Replace("d", string.Empty), out value) ? value : 0,
                    Hitp = 10 * (int.TryParse(text[7].Replace("h", string.Empty), out value) ? value : 0),
                    Firepwr = int.TryParse(text[8].Replace("f", string.Empty), out value) ? value : 0,
                    Cost = int.TryParse(text[9], out value) ? value : 0,
                    Hold = int.TryParse(text[10], out value) ? value : 0,
                    AIrole = (AiRoleType)(int.TryParse(text[11], out value) ? value : 0),
                    Prereq = Rules.AdvanceMappings.ContainsKey(text[12]) ? Rules.AdvanceMappings[text[12]] : -1,    // temp
                    Flags = text[13],
                    AttackSound = defaultAttackSounds.FirstOrDefault(s=>s.Item1 == type)?.Item2
                };
                unit.IsSettler = unit.AIrole == AiRoleType.Settle;
                
                if (!unit.IsSettler) return unit;
                
                if (unit.Prereq == -1)
                {
                    unit.WorkRate = 1;
                }
                else
                {
                    unit.WorkRate = 2;
                    unit.IsEngineer = true;
                }
                return unit;
            }).ToArray();
            
        }
        
        private void ProcessAdvancedUnitFlags(string[] values)
        {
            var limit = values.Length < Rules.UnitTypes.Length ? values.Length : Rules.UnitTypes.Length;
            for (int i = 0; i < limit; i++)
            {
                var line = values[i].Split(new []{ ',',';'}, StringSplitOptions.TrimEntries);
                var unit = Rules.UnitTypes[i];
                unit.CivCanBuild = ReadBitsReversed(line[0]);
                unit.CanBeOnMap = ReadBitsReversed(line[1]);
                unit.MinBribe = int.TryParse(line[2], out int val) ? val : 0;
                var extraFlags = ReadBitsReversed(line[6]);
                unit.Invisible = extraFlags[0];
                unit.NonDispandable = extraFlags[1];
                unit.UnbribaleBarb = extraFlags[3];
                unit.NothingImpassable = extraFlags[4];
                unit.IsEngineer = unit.IsEngineer || extraFlags[5];
                unit.NonExpireForBarbarian = extraFlags[6];
            }
        }

        private bool[] ReadBitsReversed(string bitfield)
        {
            return bitfield.Select(c => c == '1').Reverse().ToArray();
        }

        private void ProcessEndWonders(string[] values)
        {
            var improvementIndex = Rules.Improvements.Length - 1;
            for (var i = values.Length - 1; i >= 0 && improvementIndex >=0; i--)
            {
                var wonder = Rules.Improvements[improvementIndex];
                wonder.IsWonder = true;
                if (!values[i].StartsWith("nil"))
                {
                    wonder.ExpiresAt =
                        Rules.AdvanceMappings.ContainsKey(values[i].Split(',', 2)[0]) ? Rules.AdvanceMappings[values[i].Split(',', 2)[0]] : -1; // temp
                }
                improvementIndex--;
            }

            Rules.FirstWonderIndex = improvementIndex + 1;
        }

        private void ProcessImprovements(string[] values)
        {
            Rules.Improvements = values.Select((value, type) =>
            {
                var parts = value.Split(',', StringSplitOptions.TrimEntries);
                return new Improvement
                {
                    Type = type,
                    Name = parts[0],
                    Cost = int.TryParse(parts[1], out int val) ? val : 1,
                    Upkeep = int.TryParse(parts[2], out val) ? val : 0,
                    Prerequisite = Rules.AdvanceMappings.TryGetValue(parts[3], out val) ? val : -1
                };
            }).ToArray();
        }


        private void ProcessCosmicRules(string[] values)
        {
            var type = typeof(CosmicRules);
            var props = type.GetProperties();
            var cosmic = this.Rules.Cosmic;
            var limit = values.Length < 30 ? values.Length : 30;
            for (var i = 0; i < limit; i++)
            {
                var value = values[i].Split(";", 2, StringSplitOptions.TrimEntries)[0];
                
                if (int.TryParse(value, out var result))
                {
                    props[i].SetValue(cosmic, result);
                }
            }
            
            if (30 >= values.Length) return;
            
            Rules.Cosmic.MapHasGoddyHuts =
                ReadBitsReversed(values[30].Split(';', 2, StringSplitOptions.TrimEntries)[0]);
            
            if (31 < values.Length)
            {
                Rules.Cosmic.HelicoptersCanCollectHuts = values[31][0] == '1';
            }
        }
        
        private static int IndividualMoveMultiplier(int multiplier, int commonMultiplier)
        {
            return multiplier > 0 ? commonMultiplier / multiplier : 0;
        }

        private void ProcessExtraMovementAdjustments(string[] values)
        {
            var multipliers = values.Select(v => int.Parse(v.Split(',', StringSplitOptions.TrimEntries).Last()))
                .ToList();

            var commonMultiplier = multipliers.Aggregate(1, Utils.LowestCommonMultiple);

            Rules.Cosmic.RoadMovement = IndividualMoveMultiplier(multipliers[0], commonMultiplier);
            Rules.Cosmic.RiverMovement = IndividualMoveMultiplier(multipliers[1], commonMultiplier);
            Rules.Cosmic.AlpineMovement = multipliers.Count > 2
                ? IndividualMoveMultiplier(multipliers[2], commonMultiplier)
                : Rules.Cosmic.RoadMovement;
            Rules.Cosmic.RailroadMovement =
                multipliers.Count > 3 ? IndividualMoveMultiplier(multipliers[3], commonMultiplier) : 0;

            if (Rules.Cosmic.MovementMultiplier == commonMultiplier) return;

            if (Rules.UnitTypes is { Length: >0})
            {
                foreach (var unitType in Rules.UnitTypes)
                {
                    unitType.Move = (unitType.Move / Rules.Cosmic.MovementMultiplier) * commonMultiplier;
                }
            }

            Rules.Cosmic.MovementMultiplier = commonMultiplier;
        }


        private void ProcessTech(IReadOnlyList<string> values)
        {
            var techs = new List<string>();
            for (var i = 0; i < values.Count; i++)
            {
                var parts = values[i].Split(';', StringSplitOptions.TrimEntries);
                techs.Add(parts[0]);
                Rules.AdvanceMappings.Add(parts[1].Split(" ", 2, StringSplitOptions.TrimEntries )[0], i);
            }

            Rules.Advances = techs.Select((line, index) =>
            {
                var text = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return new Advance
                {
                    Index = index,
                    Name = text[0],
                    AIvalue = int.TryParse(text[1], out int val) ? val : 1,
                    Modifier = int.TryParse(text[2], out val) ? val : 0,
                    Prereq1 = Rules.AdvanceMappings.TryGetValue(text[3], out val) ? val : -1,
                    Prereq2 = Rules.AdvanceMappings.TryGetValue(text[4], out val) ? val : -1,
                    Epoch = int.TryParse(text[5], out val) ? val : 0,
                    KnowledgeCategory = int.TryParse(text[6], out val) ? val : 0
                };
            }).ToArray();
        }


        public void ProcessSection(string section, List<string>? contents)
        {
            if (section.StartsWith("TERRAIN"))
            {
                ProcessTerrain(contents);
            }else if (_sectionHandlers.TryGetValue(section, out var handler))
            {
                handler(contents.ToArray());
            }
        }


        private Special MakeSpecial(string source)
        {
            var line = source.Split(',', StringSplitOptions.TrimEntries);
            return new Special
            {
                Name = line[0],
                MoveCost = int.TryParse(line[1], out int val) ? val : 1,
                Defense = int.TryParse(line[2], out val) ? val : 1,
                Food = int.TryParse(line[3], out val) ? val : 0,
                Shields = int.TryParse(line[4], out val) ? val : 0,
                Trade = int.TryParse(line[5], out val) ? val : 0,
            };
        }
    }
}

    