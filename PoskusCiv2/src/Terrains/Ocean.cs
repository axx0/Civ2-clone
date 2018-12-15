using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Ocean : BaseTerrain
    {
        public Ocean(SpecialType SpecType)
        {
            Type = TerrainType.Ocean;
            Name = "Ocean";
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 1;
                        Defense = 2;
                        Food = 1;
                        Shields = 0;
                        Trade = 2;
                        break;
                    }
                case SpecialType.Fish:
                    {
                        SpecName = "Fish";
                        Movecost = 1;
                        Defense = 2;
                        Food = 3;
                        Shields = 0;
                        Trade = 2;
                        break;
                    }
                case SpecialType.Whales:
                    {
                        SpecName = "Whales";
                        Movecost = 1;
                        Defense = 2;
                        Food = 2;
                        Shields = 2;
                        Trade = 3;
                        break;
                    }
            }
            CanIrrigate = false;
            IrrigationResult = null;
            IrrigationBonus = 0;
            TurnsToIrrigate = 0;
            AIirrigation = 0;
            CanMine = false;
            MiningResult = null;
            MiningBonus = 0;
            TurnsToMine = 0;
            AImining = 0;
            TransformResult = null;
        }
    }
}
