using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Scripting;
using Civ2engine.Units;

namespace Civ2engine
{
    public partial class Game
    {
        private readonly Options _options;
        private readonly Rules _rules;
        private readonly Scenario _scenarioData;
        private DifficultyType _difficultyLevel;
        private readonly BarbarianActivityType _barbarianActivity;
        public FastRandom Random { get; set; } = new();
        public List<City> AllCities { get; } = new();

        public History History
        {
            get { return _history ??= new History(this); }
        }

        public List<Civilization> AllCivilizations { get; } = new();

        public List<Civilization> GetActiveCivs => AllCivilizations.Where(c => c.Alive).ToList();
        public Options Options => _options;
        public Scenario ScenarioData => _scenarioData;
        public Rules Rules => _rules;

        public Date Date;
        public int TurnNumber { get; private set; }

        public DifficultyType DifficultyLevel => _difficultyLevel;
        public BarbarianActivityType BarbarianActivity => _barbarianActivity;
        public int PollutionSkulls { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }

        public Tile ActiveTile
        {
            get => Players[_activeCiv.Id].ActiveTile;
            set => Players[_activeCiv.Id].ActiveTile = value;
        }

        public IPlayer ActivePlayer => Players[_activeCiv.Id];

        public Unit? ActiveUnit
        {
            get => Players[_activeCiv.Id].ActiveUnit;
            set
            {
                var player = Players[_activeCiv.Id];
                player.ActiveUnit = value;
                if (player.ActiveUnit == value && value != null)
                {
                    TriggerUnitEvent(new ActivationEventArgs(value, true, false));
                }
            }
        }

        private Civilization
            _activeCiv; // ActiveCiv can be AI. PlayerCiv is human. They are equal except during enemy turns.

        public Civilization GetActiveCiv => _activeCiv;


        // Singleton instance of a game
        private static Game _instance;
        private readonly Map[] _maps;

        public static Game Instance
        {
            get
            {
                if (_instance == null)
                {
                    Console.WriteLine("Game instance does not exist!");
                }

                return _instance;
            }
        }

        private History _history;
        public ScriptEngine Script { get; }

        public Map CurrentMap => _maps[ActiveTile.Z];
        public int NoMaps => _maps.Length;

        public int TotalMapArea => _maps.Select(m => m.Tile.GetLength(0) * m.Tile.GetLength(1)).Sum();
        internal Dictionary<string, List<string>> CityNames { get; set; }
        public Civilization GetPlayerCiv => AllCivilizations.FirstOrDefault(c => c.PlayerType == PlayerType.Local);

        public IPlayer[] Players { get; }

        public void TriggerUnitEvent(UnitEventType eventType, Unit movedUnit,
            BlockedReason blockedReason = BlockedReason.NotBlocked)
        {
            OnUnitEvent?.Invoke(this, new MovementBlockedEventArgs(eventType, movedUnit, blockedReason));
        }

        public void TriggerUnitEvent(UnitEventArgs args)
        {
            OnUnitEvent?.Invoke(this, args);
        }

        public void TriggerMapEvent(MapEventType eventType, List<Tile> tilesChanged)
        {
            foreach (var player in Players)
            {
                var tiles = tilesChanged.Where(t => t.Map.IsCurrentlyVisible(t, player.Civilization.Id)).ToList();
                if (tiles.Count > 0)
                {
                    tiles.ForEach(t=>t.UpdatePlayer(player.Civilization.Id));
                    player.MapChanged(tiles);
                }
            }   
            OnMapEvent?.Invoke(this, new MapEventArgs(eventType)
                { TilesChanged = tilesChanged });
        }

        private double? _maxDistance;

        public double MaxDistance
        {
            get { return _maxDistance ??= ComputeMaxDistance(); }
        }

        public IDictionary<int,TerrainImprovement> TerrainImprovements { get; set; }

        private double ComputeMaxDistance()
        {
            var xLength = _maps[0].Tile.GetLength(0);
            var yLength = _maps[0].Tile.GetLength(1);

            if (_options.FlatEarth)
            {
                return Utilities.DistanceTo(_maps[0].Tile[0, 0], _maps[0].Tile[xLength - 1, yLength - 1], true);
            }

            return Utilities.DistanceTo(_maps[0].Tile[0, 0], _maps[0].Tile[(int)xLength / 2, yLength - 1],
                false);
        }

        public string Order2String(OrderType unitOrder)
        {
            var order = Rules.Orders.FirstOrDefault(t => t.Type == unitOrder);
            return order != null ? order.Name : Labels.For(LabelIndex.NoOrders);
        }

        public void SetImprovementsForCity(City city)
        {
            SetImprovementsForCities(city.Owner, city);
        }
        
        public void SetImprovementsForCities(Civilization civilization, params City[] cities)
        {
            var citiesToSet = cities.Length == 0 ? civilization.Cities : (IList<City>)cities;
            var improvements = TerrainImprovements.Values
                .Where(t => t.AllCitys)
                .Select(improvement =>
                {
                    var level = -1;
                    for (var i = 0; i < improvement.Levels.Count; i++)
                    {
                        if (AdvanceFunctions.HasTech(civilization, improvement.Levels[i].RequiredTech))
                        {
                            level = i;
                        }
                    }
                    return new { improvement, level };
                }).Where(ti => ti.level != -1)
                .ToList();
            
            
            foreach (var tile in citiesToSet.Select(c=>c.Location))
            {
                tile.Improvements.Clear();
                tile.EffectsList.Clear();
                foreach (var can in improvements)
                {
                    var terrain = can.improvement.AllowedTerrains[tile.Z]
                        .FirstOrDefault(t => t.TerrainType == (int)tile.Type);
                    if (terrain is not null)
                    {
                        tile.AddImprovement(can.improvement, terrain, can.level, Rules.Terrains[tile.Z]);
                    }
                }
            }
        }

        public void ConnectPlayer(IPlayer player)
        {
            var id = player.Civilization.Id;
            var currentPlayer = Players[id];
            player.ActiveTile = currentPlayer.ActiveTile;
            player.ActiveUnit = currentPlayer.ActiveUnit;
            Players[id] = player;
            Script.Connect(player.Ui);
        }

        public void UpdatePlayerViewData()
        {
            foreach (var map in _maps)
            {
                for (int y = 0; y < map.Tile.GetLength(1); y++)
                {
                    for (int x = 0; x < map.Tile.GetLength(0); x++)
                    {
                        var tile = map.Tile[x, y];
                        tile.UpdateAllPlayers();
                        if (tile.Owner == -1)
                        {
                            if (tile.IsUnitPresent)
                            {
                                tile.Owner = tile.UnitsHere[0].Owner.Id;
                            }else if (tile.CityHere != null)
                            {
                                tile.Owner = tile.CityHere.OwnerId;
                            }else if (tile.WorkedBy != null)
                            {
                                tile.Owner = tile.WorkedBy.OwnerId;
                            }
                        }
                    }
                }
            }
        }
    }
}
