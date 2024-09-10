using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Advances;

namespace Civ2engine
{
    public class Player : IPlayer
    {
        public Civilization Civilization { get; set; }
        
        private readonly int _difficultyLevel;

        public Player(int difficultyLevel, Civilization civilization)
        {
            _difficultyLevel = difficultyLevel;
            Civilization = civilization;
            ActiveTile = civilization.Units.FirstOrDefault()?.CurrentLocation ??
                         civilization.Cities.FirstOrDefault()?.Location; 
        }

        public void WeLoveTheKingCanceled(City city)
        {
        }

        public Tile ActiveTile { get; set; }
        public Unit? ActiveUnit { get; set; }

        public List<Unit> WaitingList { get; }

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

        public void SelectNewAdvance(IGame game, List<Advance> researchPossibilities)
        {
            Civilization.ReseachingAdvance = game.Random.ChooseFrom(researchPossibilities).Index;
        }

        public void CantProduce(City city, IProductionOrder newItem)
        {
        }

        public void CityProductionComplete(City city)
        {
            
        }

        public IInterfaceCommands Ui { get; } = null;

        public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
        {
            // 
        }

        public void MapChanged(List<Tile> tiles)
        {
            //Does the AI care?
        }

        public void WaitingAtEndOfTurn()
        {
            //What should the AI do??
        }
    }
}