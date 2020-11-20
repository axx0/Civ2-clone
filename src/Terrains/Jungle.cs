using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Jungle : BaseTerrain
    {
        public Jungle(SpecialType spec)
        {
            Type = TerrainType.Jungle;
            Name = "Jungle";
            SpecType = spec;
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 2;
                        Defense = 3;
                        Food = 1;
                        Shields = 0;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Gems:
                    {
                        SpecName = "Gems";
                        Movecost = 2;
                        Defense = 3;
                        Food = 1;
                        Shields = 0;
                        Trade = 4;
                        break;
                    }
                case SpecialType.Fruit:
                    {
                        SpecName = "Fruit";
                        Movecost = 2;
                        Defense = 3;
                        Food = 4;
                        Shields = 0;
                        Trade = 1;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = TerrainType.Grassland;
            IrrigationBonus = 0;
            TurnsToIrrigate = 15;
            AIirrigation = 6;
            CanMine = true;
            MiningResult = TerrainType.Forest;
            MiningBonus = 0;
            TurnsToMine = 15;
            AImining = 0;
            TransformResult = TerrainType.Plains;
        }
    }
}
