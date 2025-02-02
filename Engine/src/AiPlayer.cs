using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Core;
using Model.Core.Advances;
using Neo.IronLua;

namespace Civ2engine
{
    public class AiPlayer : IPlayer
    {
        private readonly Game _game;
        public AiInterface AI;
        public Civilization Civilization { get; set; }

        public int DifficultyLevel { get; }

        public AiPlayer(int difficultyLevel, Civilization civilization, Tile tile0, Game game)
        {
            _game = game;
            DifficultyLevel = difficultyLevel;
            Civilization = civilization;
            ActiveTile = civilization.Units.FirstOrDefault()?.CurrentLocation ??
                         civilization.Cities.FirstOrDefault()?.Location ?? tile0; 
            AIScript = civilization.PlayerType == PlayerType.Barbarians ? "barbarian.ai" : "default.ai";
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
            var res = AI.Call(AiEvent.ResearchComplete,
                new LuaTable { { "researchPossibilities", researchPossibilities } });
            Civilization.ReseachingAdvance = res switch
            {
                Advance advance => advance.Index,
                int index and >= 0 when index < researchPossibilities.Count => researchPossibilities[index].Index,
                _ => game.Random.ChooseFrom(researchPossibilities).Index
            };
        }

        public void CantProduce(City city, IProductionOrder newItem)
        {
        }

        public void CityProductionComplete(City city)
        {
            
        }

        public IInterfaceCommands Ui { get; } = null;
        public string AIScript { get; set; }

        public void NotifyImprovementEnabled(TerrainImprovement improvement, int level)
        {
            // 
        }

        public void MapChanged(List<Tile> tiles)
        {
            //Does the AI care?
        }

        public void WaitingAtEndOfTurn(IGame game)
        {
            //Call out to AI script to finalize turn
            AI.Call(AiEvent.TurnEnd, new LuaTable());
            
            //End turn
            game.ChoseNextCiv();
        }

        public void NotifyAdvanceResearched(int advance)
        {
            // What should the AI do??
        }

        public void FoodShortage(City city)
        {
            // What should the AI do??
        }

        public void CityDecrease(City city)
        {
            // What should the AI do??
        }

        public void TurnStart(int turnNumber)
        {
            AI.Call(AiEvent.TurnStart, new LuaTable { { "Turn", turnNumber } });
        }

        public void SetUnitActive(Unit? unit, bool move)
        {
            ActiveUnit = unit;
            if (unit != null)
            {
                if (unit.CurrentLocation != null) ActiveTile = unit.CurrentLocation;
                if (move)
                {
                    UnitAction? action = null;
                    var result = AI.Call(AiEvent.UnitOrdersNeeded, new LuaTable { { "Unit", unit } });
                    if (result is LuaResult { Count: > 0 } luaResult)
                    {
                        action = luaResult[0] switch
                        {
                            UnitAction luaAction => luaAction,
                            "B" => new BuildCityAction(unit, _game),
                            "F" => new FortifyAction(unit),
                            _ => action
                        };
                    }
                        
                    // Handle other things AI script could return? perhaps a tile indicating a GOTO order or a letter for a string order? (e.g. F for fortify)   
                    
                    action ??= new NothingAction(unit);
                    action.Execute();
                }
            }
        }
    }
}