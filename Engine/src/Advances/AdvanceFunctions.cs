using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Terrains;

namespace Civ2engine.Advances
{
    public static class AdvanceFunctions
    {
        private static AdvanceResearch[] _researched;

        private static int _MapSizeAdjustment;
        
        public static void SetupTech(this Game game)
        {
            _researched = game.Rules.Advances.OrderBy(a=>a.Index).Select(a=> new AdvanceResearch()).ToArray();
            
            foreach (var civilization in game.AllCivilizations)
            {
                for (var index = 0; index < civilization.Advances.Length; index++)
                {
                    if (civilization.Advances[index])
                    {
                        _researched[index].DiscoveredBy = civilization.Id;
                    }
                }
            }

            _MapSizeAdjustment = game.TotalMapArea / 1000;
            
            ProductionPossibilities.InitializeProductionLists(game.AllCivilizations, game.Rules.ProductionItems);
        }
        
        public static bool HasAdvanceBeenDiscovered(this Game game, int advanceIndex, int byCiv = -1)
        {
            return HasAdvanceBeenDiscovered(advanceIndex) &&
                   (byCiv == -1 || game.AllCivilizations[byCiv].Advances[advanceIndex]);
        }
        
        public static bool HasAdvanceBeenDiscovered(int advanceIndex)
        {
            var research = _researched[advanceIndex];
            return research.Discovered;
        }

        public static void GiveAdvance(this Game game, int advanceIndex, Civilization civilization)
        {
            var research = _researched[advanceIndex];
            if(civilization.Advances[advanceIndex]) return;
            if(civilization.AllowedAdvanceGroups[game.Rules.Advances[advanceIndex].AdvanceGroup] == AdvanceGroupAccess.Prohibited) return;

            var targetCiv = civilization.Id;

            if (!research.Discovered)
            {
                research.DiscoveredBy = targetCiv;
                game.History.AdvanceDiscovered(advanceIndex, targetCiv);
            }

            if (civilization.ReseachingAdvance == advanceIndex)
            {
                civilization.ReseachingAdvance = AdvancesConstants.No;
            }

            foreach (var effect in game.Rules.Advances[advanceIndex].Effects)
            {
                civilization.GlobalEffects[effect.Key] += effect.Value;
            }

            foreach (var improvement in game.TerrainImprovements.Values)
            {
                for (var level = 0; level < improvement.Levels.Count; level++)
                {
                    if (improvement.Levels[level].RequiredTech != advanceIndex) continue;
                    
                    game.Players[civilization.Id].NotifyImprovementEnabled(improvement, level);

                    if (!improvement.AllCitys) continue;
                    var locations = civilization.Cities
                        .Select(c => c.Location)
                        .Select(tile => new
                        {
                            tile,
                            terrain = improvement.AllowedTerrains[tile.Z]
                                .FirstOrDefault(t => t.TerrainType == (int)tile.Type)
                        })
                        .Where(t => t.terrain is not null)
                        .Select(loc =>
                        {
                            loc.tile.AddImprovement(improvement, loc.terrain, level,
                                game.Rules.Terrains[loc.tile.Z]);
                            return loc.tile;
                        }).ToList();
                    game.TriggerMapEvent(MapEventType.UpdateMap, improvement.HasMultiTile ? locations.Concat(locations.SelectMany(l=> l.Neighbours())).ToList() : locations );
                }
            }

            civilization.Advances[advanceIndex] = true;
            ProductionPossibilities.AddItems(targetCiv,
                game.Rules.ProductionItems.Where(i => i.RequiredTech == advanceIndex && i.CanBuild(civilization)));
            ProductionPossibilities.RemoveItems(targetCiv, game.Rules.ProductionItems.Where(o => o.ExpiresTech == advanceIndex));
        }

        public static int TotalAdvances(this Game game, int targetCiv)
        {
            return game.AllCivilizations[targetCiv].Advances.Count(a => a);
        }

        /// <summary>
        ///  I'm not sure if this formula is correct I've just grabed if from https://forums.civfanatics.com/threads/tips-tricks-for-new-players.96725/
        /// </summary>
        /// <param name="game"></param>
        /// <param name="civ"></param>
        /// <returns></returns>
        public static int CalculateScienceCost(Game game, Civilization civ)
        {
            if (civ.ReseachingAdvance < 0) return -1;
            var techParadigm = game.Rules.Cosmic.TechParadigm;
            var ourAdvances = TotalAdvances(game, civ.Id);
            var keyCivAdvances = TotalAdvances(game, civ.PowerRank);
            var techLead = (ourAdvances - keyCivAdvances) / 3;
            var baseCost = techParadigm + techLead;

            if (ourAdvances > 20)
            {
                baseCost += _MapSizeAdjustment;
            }

            return baseCost * ourAdvances;

        }

        public static bool HasTech(Civilization civ, int tech)
        {
            if (tech < 0)
            {
                return tech == AdvancesConstants.Nil;
            }

            return civ.Advances[tech];
        }

        public static List<Advance> CalculateAvailableResearch(Game game, Civilization activeCiv)
        {
            var allAvailable = game.Rules.Advances.Where(a =>
                activeCiv.AllowedAdvanceGroups[a.AdvanceGroup] == AdvanceGroupAccess.CanResearch &&
                HasTech(activeCiv, a.Prereq1) && HasTech(activeCiv, a.Prereq1) && !activeCiv.Advances[a.Index]).ToList();
            
            //TODO: cull list based on difficulty
            return allAvailable.ToList();
        }
    }
}