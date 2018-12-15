using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Forest : BaseTerrain
    {
        public Forest(SpecialType SpecType)
        {
            Type = TerrainType.Forest;
            Name = "Forest";
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 2;
                        Defense = 3;
                        Food = 1;
                        Shields = 2;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Pheasant:
                    {
                        SpecName = "Pheasant";
                        Movecost = 2;
                        Defense = 3;
                        Food = 3;
                        Shields = 2;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Silk:
                    {
                        SpecName = "Silk";
                        Movecost = 2;
                        Defense = 3;
                        Food = 1;
                        Shields = 2;
                        Trade = 3;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = TerrainType.Plains;
            IrrigationBonus = 0;
            TurnsToIrrigate = 5;
            AIirrigation = 5;
            CanMine = false;
            MiningResult = null;
            MiningBonus = 0;
            TurnsToMine = 5;
            AImining = 0;
            TransformResult = TerrainType.Grassland;
        }
    }
}
