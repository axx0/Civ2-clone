using System;
using System.Drawing;
using System.Linq;
using civ2.Bitmaps;
using civ2.Enums;

namespace civ2.Terrains
{
    internal class Terrain : BaseInstance, ITerrain
    {
        public int X { get; set; }
        public int Y { get; set; }
        public TerrainType Type { get; set; }
        public SpecialType? SpecType { get; set; }

        // From RULES.TXT
        public string Name => Game.Rules.TerrainName[(int)Type];
        public int MoveCost => Game.Rules.TerrainMovecost[(int)Type];
        public int Defense => Game.Rules.TerrainDefense[(int)Type];
        public int Food => Game.Rules.TerrainFood[(int)Type];
        public int Shields => Game.Rules.TerrainShields[(int)Type];
        public int Trade => Game.Rules.TerrainTrade[(int)Type];
        public bool CanBeIrrigated => Game.Rules.TerrainCanIrrigate[(int)Type] != "no";  // yes meaning the result can be irrigation or transform. of terrain
        public TerrainType IrrigationResult 
        {
            get 
            {
                // If result == type of terrain before irrigation, this means that it's regular irrigation.
                // (If it can actually be irrigated is determined by CanBeIrrigated.)
                if (Game.Rules.TerrainCanIrrigate[(int)Type] == "yes" || Game.Rules.TerrainCanIrrigate[(int)Type] == "no")
                    return Type;
                // Otherwise the result is the type of terrain which is formed.
                else
                    return (TerrainType)Array.IndexOf(Game.Rules.TerrainShortName, Game.Rules.TerrainCanIrrigate[(int)Type]);
            }
        }
        public int IrrigationBonus => Game.Rules.TerrainIrrigateBonus[(int)Type];
        public int TurnsToIrrigate => Game.Rules.TerrainIrrigateTurns[(int)Type];
        public GovernmentType MinGovrnLevelAItoPerformIrrigation => (GovernmentType)Game.Rules.TerrainIrrigateAI[(int)Type];     // Be careful, 0=never!
        public bool CanBeMined => Game.Rules.TerrainCanMine[(int)Type] != "no";  // yes meaning the result can be mining or transform. of terrain
        public TerrainType MiningResult
        {
            get
            {
                // If result == type of terrain before mining, this means that it's regular mine.
                // (If it can actually be mined is determined by CanBeMined.)
                if (Game.Rules.TerrainCanMine[(int)Type] == "yes" || Game.Rules.TerrainCanMine[(int)Type] == "no")
                    return Type;
                // Otherwise the result is the type of terrain which is formed.
                else
                    return (TerrainType)Array.IndexOf(Game.Rules.TerrainShortName, Game.Rules.TerrainCanMine[(int)Type]);
            }
        }
        public int MiningBonus => Game.Rules.TerrainMineBonus[(int)Type];
        public int TurnsToMine => Game.Rules.TerrainMineTurns[(int)Type];
        public GovernmentType MinGovrnLevelAItoPerformMining => (GovernmentType)Game.Rules.TerrainMineAI[(int)Type];     // Be careful, 0=never!
        public bool CanBeTransformed => Game.Rules.TerrainTransform[(int)Type] != "no"; // usually only ocean can't be transformed
        public TerrainType TransformResult
        {
            get
            {
                // If result == type of terrain before transformation, it means it can't be transformed.
                if (Game.Rules.TerrainTransform[(int)Type] == "no")
                    return Type;
                // Otherwise the result is the type of terrain which is transformed.
                else
                    return (TerrainType)Array.IndexOf(Game.Rules.TerrainShortName, Game.Rules.TerrainTransform[(int)Type]);
            }
        }

        // TODO: put special resources logic into here
        public string SpecName => Game.Rules.TerrainSpec1Name[(int)SpecType];



        



        public bool Resource { get; set; }
        public bool River { get; set; }
        public bool UnitPresent => Game.GetUnits.Any(u => u.X == X && u.Y == Y);
        public bool CityPresent => Game.GetCities.Any(c => c.X == X && c.Y == Y);
        public bool Irrigation { get; set; }
        public bool Mining { get; set; }
        public bool Road { get; set; }
        public bool Railroad { get; set; }
        public bool Fortress { get; set; }
        public bool Pollution { get; set; }
        public bool Farmland { get; set; }
        public bool Airbase { get; set; }
        public int Island { get; set; }
        public bool[] Visibility { get; set; }
        public string Hexvalue { get; set; }

        public Bitmap Graphic { get; set; }

    }
}
