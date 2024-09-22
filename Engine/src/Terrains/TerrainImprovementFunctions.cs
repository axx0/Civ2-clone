using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model.Core;

namespace Civ2engine.Terrains
{
    public static class TerrainImprovementFunctions
    {
        
        public static string LabelFrom(ImprovementLevel improvementLevel)
        {
            if (string.IsNullOrWhiteSpace(improvementLevel.BuildLabel))
                return $"{Labels.For(LabelIndex.Build)} {improvementLevel.Name}";

            if (!improvementLevel.BuildLabel.StartsWith("~") || !Enum.TryParse(typeof(LabelIndex),
                    improvementLevel.BuildLabel.Substring(1), out var index))
                return improvementLevel.BuildLabel;

            var labelIndex = (LabelIndex)index;
            return Labels.For(labelIndex);
        }
        
        public static UnitActionAssessment CanImprovementBeBuiltHere(Tile location,
            TerrainImprovement improvement, Civilization civilization)
        {
            if (location.CityHere != null)
            {
                return new UnitActionAssessment(false, errorPopup: "CANTDO");
            }

            var alreadyBuilt = location.Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
           
            
            if (alreadyBuilt != null)
            {
                if (improvement.Negative)
                {
                    return new UnitActionAssessment(true,
                        commandTitle: improvement.Levels[alreadyBuilt.Level].BuildLabel);
                }
                if (alreadyBuilt.Level + 1 >= improvement.Levels.Count)
                {
                    return new UnitActionAssessment(false, errorPopup: improvement.MaxLevelReachedMessage);
                }

                var nextLevel = improvement.Levels[alreadyBuilt.Level + 1];
                if (nextLevel.RequiredTech != AdvancesConstants.Nil &&
                    !AdvanceFunctions.HasTech(civilization, nextLevel.RequiredTech))
                {
                    return new UnitActionAssessment(false, nextLevel.BuildLabel, errorPopup: nextLevel.MissingRequiredTechMessage);
                }

                return new UnitActionAssessment(true, LabelFrom(nextLevel));
            }

            if (improvement.Negative)
            {
                return new UnitActionAssessment(false, errorPopup: improvement.MaxLevelReachedMessage);
            }

            if (improvement.Levels[0].RequiredTech != AdvancesConstants.Nil &&
                !AdvanceFunctions.HasTech(civilization, improvement.Levels[0].RequiredTech))
            {
                return new UnitActionAssessment(false,
                    errorPopup: improvement.Levels[0].MissingRequiredTechMessage);
            }

            var allowedTerrain = improvement.AllowedTerrains[location.Z]
                .FirstOrDefault(t => t.TerrainType == (int)location.Type);
            if (allowedTerrain == null)
            {
                return new UnitActionAssessment(false, errorPopup: "CANTIMPROVE");
            }

            if (location.River)
            {
                var riverAllowance = improvement.AllowedTerrains[location.Z]
                    .FirstOrDefault(t => t.TerrainType == TerrainConstants.River);
                if (riverAllowance != null)
                {
                    if (riverAllowance.RequiredTech != AdvancesConstants.Nil &&
                        !AdvanceFunctions.HasTech(civilization, riverAllowance.RequiredTech))
                    {
                        return new UnitActionAssessment(false, errorPopup: riverAllowance.MissingRequiredTechMessage);
                    }
                }
            }

            if (allowedTerrain.RequiredTech != AdvancesConstants.Nil &&
                !AdvanceFunctions.HasTech(civilization, allowedTerrain.RequiredTech))
            {
                return new UnitActionAssessment(false, errorPopup: allowedTerrain.MissingRequiredTechMessage);
            }

            var transform = allowedTerrain.Effects?.FirstOrDefault(e => e.Target == ImprovementConstants.Transform);
            if (transform != null)
            {
                return new UnitActionAssessment(true,transform.Text);
            }

            if (improvement.AdjacencyRules is { Count: > 0 })
            {
                if (NoAdjacentValid(location,improvement))
                {
                    return new UnitActionAssessment(false, errorPopup: improvement.NoAdjacencyMessage);
                }
            }
            return new UnitActionAssessment(true);
        }
        
