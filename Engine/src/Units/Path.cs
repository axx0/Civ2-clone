using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.UnitActions;
using Model.Core;

namespace Civ2engine.Units;

public class Path
{
    public Dictionary<Tile, Route> RouteDebugData { get; }

    private Path(Route candidate, Dictionary<Tile, Route> routeDebugData)
    {
        RouteDebugData = routeDebugData;
        var tiles = new Tile[candidate.Steps];
        var cur = candidate;
        for (var i = tiles.Length - 1; i >= 0; i--)
        {
            tiles[i] = cur.Tile;
            cur = cur.Previous;
        }
        this.Tiles = tiles;
    }

    public Tile[] Tiles { get; set; }

    private const int NotPossible = -1;

    public static Path? CalculatePathBetween(IGame game, Tile startTile, Tile endTile, UnitGas domain, int moveFactor,
        Civilization owner, bool alpine, bool ignoreZoc)
    {
        if (startTile.Z != endTile.Z || !endTile.IsVisible(owner.Id)) return null;

        switch (domain)
        {
            case UnitGas.Ground when startTile.Island != endTile.Island && startTile.Neighbours().All(t=>t.Island != endTile.Island):
            case UnitGas.Sea when !OnCompatibleSea(startTile, endTile):
                return null;
        }
        

        var rules = game.Rules;
        var costFunction = domain switch
        {
            UnitGas.Ground => BuildGroundMovementFunction(rules.Cosmic, alpine, moveFactor),
            UnitGas.Air => (source, dest) => rules.Cosmic.MovementMultiplier,
            UnitGas.Sea => (source, dest) => dest == endTile || dest.Type == TerrainType.Ocean || (dest.CityHere is { } && dest.CityHere.OwnerId == owner.Id)
                    ? rules.Cosmic.MovementMultiplier
                    : NotPossible,
            UnitGas.Special => (source, dest) => rules.Cosmic.MovementMultiplier,
            _ => throw new ArgumentOutOfRangeException(nameof(domain), domain, null)
        };

        var visited = new HashSet<Tile> { startTile };
        var initialRoute = new Route { Cost = 0, Previous = null, Steps = 0, Tile = startTile, Distance = Utilities.DistanceTo(startTile, endTile) };
        var candidates = new LinkedList<Route>(
            GetInitialCandidates(startTile, ignoreZoc, owner, domain)
                .Select(t => new Route
                {
                    Tile = t, Cost = costFunction(startTile, t), Previous = initialRoute, Steps = 1,
                    Distance = Utilities.DistanceTo(t, endTile)
                })
                .Where(r => r.Cost != NotPossible)
                .OrderBy(r => r.Cost)
                .ThenBy(r => r.Steps)
                .ThenBy(r=>r.Distance)
            );


        var debug = new Dictionary<Tile, Route> { { startTile, initialRoute } };

        while (candidates.First != null)
        {
            var candidate = candidates.First.Value;
            candidates.RemoveFirst();
            if (candidate.Tile == endTile)
            {
                debug.Add(candidate.Tile, candidate);
                return new Path(candidate, debug);
            }

            if (visited.Contains(candidate.Tile)) continue;
            debug.Add(candidate.Tile, candidate);
            visited.Add(candidate.Tile);

            foreach (var neighbour in candidate.Tile.Neighbours())
            {
                if (!neighbour.IsVisible(owner.Id) || visited.Contains(neighbour)) continue;

                var cost = costFunction(candidate.Tile, neighbour);
                if (cost == NotPossible) continue;

                var route = new Route
                {
                    Cost = candidate.Cost + cost, Previous = candidate, Steps = candidate.Steps + 1, Tile = neighbour, Distance = Utilities.DistanceTo(candidate.Tile, endTile)
                };
                var nextBest = candidates.First;
                while (nextBest != null && nextBest.Value.Cost < route.Cost)
                {
                    nextBest = nextBest.Next;
                }

                while (nextBest != null && nextBest.Value.Cost == route.Cost && nextBest.Value.Steps < route.Steps)
                {
                    nextBest = nextBest.Next;
                }
                while (nextBest != null && nextBest.Value.Cost == route.Cost && nextBest.Value.Steps == route.Steps && nextBest.Value.Distance <= route.Distance)
                {
                    nextBest = nextBest.Next;
                }

                if (nextBest == null)
                {
                    candidates.AddLast(route);
                }
                else
                {
                    candidates.AddBefore(nextBest, route);
                }
            }
        }

        return null;
    }

    private static IEnumerable<Tile> GetInitialCandidates(Tile startTile, bool ignoreZoc, Civilization owner, UnitGas domain)
    {
        if (ignoreZoc)
        {
            return startTile.Neighbours().Where(n => !n.IsUnitPresent || MovementFunctions.IsFriendlyTile(n, owner));

        }

        if (!MovementFunctions.IsNextToEnemy(startTile, owner, domain))
        {
            return startTile.Neighbours();
        }

        return startTile.Neighbours().Where(n =>
            MovementFunctions.IsFriendlyTile(n, owner) || !MovementFunctions.IsNextToEnemy(n, owner, domain));
    }

    private static Func<Tile, Tile, int> BuildGroundMovementFunction(CosmicRules cosmic, bool alpine, int moveFactor)
    {

        if (alpine)
        {
            return (source, dest) => dest.Type != TerrainType.Ocean
                ? MovementFunctions.MoveCost(source, dest, cosmic.AlpineMovement, cosmic)
                : NotPossible;
        }

        return (source, dest) => dest.Type != TerrainType.Ocean
            ? Math.Min(moveFactor, MovementFunctions.MoveCost(source, dest, cosmic.MovementMultiplier * dest.MoveCost, cosmic))
            : NotPossible;
    }

    private static bool OnCompatibleSea(Tile startTile, Tile endTile)
    {
        if (startTile.Type == TerrainType.Ocean)
            return startTile.Island == endTile.Island || endTile.Neighbours()
                .Any(t => t.Type == TerrainType.Ocean && t.Island == startTile.Island);

        var possibleOceans = startTile.Neighbours().Where(t => t.Type == TerrainType.Ocean).Select(t => t.Island)
            .Distinct().ToList();
        return possibleOceans.Contains(endTile.Island) || endTile.Neighbours()
            .Any(t => t.Type == TerrainType.Ocean && possibleOceans.Contains(t.Island));
    }

    public void Follow(IGame game, Unit unit)
    {
        int pos = 0;
        do
        {
            var tileTo = Tiles[pos++];
            var tileFrom = unit.CurrentLocation;
            if (MovementFunctions.UnitMoved(game, unit, tileTo, unit.CurrentLocation!))
            {
                var neighbours = tileTo.Neighbours().Where(n => !n.IsVisible(unit.Owner.Id)).ToList();
                if (neighbours.Count > 0)
                {
                    neighbours.ForEach(n => n.SetVisible(unit.Owner.Id));
                    game.TriggerMapEvent(MapEventType.UpdateMap, neighbours);
                }
                game.TriggerUnitEvent(new MovementEventArgs(unit, tileFrom, tileTo));
            }
        } while (unit.MovePoints > 0 && pos < Tiles.Length &&
                 !MovementFunctions.IsNextToEnemy(unit.CurrentLocation!, unit.Owner, unit.Domain));

        if (unit.MovePoints > 0)
        {
            unit.Order = (int)OrderType.NoOrders;
        }
    }
}

public class Route
{
    public int Steps { get; init; }
    
    public decimal Cost { get; init; }
    
    public Route Previous { get; init; }
    
    public Tile Tile { get; set; }
    public double Distance { get; set; }
}