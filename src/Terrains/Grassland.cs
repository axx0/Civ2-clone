using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Grassland : BaseTerrain
    {
        public Grassland(SpecialType spec)
        {
            Type = TerrainType.Grassland;
            Name = "Grassland";
            SpecType = spec;
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 1;
                        Defense = 2;
                        Food = 2;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
                case SpecialType.GrasslandShield:
                    {
                        SpecName = "Shield";
                        Movecost = 1;
                        Defense = 2;
                        Food = 2;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = null;
            IrrigationBonus = 1;
            TurnsToIrrigate = 5;
            AIirrigation = 2;
            CanMine = true;
            MiningResult = TerrainType.Forest;
            MiningBonus = 0;
            TurnsToMine = 10;
            AImining = 0;
            TransformResult = TerrainType.Hills;
        }
    }
}
