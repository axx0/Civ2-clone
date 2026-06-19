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
using Model.Core.Events;
using Model.Core.Mapping;
using Model.Core.Player;
using Model.Core.Units;
using Neo.IronLua;

namespace Civ2engine
{
    public partial class Game
    {
        public event EventHandler<PlayerEventArgs> OnPlayerEvent;

        private bool _choosingNextCiv;
        private bool _chooseNextCivAgain;

        public void StartNextTurn()
        {
            StartNextTurnCore();
            ChoseNextCiv();
        }

        private void StartNextTurnCore()
        {
            TurnNumber++;

            if (TurnNumber % 2 == 0)
            {
                Power.CalculatePowerRatings(this);
            }

            _activeCivId = -1;
        }

        public void ChoseNextCiv()
        {
            if (_choosingNextCiv)
            {
                _chooseNextCivAgain = true;
                return;
            }

            _choosingNextCiv = true;
            try
            {
                do
                {
                    _chooseNextCivAgain = false;
                    ChooseNextCivilizationOnce();
                } while (_chooseNextCivAgain);
            }
            finally
            {
                _choosingNextCiv = false;
            }
        }

        private void ChooseNextCivilizationOnce()
        {
            var safety = Math.Max(8, AllCivilizations.Count * 4);
            while (safety-- > 0)
            {
                if (_activeCivId >= AllCivilizations.Count - 1)
                {
                    StartNextTurnCore();
                }

                _activeCivId++;
                if (_activeCivId < 0 || _activeCivId >= AllCivilizations.Count)
                {
                    StartNextTurnCore();
                    continue;
                }

                _activeCiv = AllCivilizations[_activeCivId];

                if (!_activeCiv.Alive)
                {
                    if (!Options.DontRestartIfEliminated)
                    {
                        //Look to restart if possible
                    }
                    continue;
                }

                var activePlayer = Players[_activeCiv.Id];
                TurnBeginning(_activeCiv, activePlayer);

                if (_activeCiv.PlayerType == PlayerType.Barbarians)
                {
                    ProcessBarbarianTurn(activePlayer);
                    continue;
                }

                StartPlayerTurn(activePlayer);
                return;
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
                activePlayer.WaitingAtEndOfTurn();
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
            
            // Reset turns of all units and let resting wounded units recover.
            foreach (var unit in activeCiv.Units.Where(n => !n.Dead))
            {
                HealRestingUnit(unit);
                unit.MovePointsLost = 0;
            }

            // Update all cities
            this.CitiesTurn(player);
        }

        private static void HealRestingUnit(Unit unit)
        {
            if (unit.HitPointsLost <= 0)
            {
                return;
            }

            var inFriendlyCity = unit.CurrentLocation.CityHere?.Owner == unit.Owner;
            var resting = unit.Order is (int)OrderType.Sleep or (int)OrderType.Fortify or (int)OrderType.Fortified;
            if (!resting && !inFriendlyCity)
            {
                return;
            }

            var healed = inFriendlyCity ? 2 : 1;
            unit.HitPointsLost = Math.Max(0, unit.HitPointsLost - healed);
        }

        private void ProcessBarbarianTurn(IPlayer activePlayer)
        {
            activePlayer.TurnStart(TurnNumber);

            foreach (var unit in _activeCiv.Units.Where(u => !u.Dead).ToList())
            {
                if (unit.AttackBase <= 0)
                {
                    unit.SkipTurn();
                    continue;
                }

                while (!unit.Dead && unit.MovePoints > 0)
                {
                    var currentTile = unit.CurrentLocation;
                    var adjacentEnemy = FindAdjacentBarbarianTarget(unit, currentTile);
                    if (adjacentEnemy != null)
                    {
                        MovementFunctions.AttackAtTile(unit, this, adjacentEnemy);
                        if (!unit.Dead && unit.MovePoints <= 0)
                        {
                            break;
                        }

                        if (unit.Dead)
                        {
                            break;
                        }
                    }
                    else
                    {
                        var target = FindNearestBarbarianTarget(unit, currentTile);
                        if (target == null)
                        {
                            unit.SkipTurn();
                            break;
                        }

                        var destination = MovementFunctions.GetPossibleMoves(currentTile, unit)
                            .Where(t => !t.Terrain.Impassable)
                            .OrderBy(t => BarbarianDistance(t, target))
                            .FirstOrDefault();

                        if (destination == null || destination == currentTile)
                        {
                            unit.SkipTurn();
                            break;
                        }

                        if (IsEnemyTileForBarbarian(unit, destination))
                        {
                            MovementFunctions.AttackAtTile(unit, this, destination);
                        }
                        else
                        {
                            MovementFunctions.MoveC2(this, unit, destination.X - currentTile.X, destination.Y - currentTile.Y);
                        }
                    }
                }
            }
        }

        private static Tile? FindAdjacentBarbarianTarget(Unit unit, Tile currentTile)
        {
            return currentTile.Neighbours()
                .Where(tile => IsEnemyTileForBarbarian(unit, tile))
                .OrderBy(tile => tile.CityHere == null ? 1 : 0)
                .ThenBy(tile => BarbarianDistance(currentTile, tile))
                .FirstOrDefault();
        }

        private Tile? FindNearestBarbarianTarget(Unit unit, Tile currentTile)
        {
            Tile? best = null;
            var bestDistance = int.MaxValue;
            foreach (var tile in currentTile.Map.Tile)
            {
                if (!IsEnemyTileForBarbarian(unit, tile))
                {
                    continue;
                }

                var distance = BarbarianDistance(currentTile, tile);
                if (distance < bestDistance)
                {
                    best = tile;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private static bool IsEnemyTileForBarbarian(Unit unit, Tile tile)
        {
            if (tile.CityHere is { } city && city.Owner != unit.Owner)
            {
                return true;
            }

            return tile.UnitsHere.Any(other => !other.Dead && other.Owner != unit.Owner && other.InShip == null);
        }

        private static int BarbarianDistance(Tile from, Tile to)
        {
            return Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
        }
    }
}
