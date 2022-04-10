using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;

namespace Civ2engine
{
    public class AIPlayer : IPlayer
    {
        private Civilization Civilization { get; set; }
        
        private readonly DifficultyType _level;

        public AIPlayer(DifficultyType level)
        {
            _level = level;
        }

        public void WeLoveTheKingCanceled(City city)
        {
        }

        public Tile ActiveTile { get; set; }
        public Unit ActiveUnit { get; set; }

        public void CivilDisorder(City city)
        {
        }

        public void OrderRestored(City city)
        {
        }

        public void WeLoveTheKingStarted(City city)
        {
        }

        public void CantMaintain(City city, Improvement cityImprovement)
        {
        }

        public void SelectNewAdvance(Game game, List<Advance> researchPossibilities)
        {
            Civilization.ReseachingAdvance = game.Random.ChooseFrom(researchPossibilities).Index;
        }

        public void CantProduce(City city, ProductionOrder newItem)
        {
        }

        public void CityProductionComplete(City city)
        {
            
        }

        public IInterfaceCommands UI { get; } = null;
        public IPlayer SetCiv(Civilization civilization)
        {
            this.Civilization = civilization;
            ActiveTile = civilization.Units.FirstOrDefault()?.CurrentLocation ??
                         civilization.Cities.FirstOrDefault()?.Location; 
            return this;
        }
    }
}