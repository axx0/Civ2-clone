using System;
using System.Collections.Generic;
using Civ2engine;
using Eto.Drawing;

namespace EtoFormsUI.Initialization
{
    internal class GameInitializationConfig
    {
        public Random Random { get; set; } = new ();
        public Ruleset RuleSet { get; set; }
        
        public Dictionary<string,PopupBox> PopUps { get; set; }
        public int[] WorldSize { get; set; }
        public int DifficultlyLevel { get; set; }
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
        public int ResourceSeed { get; set; }
        public Point[] StartPositions { get; set; }
        public byte[] TerrainData { get; set; }
        public int MapArea { get; set; }
        public Civilization PlayerCiv { get; set; }
    }
}