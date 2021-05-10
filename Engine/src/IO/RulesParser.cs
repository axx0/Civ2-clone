using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine
{
    public class RulesParser : IFileHandler
    {
        private Rules Rules { get; set; }

        private readonly Dictionary<string, Action<string[]>> _sectionHandlers = new();

        private RulesParser()
        {
            _sectionHandlers.Add("COSMIC", ProcessCosmicRules);
            _sectionHandlers.Add("CIVILIZE", ProcessTech);
            _sectionHandlers.Add("IMPROVE", ProcessImprovements);
            // ReSharper disable once StringLiteralTypo
            _sectionHandlers.Add("ENDWONDER", ProcessEndWonders);
            _sectionHandlers.Add("UNITS", ProcessUnits);
            _sectionHandlers.Add("TERRAIN", ProcessTerrain);
            _sectionHandlers.Add("GOVERNMENTS", ProcessGovernments);
            _sectionHandlers.Add("LEADERS", ProcessLeaders);
            _sectionHandlers.Add("ORDERS", ProcessOrders);
            _sectionHandlers.Add("CARAVAN", ProcessGoods);
            _sectionHandlers.Add("CIVILIZE2", ProcessAdvanceGroups);
            _sectionHandlers.Add("DIFFICULTY", strings => Rules.Difficulty = strings.ToArray() );
            _sectionHandlers.Add("@ATTITUDES", strings => Rules.Attitude = strings.ToArray());
        }

        private void ProcessAdvanceGroups(string[] values)
        {
            var limit = values.Length < Rules.Advances.Length ? values.Length : Rules.Advances.Length;
            for (var i = 0; i < limit ; i++)
            {
                Rules.Advances[i].AdvanceGroup = int.Parse(values[i].Split(',', 2, StringSplitOptions.TrimEntries)[0]);
            }
        }

        private void ProcessOrders(string[] values)
        {
            Rules.Orders = values.Select((line, id) =>
            {
                var parts = line.Split(',', StringSplitOptions.TrimEntries);
                return new Order
                {
                    Id = id,
                    Name = parts[0],
                    Key = parts[1]
                };
            }).ToArray();
        }

        private void ProcessGoods(string[] values)
        {
            Rules.CaravanCommoditie =
                values.Select((value => value.Split(',', StringSplitOptions.TrimEntries)[0])).ToArray();
        }

        private void ProcessLeaders(string[] values)
        {
            Rules.Leaders = values.Select((value) =>
            {
                var line = value.Split(',', StringSplitOptions.TrimEntries);
                var titles = new List<LeaderTitle>(
                );
                for(var i = 12; i < line.Length;i+=3)
                {
                    titles.Add(new LeaderTitle
                    {
                        Gov = int.Parse(line[i-2]),
                        TitleMale = line[i-1],
                        TitleFemale = line[i],
                    });
                }
                return new LeaderDefaults()
                {
                    NameMale = line[0],
                    NameFemale = line[1],
                    Female = int.Parse(line[2]),
                    Color = int.Parse(line[3]),
                    CityStyle = int.Parse(line[4]),
                    Plural = line[5],
                    Adjective = line[6],
                    Attack = int.Parse(line[7]),
                    Expand = int.Parse(line[8]),
                    Civilize = int.Parse(line[9]),
                    Titles = titles.ToArray()
                };
            }).ToArray();
        }

        private void ProcessGovernments(string[] values)
        {
            Rules.Governments = values.Select((value =>
            {
                var line = value.Split(',', StringSplitOptions.TrimEntries);
                return new Government
                {
                    Name = line[0],
                    TitleMale = line[1],
                    TitleFemale = line[2]
                };
            })).ToArray();
        }

        private void ProcessTerrain(string[] values)
        {
            var terrains = new List<string>();
            var bonus = new List<string>();
            var mappings = new Dictionary<string, int> {{"yes", -1}, {"no", -2}};
            foreach (var t in values)
            {
                var parts = t.Split(';', StringSplitOptions.TrimEntries);
                if (parts.Length == 1)
                {
                    bonus.Add(parts[0]);
                }
                else
                {
                    mappings.Add(parts[1], terrains.Count);
                    terrains.Add(parts[0]);
                }
            }

            Rules.Terrains = terrains.Select((value, type) =>
            {
                var line = value.Split(',', StringSplitOptions.TrimEntries);
                return new Terrain
                {
                    Type = (TerrainType) type,
                    Name = line[0],
                    MoveCost = int.Parse(line[1]),
                    Defense = int.Parse(line[2]),
                    Food = int.Parse(line[3]),
                    Shields = int.Parse(line[4]),
                    Trade = int.Parse(line[5]),
                    CanIrrigate = mappings[line[6]],
                    IrrigationBonus = int.Parse(line[7]),
                    TurnsToIrrigate = int.Parse(line[8]),
                    MinGovrnLevelAItoPerformIrrigation = (GovernmentType) int.Parse(line[9]),
                    CanMine = mappings[line[10]],
                    MiningBonus = int.Parse(line[11]),
                    TurnsToMine = int.Parse(line[12]),
                    MinGovrnLevelAItoPerformMining = (GovernmentType) int.Parse(line[13]),
                    Transform = mappings[line[14]],
                    Specials = new[]
                    {
                        MakeSpecial(bonus[type]), MakeSpecial(bonus[type + terrains.Count])
                    }
                };
            }).ToArray();
        }

        private void ProcessUnits(string[] values)
        {
            Rules.UnitTypes = values.Select((line, type) =>
            {
                var text = line.Split(',', StringSplitOptions.TrimEntries);
                return new UnitDefinition
                {
                    Type = (UnitType) type,
                    Name = text[0],
                    Until = Rules.AdvanceMappings[text[1]],
                    Domain = (UnitGAS) int.Parse(text[2]),
                    Move = Rules.Cosmic.RoadMultiplier * int.Parse(text[3].Replace(".", string.Empty)),
                    Range = int.Parse(text[4]),
                    Attack = int.Parse(text[5].Replace("a", string.Empty)),
                    Defense = int.Parse(text[6].Replace("d", string.Empty)),
                    Hitp = 10 * int.Parse(text[7].Replace("h", string.Empty)),
                    Firepwr = int.Parse(text[8].Replace("f", string.Empty)),
                    Cost = int.Parse(text[9]),
                    Hold = int.Parse(text[10]),
                    AIrole = int.Parse(text[11]),
                    Prereq = Rules.AdvanceMappings[text[12]],
                    Flags = text[13]
                };
            }).ToArray();
        }


        private void ProcessEndWonders(string[] values)
        {
            var firstWonderIndex = Rules.Improvements.First(i => i.Type == ImprovementType.Pyramids).Id;
            for (var i = 0; i < values.Length; i++)
            {
                if (!values[i].StartsWith("nil"))
                {
                    Rules.Improvements[firstWonderIndex + i].ExpiresAt =
                        Rules.AdvanceMappings[values[i].Split(',', 2)[0]];
                }
            }
        }

        private void ProcessImprovements(string[] values)
        {
            Rules.Improvements = values.Select((value, type) =>
            {
                var parts = value.Split(',', StringSplitOptions.TrimEntries);
                return new Improvement
                {
                    Type = (ImprovementType) type,
                    Name = parts[0],
                    Cost = int.Parse(parts[1]),
                    Upkeep = int.Parse(parts[2]),
                    Prerequisite = Rules.AdvanceMappings[parts[3]]
                };
            }).ToArray();
        }


        private void ProcessCosmicRules(string[] values)
        {
            var type = typeof(CosmicRules);
            var props = type.GetProperties();
            var cosmic = this.Rules.Cosmic;
            for (var i = 0; i < values.Length; i++)
            {
                var value = values[i].Split(";", 2, StringSplitOptions.TrimEntries)[0];

                if (int.TryParse(value, out var result))
                {
                    props[i].SetValue(cosmic, result);
                }
            }
        }


        private void ProcessTech(IReadOnlyList<string> values)
        {
            var techs = new List<string>();
            for (var i = 0; i < values.Count; i++)
            {
                var parts = values[i].Split(';', StringSplitOptions.TrimEntries);
                techs.Add(parts[0]);
                Rules.AdvanceMappings.Add(parts[1], i);
            }

            Rules.Advances = techs.Select(line =>
            {
                var text = line.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                return new Advance
                {
                    Name = text[0],
                    AIvalue = int.Parse(text[1]),
                    Modifier = int.Parse(text[2]),
                    Prereq1 = Rules.AdvanceMappings[text[3]],
                    Prereq2 = Rules.AdvanceMappings[text[4]],
                    Epoch = (EpochType) int.Parse(text[5]),
                    KnowledgeCategory = (KnowledgeType) int.Parse((text[6]))
                };
            }).ToArray();
        }

        public static Rules ParseRules(string path)
        {
            var rules = new Rules();
            var filePath = Utils.GetFilePath("RULES.txt", path);
            TextFileParser.ParseFile(filePath, new RulesParser {Rules = rules});
            return rules;
        }

        public void ProcessSection(string section, List<string> contents)
        {
            if (_sectionHandlers.ContainsKey(section))
            {
                _sectionHandlers[section](contents.ToArray());
            }
        }


        private Special MakeSpecial(string source)
        {
            var line = source.Split(',', StringSplitOptions.TrimEntries);
            return new Special
            {
                Name = line[0],
                MoveCost = int.Parse(line[1]),
                Defense = int.Parse(line[2]),
                Food = int.Parse(line[3]),
                Shields = int.Parse(line[4]),
                Trade = int.Parse(line[5]),
            };
        }
    }

    public struct Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Key { get; set; }
    }
}

    