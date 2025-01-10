using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Civ2engine.IO;
using Civ2engine.MapObjects;

namespace Civ2engine
{
    public class GameInitializationConfig
    {
        public FastRandom Random { get; set; } = new (4564234);
        
        public Dictionary<string, PopupBox?> PopUps { get; set; }
        public int[] WorldSize { get; set; } = { 50, 80 };
        public int DifficultyLevel { get; set; }
        public int NumberOfCivs { get; set; }
        public int BarbarianActivity { get; set; }
        public bool SimplifiedCombat { get; set; }
        public bool FlatWorld { get; set; }
        public bool SelectComputerOpponents { get; set; }
        public int AcceleratedStartup { get; set; }
        public bool Bloodlust { get; set; }
        public bool DontRestartEliminatedPlayers { get; set; }
        public int Gender { get; set; }
        public Rules Rules { get; set; }
        public int? ResourceSeed { get; set; }
        
        public int[][]? StartPositions { get; set; }
        public byte[]? TerrainData { get; set; }
        public Civilization PlayerCiv { get; set; }
        public Task<Map[]> MapTask { get; set; }
        
        public bool Started { get; set; }
        public List<Tile> StartTiles { get; } = new();
        public int PropLand { get; set; } = 1;
        public int Landform { get; set; } = 1;
        public int Climate { get; set; } = 1;
        public int Temperature { get; set; } = 1;
        public int Age { get; set; } = 1;
        public bool CustomizeWorld { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public ILookup<int,LeaderDefaults> GroupedTribes { get; set; }
        public bool QuickStart { get; set; }

        // For scenarios
        public bool IsScenario { get; set; } = false;
        public int TechParadigm { get; set; }
        public string ScenarioName { get; set; }
        public int ScenPlayerCivId { get; set; }
        public string[] CivNames { get; set; }
        public int[] CivGenders { get; set; }
        public string[] LeaderNames { get; set; }
        public bool[] CivsInPlay { get; set; }
        public string LeaderName { get; set; }
        public int StartingYear { get; set; }
        public int TurnYearIncrement { get; set; }
        public int MaxTurns { get; set; }
    }
}