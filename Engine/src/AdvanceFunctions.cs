using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Terrains;
using Model.Constants;
using Model.Core.Advances;

namespace Civ2engine.Advances
{
    public static class AdvanceFunctions
    {
        private static AdvanceResearch[] _researched;

        private static int _mapSizeAdjustment;
        
        public static void SetupTech(this Game game)
        {
            _researched = game.Rules.Advances.OrderBy(a=>a.Index).Select(a=> new AdvanceResearch()).ToArray();
            
            _mapSizeAdjustment = game.TotalMapArea / 1000;

            foreach (var civilization in game.AllCivilizations)
            {
                SetEpoch(game, civilization);

                for (var advanceIndex = 0; advanceIndex < game.Rules.Advances.Length; advanceIndex++)
                {
                    if (civilization.Advances.Length <= advanceIndex || !civilization.Advances[advanceIndex]) continue;

                    foreach (var effect in game.Rules.Advances[advanceIndex].Effects
                                 .Where(effect => effect.Key != Effects.EpochTech))
                    {
                        civilization.GlobalEffects[effect.Key] = civilization.GlobalEffects.GetValueOrDefault(effect.Key) + effect.Value;
                    }
                }
            }
            
            ProductionPossibilities.InitializeProductionLists(game.AllCivilizations, ProductionOrder.GetAll( game.Rules));
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

        public static void RemoveAdvance(this Game game, int advanceIndex, Civilization civilization)
        {
            if (!civilization.Advances[advanceIndex]) return;

            foreach (var effect in game.Rules.Advances[advanceIndex].Effects)
            {
                if (effect.Key == Effects.EpochTech)
                {
                    SetEpoch(game, civilization);
                }
                else
                {
                    civilization.GlobalEffects[effect.Key] = civilization.GlobalEffects.GetValueOrDefault(effect.Key) - effect.Value;
                }
            }

            civilization.Advances[advanceIndex] = false;
            var allOrders = ProductionOrder.GetAll(game.Rules);
            ProductionPossibilities.RemoveItems(civilization.Id,
                allOrders.Where(i => i.RequiredTech == advanceIndex));
            ProductionPossibilities.AddItems(civilization.Id,
                allOrders.Where(o => o.ExpiresTech == advanceIndex && o.CanBuild(civilization)));
        }

        private static void SetEpoch(Game game, Civilization civilization)
        {
            civilization.Epoch = game.Rules.Advances.Where(a => a.Effects.ContainsKey(Effects.EpochTech))
                .GroupBy(a => a.Effects[Effects.EpochTech])
                .Where(techs => techs.All(t => t.Index < civilization.Advances.Length && civilization.Advances[t.Index])).Select(t => t.Key)
                .DefaultIfEmpty(0).Max();
        }

        public static void GiveAdvance(this Game game, int advanceIndex, Civilization civilization)
        {
            var research = _researched[advanceIndex];
            if(civilization.Advances[advanceIndex]) return;
            if(civilization.AllowedAdvanceGroups[game.Rules.Advances[advanceIndex].AdvanceGroup] == AdvanceGroupAccess.Prohibited) return;

            ApplyCivAdvance(game, advanceIndex, civilization, research, civilization.Id);
        }

        private static void ApplyCivAdvance(Game game, int advanceIndex, Civilization civilization, AdvanceResearch research,
            int targetCiv)
        {
            if (!research.Discovered)
            {
                research.DiscoveredBy = targetCiv;
                game.History.AdvanceDiscovered(advanceIndex, civilization);
            }

            if (civilization.ReseachingAdvance == advanceIndex)
            {
                civilization.ReseachingAdvance = AdvancesConstants.No;
            }

            foreach (var effect in game.Rules.Advances[advanceIndex].Effects)
            {
                if (effect.Key == Effects.EpochTech)
                {
                    if (civilization.Epoch < effect.Value)
                    {
                        var hasAllEpochTechs = game.Rules.Advances.Where(a =>
                            a.Effects.ContainsKey(Effects.EpochTech) && a.Effects[Effects.EpochTech] == effect.Value).All(a=>a.Index == advanceIndex || civilization.Advances[a.Index]);
                        if (hasAllEpochTechs)
                        {
                            civilization.Epoch = effect.Value;
                        }
                    } 
                }
                else
                {
                    civilization.GlobalEffects[effect.Key] = civilization.GlobalEffects.GetValueOrDefault(effect.Key) + effect.Value;
                }
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
                                game.Rules.Terrains[loc.tile.Z], loc.tile.GetCivsVisibleTo(game));
                            return loc.tile;
                        }).ToList();
                    game.TriggerMapEvent(MapEventType.UpdateMap, improvement.HasMultiTile ? locations.Concat(locations.SelectMany(l=> l.Neighbours())).ToList() : locations );
                }
            }

            if (civilization.Advances.Length < advanceIndex)
            {
                var advances = new bool[game.Rules.Advances.Length];
                Array.Copy(civilization.Advances, advances.Length, advances, 0, advances.Length);
                civilization.Advances = advances;
            }
            civilization.Advances[advanceIndex] = true;

            var orders = ProductionOrder.GetAll(game.Rules);
            ProductionPossibilities.AddItems(targetCiv,
                orders.Where(i => i.RequiredTech == advanceIndex && i.CanBuild(civilization)));
            ProductionPossibilities.RemoveItems(targetCiv, orders.Where(o => o.ExpiresTech == advanceIndex));
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
                baseCost += _mapSizeAdjustment;
            }

            return baseCost * (ourAdvances +1);

        }

        public static bool HasTech(Civilization civ, int tech)
        {
            if (tech < 0)
            {
                return tech == AdvancesConstants.Nil;
            }

            return tech < civ.Advances.Length && civ.Advances[tech];
        }

        public static List<Advance> CalculateAvailableResearch(Game game, Civilization activeCiv)
        {
            var allAvailable = game.Rules.Advances.Where(a =>
                activeCiv.AllowedAdvanceGroups[a.AdvanceGroup] == AdvanceGroupAccess.CanResearch &&
                HasTech(activeCiv, a.Prereq1) && HasTech(activeCiv, a.Prereq1) && (activeCiv.Advances.Length < a.Index || !activeCiv.Advances[a.Index])).ToList();
            
            //TODO: cull list based on difficulty
            return allAvailable.ToList();
        }
    }
}