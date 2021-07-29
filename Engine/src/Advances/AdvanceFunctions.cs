using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;

namespace Civ2engine.Advances
{
    public static class AdvanceFunctions
    {
        private static AdvanceResearch[] _researched; 
        
        public static void SetupTech(this Game game)
        {
            var totalCivs = game.AllCivilizations.Count;
            _researched = game.Rules.Advances.OrderBy(a=>a.Index).Select(a=> new AdvanceResearch(totalCivs)).ToArray();
        }
        
        public static bool HasAdvanceBeenDiscovered(this Game game, int advanceIndex, int byCiv = -1)
        {
            var research = _researched[advanceIndex];
            if (byCiv > -1)
            {
                return research.Discovered && research.Civs[byCiv];
            }

            return research.Discovered;
        }

        public static void GiveAdvance(this Game game, int advanceIndex, int targetCiv)
        {
            var research = _researched[advanceIndex];
            if(research.Civs[targetCiv]) return;
            
            //TODO: here we'd look for a lua script to check for effeccts
            
            //TODO: check for default effect

            if (!research.Discovered)
            {
                research.DiscoveredBy = targetCiv;
                game.History.AdvanceDiscovered(advanceIndex, targetCiv);
            }

            research.Civs[targetCiv] = true;
        }

        public static int TotalAdvances(this Game game, int targetCiv)
        {
            return _researched.Count(r => r.Civs[targetCiv]);
        }
    }

    internal class AdvanceResearch
    {
        public AdvanceResearch(int totalCivs)
        {
            Civs = new bool[totalCivs];
        }

        public bool Discovered => DiscoveredBy != -1;
        public bool[] Civs { get; }

        public int DiscoveredBy { get; set; } = -1;
    }
}