        public static Dictionary<int, TerrainImprovement> GetStandardImprovements(Rules rules)
        {
            return new List<TerrainImprovement>
            {
                new()
                {
                    Id = ImprovementTypes.Irrigation,
                    Name = Labels.For(LabelIndex.Irrigation),
                    Levels = new List<ImprovementLevel>
                    {
                        new()
                        {
                            Name = Labels.For(LabelIndex.Irrigation),
                            RequiredTech = AdvancesConstants.Nil,
                            BuildLabel = Labels.For(LabelIndex.BuildIrrigation)
                        },
                        new()
                        {
                            Name = Labels.For(LabelIndex.Farmland),
                            RequiredTech = (int)AdvanceType.Refrigerat,
                            BuildLabel = Labels.For(LabelIndex.ImproveFarmland),
                            EnabledMessage = "NEWFARMLAND",
                            MissingRequiredTechMessage = "FARMLAND"
                        }
                    },
                    AllCitys = true,
                    AllowedTerrains = rules.Terrains.Select(terrains =>
                        terrains
                            .Where(t => t.CanIrrigate != -2)
                            .Select(t => new AllowedTerrain
                            {
                                TerrainType = (int)t.Type, BuildTime = t.TurnsToIrrigate,
                                AiGovernmentBuild = (int)t.MinGovrnLevelAItoPerformIrrigation,
                                Effects = GetStandardIrrigationEffects(t, terrains)
                            })
                            .ToList()
                    ).ToList(),
                    Shortcut = "I",
                    AdjacencyRules = new List<int> { TerrainConstants.River, TerrainConstants.Existing, (int)TerrainType.Ocean},
                    NoAdjacencyMessage = "NOWATER",
                    MaxLevelReachedMessage = "ALREADYFARMLAND", 
                    ExclusiveGroup = 2,
                    Layer = 1
                },
                new() {
                    Id = ImprovementTypes.Mining,
                    Name = Labels.For(LabelIndex.Mining),
                    Levels = new List<ImprovementLevel>
                    {
                        new ()
                        {
                            Name = Labels.For(LabelIndex.Mining),
                            BuildLabel = Labels.For(LabelIndex.BuildMines),
                            RequiredTech = AdvancesConstants.Nil,
                        }
                    },
                    AllowedTerrains = rules.Terrains.Select(terrains =>
                        terrains
                            .Where(t => t.CanMine != -2)
                            .Select(t => new AllowedTerrain
                            {
                                TerrainType = (int)t.Type, BuildTime = t.TurnsToMine,
                                AiGovernmentBuild = (int)t.MinGovrnLevelAItoPerformMining,
                                Effects = GetStandardMiningEffects(t, terrains)
                            })
                            .ToList()
                    ).ToList(),
                    Shortcut = "m",
                    MaxLevelReachedMessage = "ALREADYMINING", 
                    ExclusiveGroup = 2,
                    Layer = 15
                }
                ,
                new()
                {
                    Id = ImprovementTypes.Road,
                    Name = Labels.For(LabelIndex.Road),
                    Levels = new List<ImprovementLevel>
                    {
                        new()
                        {
                            Name = Labels.For(LabelIndex.Road),
                            RequiredTech = AdvancesConstants.Nil,
                            BuildLabel = Labels.For(LabelIndex.BuildRoad),
                            Effects = new List<TerrainImprovementAction>
                            {
                                new()
                                {
                                    Target = ImprovementConstants.Movement,
                                    Action = ImprovementActions.Set,
                                    Value = rules.Cosmic.RoadMovement,
                                }
                            }
                        },
                        new()
                        {
                            Name = Labels.For(LabelIndex.Railroads),
                            RequiredTech = (int)AdvanceType.Railroad,
                            MissingRequiredTechMessage = "RAILROADS",
                            EnabledMessage = "NEWRAILROAD",
                            BuildLabel = Labels.For(LabelIndex.BuildRRTrack),
                            Effects = new List<TerrainImprovementAction>
                            {
                                new()
                                {
                                    Target = ImprovementConstants.Movement,
                                    Action = ImprovementActions.Set,
                                    Value = rules.Cosmic.RailroadMovement,
                                }
                            },
                            BuildCostMultiplier = 50
                        }
                    },
                    AllCitys = true,
                    Shortcut = "R",
                    AllowedTerrains = rules.Terrains.Select(terrains =>
                        terrains
                            .Where(t => t.Type != TerrainType.Ocean && !t.Impassable)
                            .Select(t => new AllowedTerrain
                                { BuildTime = t.MoveCost * 2, TerrainType = (int)t.Type, Effects = GetRoadEffects(t) })
                            .Concat(new[]
                            {
                                new AllowedTerrain
                                {
                                    TerrainType = TerrainConstants.River, RequiredTech = (int)AdvanceType.BridgeBuild, MissingRequiredTechMessage = "BRIDGES", BuildTime = 1
                                }
                            }).ToList()
                    ).ToList(),
                    MaxLevelReachedMessage = "ALREADYROAD",
                    Layer = 5,
                    HasMultiTile = true
                },
                new ()
                {
                    Id = ImprovementTypes.Pollution,
                    Layer = 10,
                    Shortcut = "P",
                    Name = Labels.For(LabelIndex.Pollution),
                    MaxLevelReachedMessage = "NOPOLLUTION",
                    MenuGroup = 1,
                    Levels = new List<ImprovementLevel>
                    {
                        new()
                        {
                            Name = Labels.For(LabelIndex.Pollution),
                            BuildLabel = Labels.For(LabelIndex.Clear) + " " + Labels.For(LabelIndex.Pollution),
                            RequiredTech = AdvancesConstants.Nil,
                            Effects = new List<TerrainImprovementAction>
                            {
                                new ()
                                {
                                    Target = ImprovementConstants.Food,
                                    Action = ImprovementActions.Multiply,
                                    Value = -50
                                },new ()
                                {
                                    Target = ImprovementConstants.Trade,
                                    Action = ImprovementActions.Multiply,
                                    Value = -50
                                },new ()
                                {
                                    Target = ImprovementConstants.Shields,
                                    Action = ImprovementActions.Multiply,
                                    Value = -50
                                }
                            }
                        }
                    },
                    Negative = true,
                    AllowedTerrains =  rules.Terrains.Select(terrains => terrains
                        .Where(t => t.Type != TerrainType.Ocean && !t.Impassable)
                        .Select(t => new AllowedTerrain
                            { BuildTime = t.MoveCost * 2, TerrainType = (int)t.Type }).ToList()).ToList()
                },
                new()
                {
                    Id = ImprovementTypes.Fortress,
                    Layer = 15,
                    Foreground = true,
                    Levels = new ImprovementLevel[]
                    {
                        new()
                        {
                            Name = Labels.For(LabelIndex.Fortress),
                            Effects = new List<TerrainImprovementAction>
                            {
                                new()
                                {
                                    Target = ImprovementConstants.GroundDefence,
                                    Action = ImprovementActions.Multiply,
                                    Value = 100
                                },
                                new()
                                {
                                    Target = ImprovementConstants.NoStackElimination
                                }
                            },
                            RequiredTech = (int)AdvanceType.Construct,
                            MissingRequiredTechMessage = "CONSTRUCTION",
                            EnabledMessage = "NEWFORTRESS"
                        }
                    },
                    Name = Labels.For(LabelIndex.Fortress),
                    Shortcut = "F",
                    AllowedTerrains = rules.Terrains.Select(terrains => terrains
                        .Where(t => t.Type != TerrainType.Ocean && !t.Impassable)
                        .Select(t => new AllowedTerrain
                            { BuildTime = t.MoveCost * 2, TerrainType = (int)t.Type }).ToList()).ToList(),
                    ExclusiveGroup = ImprovementTypes.DefenceGroup,
                    MaxLevelReachedMessage = "ALREADYFORT"
                },
                new()
                {
                    Id = ImprovementTypes.Airbase,
                    Layer = 20,
                    Name = Labels.For(LabelIndex.Airbase),
                    Levels = new ImprovementLevel[]
                    {
                        new()
                        {
                            Name = Labels.For(LabelIndex.Airbase),
                            Effects = new List<TerrainImprovementAction>
                            {
                                new()
                                {
                                    Target = ImprovementConstants.Airbase,
                                },
                                new()
                                {
                                    Target = ImprovementConstants.NoStackElimination
                                },
                                new()
                                {
                                    Target = ImprovementConstants.PreventPollutionInSquare
                                }
                            },
                            RequiredTech = (int)AdvanceType.Radio,
                            MissingRequiredTechMessage = "RADIO",
                            EnabledMessage = "NEWAIRLIFT"
                        }
                    },
                    Shortcut = "E",
                    AllowedTerrains = rules.Terrains.Select(terrains => terrains
                        .Where(t => t.Type != TerrainType.Ocean && !t.Impassable)
                        .Select(t => new AllowedTerrain
                            { BuildTime = t.MoveCost * 3, TerrainType = (int)t.Type }).ToList()).ToList(),
                    ExclusiveGroup = ImprovementTypes.DefenceGroup,
                    MaxLevelReachedMessage = "ALREADYAIR",
                    HideUnits = (int)UnitGas.Air
                }
            }.ToDictionary(k=>k.Id);
        }

