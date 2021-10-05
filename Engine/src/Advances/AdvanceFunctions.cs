using System.Linq;
using Civ2engine.Production;

namespace Civ2engine.Advances
{
    public static class AdvanceFunctions
    {
        private static AdvanceResearch[] _researched; 
        
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
            ProductionPossibilities.InitializeProductionLists(game.AllCivilizations, game.Rules.ProductionItems);
        }
        
        public static bool HasAdvanceBeenDiscovered(this Game game, int advanceIndex, int byCiv = -1)
        {
            var research = _researched[advanceIndex];
            if (byCiv > -1)
            {
                return research.Discovered && game.AllCivilizations[byCiv].Advances[advanceIndex];
            }

            return research.Discovered;
        }

        public static void GiveAdvance(this Game game, int advanceIndex, int targetCiv)
        {
            var research = _researched[advanceIndex];
            if(game.AllCivilizations[targetCiv].Advances[advanceIndex]) return;
            
            //TODO: here we'd look for a lua script to check for effeccts
            
            //TODO: check for default effect

            if (!research.Discovered)
            {
                research.DiscoveredBy = targetCiv;
                game.History.AdvanceDiscovered(advanceIndex, targetCiv);
            }

            game.AllCivilizations[targetCiv].Advances[advanceIndex] = true;
            ProductionPossibilities.AddItems(targetCiv,
                game.Rules.ProductionItems.Where(i => i.RequiredTech == advanceIndex && i.CanBuild(targetCiv)));
            ProductionPossibilities.RemoveItems(targetCiv, game.Rules.ProductionItems.Where(o => o.ExpiresTech == advanceIndex));
        }

        public static int TotalAdvances(this Game game, int targetCiv)
        {
            return game.AllCivilizations[targetCiv].Advances.Count(a => a);
        }

        public static int CalculateScienceCost(Civilization civ)
        {
            throw new System.NotImplementedException();
        }
    }
}