using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Desert : BaseTerrain
    {
        public Desert(SpecialType spec)
        {
            Type = TerrainType.Desert;
            Name = "Desert";
            SpecType = spec;
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 1;
                        Defense = 2;
                        Food = 0;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Oasis:
                    {
                        SpecName = "Oasis";
                        Movecost = 1;
                        Defense = 2;
                        Food = 3;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
                case SpecialType.DesertOil:
                    {
                        SpecName = "Desert Oil";
                        Movecost = 1;
                        Defense = 2;
                        Food = 0;
                        Shields = 4;
                        Trade = 0;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = null;
            IrrigationBonus = 1;
            TurnsToIrrigate = 5;
            AIirrigation = 5;
            CanMine = true;
            MiningResult = null;
            MiningBonus = 1;
            TurnsToMine = 5;
            AImining = 3;
            TransformResult = TerrainType.Plains;
        }
    }
}
