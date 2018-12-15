using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Plains : BaseTerrain
    {
        public Plains(SpecialType SpecType)
        {
            Type = TerrainType.Plains;
            Name = "Plains";
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 1;
                        Defense = 2;
                        Food = 1;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Buffalo:
                    {
                        SpecName = "Buffalo";
                        Movecost = 1;
                        Defense = 2;
                        Food = 1;
                        Shields = 3;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Wheat:
                    {
                        SpecName = "Wheat";
                        Movecost = 1;
                        Defense = 2;
                        Food = 3;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = null;
            IrrigationBonus = 1;
            TurnsToIrrigate = 5;
            AIirrigation = 1;
            CanMine = true;
            MiningResult = TerrainType.Forest;
            MiningBonus = 0;
            TurnsToMine = 15;
            AImining = 0;
            TransformResult = TerrainType.Grassland;
        }
    }
}
