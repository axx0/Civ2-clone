using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Scripting;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine
{
    public partial class Game
    {
        private readonly Options _options;
        private readonly Rules _rules;
        private readonly GameVersionType _gameVersion;
        private readonly DifficultyType _difficultyLevel;
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
        public Rules Rules => _rules;
        public GameVersionType GameVersion => _gameVersion;

        public int TurnNumber { get; private set; }

        private int _gameYear;

        public int GetGameYear
        {
            get
            {
                _gameYear = TurnNumber switch
                {
                    < 250 => -4000 + (TurnNumber - 1) * 20,
                    >= 250 and < 300 => 1000 + (TurnNumber - 1 - 250) * 10,
                    >= 300 and < 350 => 1500 + (TurnNumber - 1 - 300) * 5,
                    >= 350 and < 400 => 1750 + (TurnNumber - 1 - 350) * 2,
                    _ => 1850 + (TurnNumber - 1 - 400)
                };
                return _gameYear;
            }
        }

        public string GetGameYearString => GetGameYear < 0
            ? string.Join(" ", new string[] { Math.Abs(GetGameYear).ToString(), "B.C." })
            : string.Join(" ", new string[] { GetGameYear.ToString(), "A.D." });

        public int TurnNumberForGameYear { get; set; }
        public DifficultyType DifficultyLevel => _difficultyLevel;
        public BarbarianActivityType BarbarianActivity => _barbarianActivity;
        public int PollutionAmount { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }

        public Tile ActiveTile
        {
            get => Players[_activeCiv.Id].ActiveTile;
            set => Players[_activeCiv.Id].ActiveTile = value;
        }

        public Unit ActiveUnit
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

        public string Order2string(OrderType unitOrder)
        {
            var order = Rules.Orders.FirstOrDefault(t => t.Type == unitOrder);
            return order != null ? order.Name : string.Empty;
        }

        public void SetImprovementsForCity(City city)
        {
            city.Location.Improvements.Clear();
            BuildImprovementsList(city.Location.Improvements, city.Owner);
        }
        
        public void SetImprovementsForCities(Civilization civilization)
        {
            var improvements = new List<ConstructedImprovement>();
            BuildImprovementsList(improvements, civilization);
            foreach (var city in civilization.Cities)
            {
                city.Location.Improvements.Clear();
                city.Location.Improvements.AddRange(improvements.Select(i=> new ConstructedImprovement(i)));
            }
        }

        private void BuildImprovementsList(List<ConstructedImprovement> improvements, Civilization owner)
        {
            foreach (var improvement in TerrainImprovements.Values.Where(i => i.AllCitys))
            {
                int level = -1;
                for (var i = 0; i < improvement.Levels.Count; i++)
                {
                    if (AdvanceFunctions.HasTech(owner, improvement.Levels[i].RequiredTech))
                    {
                        level = i;
                    }
                }

                if (level != -1)
                {
                    improvements.Add(new ConstructedImprovement
                        { Group = improvement.ExclusiveGroup, Improvement = improvement.Id, Level = level });
                }
            }
        }
    }
}
