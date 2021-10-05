using Civ2engine.Enums;
using Civ2engine.Improvements;
using Civ2engine.Production;

namespace Civ2engine
{
    public class AIPlayer : IPlayer
    {
        private readonly DifficultyType _level;

        public AIPlayer(DifficultyType level)
        {
            _level = level;
        }

        public void WeLoveTheKingCanceled(City city)
        {
            throw new System.NotImplementedException();
        }

        public void CivilDisorder(City city)
        {
            throw new System.NotImplementedException();
        }

        public void OrderRestored(City city)
        {
            throw new System.NotImplementedException();
        }

        public void WeLoveTheKingStarted(City city)
        {
            throw new System.NotImplementedException();
        }

        public void CantMaintain(City city, Improvement cityImprovement)
        {
            throw new System.NotImplementedException();
        }

        public void SelectNewAdvance(Game game, Civilization activeCiv)
        {
            throw new System.NotImplementedException();
        }

        public void CantProduce(City city, ProductionOrder newItem)
        {
            throw new System.NotImplementedException();
        }

        public void CityProductionComplete(City city)
        {
            throw new System.NotImplementedException();
        }
    }
}