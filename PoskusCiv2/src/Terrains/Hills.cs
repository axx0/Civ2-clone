using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Hills : BaseTerrain
    {
        public Hills(SpecialType spec)
        {
            Type = TerrainType.Hills;
            Name = "Hills";
            SpecType = spec;
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 2;
                        Defense = 4;
                        Food = 1;
                        Shields = 0;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Coal:
                    {
                        SpecName = "Coal";
                        Movecost = 2;
                        Defense = 4;
                        Food = 1;
                        Shields = 2;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Wine:
                    {
                        SpecName = "Wine";
                        Movecost = 2;
                        Defense = 4;
                        Food = 1;
                        Shields = 0;
                        Trade = 4;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = null;
            IrrigationBonus = 1;
            TurnsToIrrigate = 10;
            AIirrigation = 0;
            CanMine = true;
            MiningResult = null;
            MiningBonus = 3;
            TurnsToMine = 10;
            AImining = 1;
            TransformResult = TerrainType.Plains;
        }
    }
}
