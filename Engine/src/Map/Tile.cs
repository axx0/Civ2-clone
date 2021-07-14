
        using System;
        using System.Collections.Generic;
        using System.Drawing;
        using System.Drawing.Text;
        using System.Linq;
using Civ2engine.Enums;
        using Civ2engine.Units;

        namespace Civ2engine.Terrains
{
    public class Tile : ITerrain, IMapItem
    {
        public int X { get; }
        public int Y { get; }

        public int Odd { get; }
        public Terrain Terrain { get; internal set; }

        public TerrainType Type => Terrain.Type;

        public int special { get; } = -1;


        // Get special resource type based on map seed & tile location
        public Tile(int x, int y, Terrain terrain, int seed)
        {
            // Courtesy of Civfanatics
            // https://forums.civfanatics.com/threads/is-there-really-no-way-to-do-this-add-resources-on-map.518649/#post-13002282
            X = x;
            Y = y;
            Odd = y % 2;
            Terrain = terrain;

            HasShield = HasSheild();

            var a = (X + Y) >> 1;
            var b = X - a;
            var c = 13 * (b >> 2) + 11 * ((X + Y) >> 3) + seed;
            if ((a & 3) + 4 * (b & 3) != (c & 15)) return;

            var d = 1 << ((seed >> 4) & 3);
            special = (d & a) == (d & b) ? 1 : 0;
        }

        public bool HasShield { get; }

        private bool HasSheild()
        {
            // Formula for determining where grassland shield is based on X-Y coords
            int rez4 = (Y / 2 + 2 * (Y % 2)) % 4;
            int rez3 = 8 - 2 * (rez4 % 4);

            int rez = (X - (Y % 2) + rez3) % 8;
            return rez < 4;
        }

        // From RULES.TXT
        public string Name => Terrain.Name;


        public int MoveCost => Terrain.MoveCost;
        public int Defense => (River ? Terrain.Defense + 1 : Terrain.Defense) / 2;
        public int Food => Irrigation ? Terrain.Food + Terrain.IrrigationBonus : Terrain.Food;
        public int Shields => Mining ? Terrain.Shields + Terrain.MiningBonus : Terrain.Shields;
        public int Trade => River ? Terrain.Trade + 1 : Terrain.Trade;
        public bool CanBeIrrigated => Terrain.CanIrrigate != -2 && (!Irrigation ||!Farmland);  // yes meaning the result can be irrigation or transform. of terrain
        
        /// <summary>
        /// If result == type of terrain before irrigation, this means that it's regular irrigation.
        /// (If it can actually be irrigated is determined by CanBeIrrigated.)
        /// Otherwise the result is the type of terrain which is formed.
        /// </summary>
        public TerrainType IrrigationResult => Terrain.CanIrrigate < 0 ? Type : (TerrainType) Terrain.CanIrrigate;

        public GovernmentType MinGovrnLevelAItoPerformIrrigation { get; set; }   // Be careful, 0=never!
        public bool CanBeMined => Terrain.CanMine != -2 && (!Mining);  // yes meaning the result can be mining or transform. of terrain
        
        /// <summary>
        /// If result == type of terrain before mining, this means that it's regular mine.
        /// (If it can actually be mined is determined by CanBeMined.)
        /// Otherwise the result is the type of terrain which is formed.
        /// </summary>
        public TerrainType MiningResult => Terrain.CanMine < 0 ? Type : (TerrainType) Terrain.CanMine;
        public int MiningBonus { get; set; }
        public int TurnsToMine { get; set; }
        public GovernmentType MinGovrnLevelAItoPerformMining { get; set; }     // Be careful, 0=never!
        public bool CanBeTransformed => Terrain.Transform != -2 ; // usually only ocean can't be transformed

        /// <summary>
        ///  If result == type of terrain before transformation, it means it can't be transformed.
        /// Otherwise the result is the type of terrain which is transformed.
        /// </summary>
        public TerrainType TransformResult => Terrain.Transform < 0 ? Type : (TerrainType) Terrain.Transform;


        // TODO: put special resources logic into here

        public bool Resource { get; set; }
        public bool River { get; set; }
        public bool IsUnitPresent => UnitsHere.Count > 0;
        public bool IsCityPresent => CityHere != null;
        public bool Irrigation { get; set; }
        public bool Mining { get; set; }
        public bool Road { get; set; }
        public bool Railroad { get; set; }
        public bool Fortress { get; set; }
        public bool Pollution { get; set; }
        public bool Farmland { get; set; }
        public bool Airbase { get; set; }
        public int Island { get; set; }
        public string Hexvalue { get; set; }
        public Bitmap Graphic { get; set; }
        public decimal Fertility { get; set; }
        public bool[] Visibility { get; set; }

        public List<Unit> UnitsHere { get; } = new();
        
        public City CityHere { get; set; }

        public IUnit GetTopUnit(Func<Unit, bool> pred = null)
        {
            var units = pred != null ? UnitsHere.Where(pred) : UnitsHere;
            return (Terrain.Type == TerrainType.Ocean
                    ? units.OrderByDescending(u => u.Domain == UnitGAS.Sea ? 1 : 0)
                    : units.OrderByDescending(u => u.Domain == UnitGAS.Sea ? 0 : 1))
                .ThenBy(u => u.AttackBase).First();
        }
    }
}