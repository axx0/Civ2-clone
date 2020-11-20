using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Glacier : BaseTerrain
    {
        public Glacier(SpecialType spec)
        {
            Type = TerrainType.Glacier;
            Name = "Glacier";
            SpecType = spec;
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 2;
                        Defense = 2;
                        Food = 0;
                        Shields = 0;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Ivory:
                    {
                        SpecName = "Ivory";
                        Movecost = 2;
                        Defense = 2;
                        Food = 1;
                        Shields = 1;
                        Trade = 4;
                        break;
                    }
                case SpecialType.GlacierOil:
                    {
                        SpecName = "Glacier Oil";
                        Movecost = 2;
                        Defense = 2;
                        Food = 0;
                        Shields = 4;
                        Trade = 0;
                        break;
                    }
            }
            CanIrrigate = false;
            IrrigationResult = null;
            IrrigationBonus = 0;
            TurnsToIrrigate = 0;
            AIirrigation = 0;
            CanMine = true;
            MiningResult = null;
            MiningBonus = 1;
            TurnsToMine = 15;
            AImining = 3;
            TransformResult = TerrainType.Tundra;
        }
    }
}
