using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class BaseTerrain : ITerrain
    {
        public TerrainType Type { get; set; }
        public string Name { get; set; }
        public SpecialType SpecType { get; set; }
        public string SpecName { get; set; }
        public bool Resource { get; set; }
        public bool River { get; set; }
        public bool UnitPresent { get; set; }
        public bool CityPresent { get; set; }
        public bool Irrigation { get; set; }
        public bool Mining { get; set; }
        public bool Road { get; set; }
        public bool Railroad { get; set; }
        public bool Fortress { get; set; }
        public bool Pollution { get; set; }
        public bool Farmland { get; set; }
        public bool Airbase { get; set; }
        public int Island { get; set; }

        public int Movecost { get; set; }
        public int Defense { get; set; }
        public int Food { get; set; }
        public int Shields { get; set; }
        public int Trade { get; set; }
        public bool CanIrrigate { get; set; }
        public TerrainType? IrrigationResult { get; set; }
        public int IrrigationBonus { get; set; }
        public int TurnsToIrrigate { get; set; }
        public int AIirrigation { get; set; }
        public bool CanMine { get; set; }
        public TerrainType? MiningResult { get; set; }
        public int MiningBonus { get; set; }
        public int TurnsToMine { get; set; }
        public int AImining { get; set; }
        public TerrainType? TransformResult { get; set; }
        
        public string Hexvalue { get; set; }
    }
}
