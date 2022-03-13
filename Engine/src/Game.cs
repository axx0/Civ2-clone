using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.Scripting;
using Civ2engine.Units;
using Civ2engine.Terrains;

namespace Civ2engine
{
    public partial class Game : BaseInstance
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
        public string GetGameYearString => GetGameYear < 0 ? string.Join(" ", new string[] { Math.Abs(GetGameYear).ToString(), "B.C." }) : string.Join(" ", new string[] { GetGameYear.ToString(), "A.D." });
        public int TurnNumberForGameYear { get; set; }
        public DifficultyType DifficultyLevel => _difficultyLevel;
        public BarbarianActivityType BarbarianActivity => _barbarianActivity;
        public int PollutionAmount { get; set; }
        public int GlobalTempRiseOccured { get; set; }
        public int NoOfTurnsOfPeace { get; set; }
        
        private Unit _activeUnit;
        public Unit ActiveUnit
        {
            get
            {
                return _activeUnit;
            }
            set
            {
                if (!value.TurnEnded)
                {
                    _activeUnit = value;
                    TriggerUnitEvent( new ActivationEventArgs(_activeUnit, true, false));
                }
                else
                {
#if DEBUG
                    throw new NotSupportedException("Tried to set ended unit to active");
#endif
                }
            }
        }
        
        private Civilization _activeCiv;    // ActiveCiv can be AI. PlayerCiv is human. They are equal except during enemy turns.
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

        private int _currentMap = 0;
        private History _history;
        public ScriptEngine Script { get; }

        public Map CurrentMap => _maps[_currentMap];

        public int TotalMapArea => _maps.Select(m => m.Tile.GetLength(0) * m.Tile.GetLength(1)).Sum();
        internal Dictionary<string,List<string>> CityNames { get; set; }
        public Civilization GetPlayerCiv => AllCivilizations.FirstOrDefault(c => c.PlayerType == PlayerType.Local);
        public AIPlayer AI { get; }
        
        public IDictionary<PlayerType, IPlayer> Players { get; }

        public void TriggerUnitEvent(UnitEventType eventType, Unit movedUnit, BlockedReason blockedReason = BlockedReason.NotBlocked)
        {
            OnUnitEvent?.Invoke(this,new MovementBlockedEventArgs(eventType, movedUnit, blockedReason));
        }

        public void TriggerUnitEvent(UnitEventArgs args)
        {
            OnUnitEvent?.Invoke(this,args);
        }

        public void TriggerMapEvent(MapEventType eventType, List<Tile> tilesChanged)
        {
            OnMapEvent?.Invoke(this, new MapEventArgs(eventType)
                {TilesChanged = tilesChanged});
        }

        private double? _maxDistance;
        public double MaxDistance
        {
            get
            {
                return _maxDistance ??= ComputeMaxDistance();
            }
        }

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
    }
}
