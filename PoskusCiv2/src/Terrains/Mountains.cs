using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Mountains : BaseTerrain
    {
        public Mountains(SpecialType SpecType)
        {
            Type = TerrainType.Mountains;
            Name = "Mountains";
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 3;
                        Defense = 6;
                        Food = 0;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Gold:
                    {
                        SpecName = "Gold";
                        Movecost = 3;
                        Defense = 6;
                        Food = 0;
                        Shields = 1;
                        Trade = 6;
                        break;
                    }
                case SpecialType.Iron:
                    {
                        SpecName = "Iron";
                        Movecost = 3;
                        Defense = 6;
                        Food = 0;
                        Shields = 4;
                        Trade = 0;
                        break;
                    }
            }
            CanIrrigate = false;
            IrrigationResult = null;
            IrrigationBonus = 1;
            TurnsToIrrigate = 10;
            AIirrigation = 0;
            CanMine = true;
            MiningResult = null;
            MiningBonus = 1;
            TurnsToMine = 10;
            AImining = 6;
            TransformResult = TerrainType.Hills;
        }
    }
}
