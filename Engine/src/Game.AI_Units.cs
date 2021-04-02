using System;
using System.Collections.Generic;
using System.Linq;
using ExtensionMethods;
using Civ2engine.Enums;

namespace Civ2engine
{
    public partial class Game : BaseInstance
    {
        public OrderType AI_UnitOrder()
        {
            OrderType order = RandomJustMovement();

            return order;
        }

        // Chose random direction of movement, if no movement possible skip turn
        private OrderType RandomJustMovement()
        {
            OrderType moveDir;

            var possibleDirections = new List<OrderType> { OrderType.MoveNE, OrderType.MoveE, OrderType.MoveSE, OrderType.MoveS, OrderType.MoveSW, OrderType.MoveW, OrderType.MoveNW, OrderType.MoveN };
            var random = new Random();
            // Try out all possible movement directions, remove a direction if it isn't possible
            while (possibleDirections.Any())
            {
                // Determine random movement direction
                moveDir = possibleDirections[random.Next(possibleDirections.Count)];
                int[] newCoords = _activeUnit.NewUnitCoords(moveDir);

                // Check if movement in this direction is possible
                // NOT if enemy unit & civ is there
                var city = CityHere(newCoords[0], newCoords[1]);
                var unitsHere = UnitsHere(newCoords[0], newCoords[1]);
                if ((city != null && city.Owner != _activeUnit.Owner) || (unitsHere.Any() && unitsHere.First().Owner != _activeUnit.Owner))
                {
                    possibleDirections.Remove(moveDir);   // Since movement here is not possible, remove this order from list of possible directions
                }
                // Check if unit can move in this direction (terrain restrictions)
                else
                {
                    if (_activeUnit.Domain == UnitGAS.Ground)
                    {
                        if (Map.TileC2(newCoords[0], newCoords[1]).Type != TerrainType.Ocean) return moveDir;
                        else possibleDirections.Remove(moveDir);
                    }
                    else if (_activeUnit.Domain == UnitGAS.Sea)
                    {
                        if (Map.TileC2(newCoords[0], newCoords[1]).Type == TerrainType.Ocean) return moveDir;
                        else possibleDirections.Remove(moveDir);
                    }
                    else    // Air unit
                    {
                        return moveDir;
                    }
                }
            }

            // All possible directions depleted --> for now just pause turn
            return OrderType.SkipTurn;
        }
    }
}