        private static List<TerrainImprovementAction> GetStandardMiningEffects(Terrain terrain, Terrain[] terrains)
        {
            if (terrain.CanMine == -1)
            {
                return new List<TerrainImprovementAction>
                {
                    new()
                    {
                        Target = ImprovementConstants.Shields, Action = ImprovementActions.Add, Value = terrain.MiningBonus
                    }
                };
            }

            return new List<TerrainImprovementAction>
            {
                new()
                {
                    Target = ImprovementConstants.Transform, Value = terrain.CanMine, Text = Labels.For(LabelIndex.ChangetoSTRING0m, terrains[terrain.CanMine].Name)
                }
            };
        }

        private static List<TerrainImprovementAction> GetRoadEffects(Terrain terrain)
        {
            if (terrain.Type == TerrainType.Desert || terrain.Type == TerrainType.Plains ||
                terrain.Type == TerrainType.Grassland)
            {

                return new List<TerrainImprovementAction>
                {
                    new() { Target = ImprovementConstants.Trade, Action = ImprovementActions.Add, Value = 1 }
                };
            }

            return null;
        }

        private static List<TerrainImprovementAction> GetStandardIrrigationEffects(Terrain terrain, Terrain[] terrains)
        {
            if (terrain.CanIrrigate == -1)
            {
                return new List<TerrainImprovementAction>
                {
                    new()
                    {
                        Target = ImprovementConstants.Food, Action = ImprovementActions.Add, Value = terrain.IrrigationBonus
                    }
                };
            }

            return new List<TerrainImprovementAction>
            {
                new()
                {
                    Target = ImprovementConstants.Transform, Value = terrain.CanIrrigate, Text = Labels.For(LabelIndex.ChangetoSTRING0i, terrains[terrain.CanIrrigate].Name)
                }
            };
        }
        
        private static bool NoAdjacentValid(Tile activeTile, TerrainImprovement improvement)
        {
            foreach (var improvementAdjacencyRule in improvement.AdjacencyRules)
            {
                switch (improvementAdjacencyRule)
                {
                    case TerrainConstants.River:
                    {
                        if (activeTile.River || activeTile.Map.Neighbours(activeTile).Any(t => t.River))
                        {
                            return false;
                        }

                        break;
                    }
                    case TerrainConstants.Existing:
                    {
                        if (activeTile.Map.Neighbours(activeTile).Any(t => t.CityHere is null && t.Improvements.Any(i=>i.Improvement == improvement.Id)))
                        {
                            return false;
                        }
                        break;
                    }
                    default:
                    {
                        if ((int)activeTile.Type == improvementAdjacencyRule || activeTile.Map.Neighbours(activeTile).Any(t => improvementAdjacencyRule == (int)t.Type))
                        {
                            return false;
                        }
                        break;
                    }
                }
            }

            return true;
        }
    }
}