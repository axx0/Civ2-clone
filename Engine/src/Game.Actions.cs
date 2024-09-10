using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Statistics;
using Civ2engine.UnitActions;
using Civ2engine.Units;
using Model.Core;

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
            
            ProcessBarbarians();

            ChoseNextCiv();
        }
        
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

        private void MoveTowards(Tile tile, List<Unit> units, IMapItem target)
        {
            var destination = MovementFunctions.GetPossibleMoves(this, tile, units[0])
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
            if (_activeCiv == ActiveCivs[^1])
            {
                StartNextTurn();
            }
            else
            {
                _activeCiv = ActiveCivs[ActiveCivs.FindIndex(civ => civ == _activeCiv) + 1];

                if (_activeCiv.Alive)
                {
                    TurnBeginning(Players[_activeCiv.Id]);

                    if (_activeCiv.PlayerType == PlayerType.Ai)
                    {
                        AiTurn();
                    }
                    else
                    {
                        ChooseNextUnit();
                    }

                    OnPlayerEvent?.Invoke(null, new PlayerEventArgs(PlayerEventType.NewTurn, _activeCiv.Id));

                }
                else if(!Options.DontRestartIfEliminated)
                {
                    //Look to restart if possible
                }
            }
        }

        public void SetHumanPlayer(int civId)
        {
            AllCivilizations.ForEach(c => c.PlayerType = PlayerType.Ai);
            AllCivilizations[0].PlayerType = PlayerType.Barbarians;
            AllCivilizations[civId].PlayerType = PlayerType.Local;
        }

        public void AiTurn()
        {
            foreach (var unit in _activeCiv.Units.Where(u => !u.Dead).ToList())
            {
                var currentTile = unit.CurrentLocation;
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
                    case AIroleType.NavalSuperiority:
                        break;
                    case AIroleType.AirSuperiority:
                        break;
                    case AIroleType.SeaTransport:
                        break;
                    case AIroleType.Settle:
                        var cityTile = CurrentMap.CityRadius(currentTile)
                            .FirstOrDefault(t => t.CityHere != null);
                        
                        if (currentTile.Fertility == -2 && cityTile == null && currentTile.Type != TerrainType.Ocean)
                        {
                            CityActions.AiBuildCity(unit, this);
                        }
                        
                        if (cityTile == null && currentTile.Type != TerrainType.Ocean)
                        {
                            var moreFertile = MovementFunctions.GetPossibleMoves(this, currentTile, unit)
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

        private void TurnBeginning(IPlayer player)
        {
            // Reset turns of all units
            foreach (var unit in GetActiveCiv.Units.Where(n => !n.Dead))
            {
                unit.MovePointsLost = 0;
            }

            // Update all cities
            CitiesTurn(player);
        }
    }
}
