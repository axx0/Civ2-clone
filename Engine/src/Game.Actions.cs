using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Scripting.ScriptObjects;
using Civ2engine.Statistics;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Constants;
using Model.Core;
using Neo.IronLua;

namespace Civ2engine
{
    public partial class Game
    {
        public event EventHandler<PlayerEventArgs> OnPlayerEvent;

        public void StartNextTurn()
        {
            TurnNumber++;

            if (TurnNumber % 2 == 0)
            {
                Power.CalculatePowerRatings(this);
            }

            _activeCivId = -1;
            ChoseNextCiv();
        }
        
/*
        private void ProcessBarbarians()
        {
            _activeCiv = AllCivilizations[0];
            
            if(TurnNumber < 16) return;
            
            TurnBeginning(Players[0]);
            
            //Pick a random tile if valid for barbarians raise horde
            
            //Move all barbarians
            var barbarianGroups = _activeCiv.Units.Where(u => u.Dead == false)
                .GroupBy(u =>u.CurrentLocation);
            foreach (var barbarianGroup in barbarianGroups)
            {
                var tile = barbarianGroup.Key;
                var barbarians = barbarianGroup.ToList();
                var target = AllCities.OrderBy(c => Utilities.DistanceTo(tile, c)).FirstOrDefault();
                if(target == null) continue;
                
                MoveTowards(tile, barbarians, target);
            }
        }
*/

        private void MoveTowards(Tile tile, List<Unit> units, IMapItem target)
        {
            var destination = MovementFunctions.GetPossibleMoves(tile, units[0])
                .OrderBy(t => Utilities.DistanceTo(t, target)).FirstOrDefault();
            if (destination == null) return;
            
            units.ForEach(b => MovementFunctions.UnitMoved(this, b,  destination, tile));
            if (units.Any(b => b.MovePoints > 0))
            {
                MoveTowards(destination, units.Where(b=>b.MovePoints > 0).ToList(), target);
            }
        }

        public void ChoseNextCiv()
        {
            if (_activeCivId >= AllCivilizations[^1].Id)
            {
                StartNextTurn();
                return;
            }

            _activeCivId++;

            _activeCiv = AllCivilizations[_activeCivId];

            if (_activeCiv.Alive)
            {
                var activePlayer = Players[_activeCiv.Id];
                TurnBeginning(_activeCiv, activePlayer);
                StartPlayerTurn(activePlayer);
            }
            else
            {
                if (!Options.DontRestartIfEliminated)
                {
                    //Look to restart if possible
                }
                ChoseNextCiv();
            }
        }

        public void StartPlayerTurn(IPlayer activePlayer)
        {
            activePlayer.TurnStart(TurnNumber);

            //If there are any units waiting to move goto move them
            if (_activeCiv.Units.Any(u => u is { MovePointsLost: 0, Order: (int)OrderType.NoOrders }))
            {
                ChooseNextUnit();
            }
            else
            {
                activePlayer.WaitingAtEndOfTurn(this);
            }
        }

        public void SetHumanPlayer(int civId)
        {
            AllCivilizations.ForEach(c => c.PlayerType = PlayerType.Ai);
            AllCivilizations[0].PlayerType = PlayerType.Barbarians;
            AllCivilizations[civId].PlayerType = PlayerType.Local;
        }

/*
        public void AiTurn()
        {
            foreach (var unit in _activeCiv.Units.Where(u => !u.Dead).ToList())
            {
                var currentTile = unit.CurrentLocation;
                switch (unit.AiRole)
                {
                    case AiRoleType.Attack:
                        break;
                    case AiRoleType.Defend:
                        if (currentTile.CityHere != null)
                        {
                            if (currentTile.UnitsHere.Count(u => u != unit && u.AiRole == AiRoleType.Defend) <
                                2 + currentTile.CityHere.Size / 3)
                            {
                                if (unit.Order == (int)OrderType.Fortify || unit.Order == (int)OrderType.Fortified)
                                {
                                    unit.Order = (int)OrderType.Fortified;
                                }
                                else
                                {
                                    unit.Order = (int)OrderType.Fortify;
                                }
                                unit.MovePointsLost = unit.MovePoints;
                            }
                        }
                        else
                        {
                            
                        }
                        break;
                    case AiRoleType.NavalSuperiority:
                        break;
                    case AiRoleType.AirSuperiority:
                        break;
                    case AiRoleType.SeaTransport:
                        break;
                    case AiRoleType.Settle:
                        var cityTile = CurrentMap.CityRadius(currentTile)
                            .FirstOrDefault(t => t.CityHere != null);
                        
                        if (currentTile.Fertility == -2 && cityTile == null && currentTile.Type != TerrainType.Ocean)
                        {
                            CityActions.AiBuildCity(unit, this);
                        }
                        
                        if (cityTile == null && currentTile.Type != TerrainType.Ocean)
                        {
                            var moreFertile = MovementFunctions.GetPossibleMoves(currentTile, unit)
                                .Where(n => n.Fertility > currentTile.Fertility).OrderByDescending(n => n.Fertility)
                                .FirstOrDefault();
                            if (moreFertile == null)
                            {
                                CityActions.AiBuildCity(unit, this);
                            }
                            else
                            {
                                if (MovementFunctions.UnitMoved(this, unit, moreFertile, currentTile))
                                {
                                    currentTile = moreFertile;
                                    if (unit.MovePoints > 0)
                                    {
                                        CityActions.AiBuildCity(unit, this);
                                    }
                                }
                            }
                        }

                        break;
                    case AiRoleType.Diplomacy:
                        break;
                    case AiRoleType.Trade:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                while (unit.MovePoints > 0)
                {
                    var possibleMoves = MovementFunctions.GetPossibleMoves(currentTile, unit).ToList();
                    if (unit.AttackBase == 0)
                    {
                        possibleMoves = possibleMoves
                            .Where(m => m.UnitsHere.Count == 0 || m.UnitsHere[0].Owner == unit.Owner).ToList();
                    }
                    if (possibleMoves.Count == 0)
                    {
                        unit.SkipTurn();
                    }
                    else
                    {
                        var destination = Random.ChooseFrom(possibleMoves);
                        if (destination.UnitsHere.Count > 0 && destination.UnitsHere[0].Owner != unit.Owner)
                        {
                            unit.Order = (int)OrderType.NoOrders;
                            MovementFunctions.AttackAtTile(unit, this, destination);
                        }
                        else if (MovementFunctions.UnitMoved(this, unit, destination, currentTile))
                        {
                            currentTile = destination;
                        }
                    }
                }
            }
            ChoseNextCiv();
        }
*/

        private void TurnBeginning(Civilization activeCiv, IPlayer player)
        {
            // Adjust reputation
            
            // Reset turns of all units
            foreach (var unit in activeCiv.Units.Where(n => !n.Dead))
            {
                unit.MovePointsLost = 0;
            }

            // Update all cities
            this.CitiesTurn(player);
        }
    }
}
