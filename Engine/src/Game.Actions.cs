using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.Terrains;
using Civ2engine.UnitActions.Move;
using Civ2engine.Units;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public static event EventHandler<PlayerEventArgs> OnPlayerEvent;

        private void StartNextTurn()
        {
            _turnNumber++;
            ProcessBarbarians();

            ChoseNextCiv();
        }
        
        private void ProcessBarbarians()
        {
            _activeCiv = AllCivilizations[0];
            
            if(_turnNumber < 16) return;
            
            TurnBeginning();
            
            //Pick a random tile if valid for barbarians raise horde
            
            //Move all barbarians
            var barbarianGroups = _activeCiv.Units.Where(u => u.Dead == false)
                .GroupBy(u =>u.CurrentLocation);
            foreach (var barbarianGroup in barbarianGroups)
            {
                var tile = barbarianGroup.Key;
                var barbarians = barbarianGroup.ToList();
                var target = GetCities.OrderBy(c => DistanceTo(tile, c)).FirstOrDefault();
                if(target == null) continue;
                
                MoveTowards(tile, barbarians, target);
            }
        }

        private void MoveTowards(Tile tile, List<Unit> units, IMapItem target)
        {
            var destination = MovementFunctions.GetPossibleMoves(this, tile, units[0])
                .OrderBy(t => DistanceTo(t, target)).FirstOrDefault();
            if (destination == null) return;
            
            units.ForEach(b => MovementFunctions.UnitMoved(this, b,  destination, tile));
            if (units.Any(b => b.MovePoints > 0))
            {
                MoveTowards(destination, units.Where(b=>b.MovePoints > 0).ToList(), target);
            }
        }

        public void ChoseNextCiv()
        {
            if (_activeCiv == AllCivilizations[^1])
            {
                StartNextTurn();
            }
            else
            {
                _activeCiv = AllCivilizations[_activeCiv.Id + 1];

                if (_activeCiv.Alive)
                {
                    

                    TurnBeginning();
                    
                    OnPlayerEvent?.Invoke(null, new PlayerEventArgs(PlayerEventType.NewTurn));


                    if (_activeCiv != _playerCiv)
                    {
                        AITurn();
                    }
                    else
                    {
                        // Choose next unit
                        ChooseNextUnit();
                    }

                }
                else if(!Options.DontRestartIfEliminated)
                {
                    //Look to restart if possible
                }
            }
        }

        private void AITurn()
        {
            foreach (var unit in _activeCiv.Units.Where(u => !u.Dead))
            {
                var currentTile = CurrentMap.TileC2(unit.X, unit.Y);
                switch (unit.AIrole)
                {
                    case AIroleType.Attack:
                        break;
                    case AIroleType.Defend:
                        if (currentTile.CityHere != null)
                        {
                            if (currentTile.UnitsHere.Count(u => u != unit && u.AIrole == AIroleType.Defend) <
                                2 + currentTile.CityHere.Size / 3)
                            {
                                if (unit.Order == OrderType.Fortify || unit.Order == OrderType.Fortified)
                                {
                                    unit.Order = OrderType.Fortified;
                                }
                                else
                                {
                                    unit.Order = OrderType.Fortify;
                                }
                                unit.MovePointsLost = unit.MovePoints;
                            }
                        }
                        else
                        {
                            
                        }
                        break;
                    case AIroleType.NavalSuperiority:
                        break;
                    case AIroleType.AirSuperiority:
                        break;
                    case AIroleType.SeaTransport:
                        break;
                    case AIroleType.Settle:
                        break;
                    case AIroleType.Diplomacy:
                        break;
                    case AIroleType.Trade:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                while (unit.MovePoints > 0)
                {
                    var possibleMoves = MovementFunctions.GetPossibleMoves(this, currentTile, unit).ToList();
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

        private void TurnBeginning()
        {
            // Reset turns of all units
            foreach (var unit in GetActiveCiv.Units.Where(n => !n.Dead))
            {
                unit.MovePointsLost = 0;

                // Increase counters
                if (unit.Order == OrderType.BuildIrrigation || (unit.Order == OrderType.BuildRoad) ||
                    (unit.Order == OrderType.BuildMine)) unit.Counter += 1;
            }

            // Update all cities
            CitiesTurn();
        }
    }
}
