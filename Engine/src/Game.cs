using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Scripting;
using Civ2engine.Units;
using Model;
using Model.Core;

namespace Civ2engine
{
    public partial class Game : IGame
    {
        private readonly Options _options;
        private readonly Rules _rules;
        private readonly Scenario _scenarioData;
        private int _difficultyLevel;
        private readonly BarbarianActivityType _barbarianActivity;
        public FastRandom Random { get; set; } = new();
        public List<City> AllCities { get; } = new();

        public IHistory History
        {
            get { return _history ??= HistoryUtils.ReconstructHistory(this); }
        }

        public List<Civilization> AllCivilizations { get; } = new();

        public List<Civilization> ActiveCivs => AllCivilizations.Where(c => c.Alive).ToList();
        public Options Options => _options;
        public Scenario ScenarioData => _scenarioData;
        public Rules Rules => _rules;

        public IGameDate Date { get; }
        public int TurnNumber { get; private set; }

        public int DifficultyLevel { get; set; }
        public int BarbarianActivity => (int)_barbarianActivity;
        public int PollutionSkulls { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }

        public Tile ActiveTile
        {
            get => Players[_activeCiv.Id].ActiveTile;
            set => Players[_activeCiv.Id].ActiveTile = value;
        }

        public IPlayer ActivePlayer => Players[_activeCiv.Id];

        private int _activeCivId = -1;
        
        private Civilization
            _activeCiv; // ActiveCiv can be AI. PlayerCiv is human. They are equal except during enemy turns.

        public Civilization GetActiveCiv => _activeCiv;
        
        private readonly Map[] _maps;

        public IList<Map> Maps => _maps;

        private History? _history;
        public IScriptEngine Script { get; }

        public int NoMaps => _maps.Length;

        public int TotalMapArea => _maps.Select(m => m.Tile.GetLength(0) * m.Tile.GetLength(1)).Sum();
        public Dictionary<string, List<string>?> CityNames { get; set; }
        public Civilization GetPlayerCiv => AllCivilizations.FirstOrDefault(c => c.PlayerType == PlayerType.Local);

        public IPlayer[] Players { get; }

        public void TriggerUnitEvent(UnitEventType eventType, IUnit movedUnit,
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
                //var tiles = tilesChanged.Where(t => t.Map.IsCurrentlyVisible(t, player.Civilization.Id)).ToList();
                var tiles = tilesChanged;
                if (tiles.Count > 0)
                {
                    tiles.ForEach(t => t.UpdatePlayer(player.Civilization.Id));
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

        public IDictionary<int, TerrainImprovement> TerrainImprovements { get; set; }

        private double ComputeMaxDistance()
        {
            var xLength = _maps[0].Tile.GetLength(0);
            var yLength = _maps[0].Tile.GetLength(1);

            if (_options.FlatEarth)
            {
                return Utilities.DistanceTo(_maps[0].Tile[0, 0], _maps[0].Tile[xLength - 1, yLength - 1]);
            }

            return Utilities.DistanceTo(_maps[0].Tile[0, 0], _maps[0].Tile[(int)xLength / 2, yLength - 1]);
        }

        public string Order2String(int unitOrder)
        {
            var order = Rules.Orders.FirstOrDefault(t => t.Type == unitOrder);
            return order != null ? order.Name : Labels.For(LabelIndex.NoOrders);
        }
        
        public void ConnectPlayer(IPlayer player)
        {
            var id = player.Civilization.Id;
            var currentPlayer = Players[id];
            player.ActiveTile = currentPlayer.ActiveTile;
            player.SetUnitActive(currentPlayer.ActiveUnit, false);
            Players[id] = player;
            Script.Connect(player.Ui);
        }
    }
}