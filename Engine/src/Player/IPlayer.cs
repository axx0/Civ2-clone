using System.Collections.Generic;
using Civ2engine.Improvements;
using Civ2engine.Production;

namespace Civ2engine
{
    public interface IPlayer
    {
        void CivilDisorder(City city);
        void OrderRestored(City city);
        void WeLoveTheKingStarted(City city);
        void WeLoveTheKingCanceled(City city);
        void CantMaintain(City city, Improvement cityImprovement);
        void SelectNewAdvance(Game game, Civilization activeCiv, IList<int> researchPossibilities);
        
        void CantProduce(City city, ProductionOrder newItem);
        
        void CityProductionComplete(City city);
    }
}