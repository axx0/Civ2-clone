using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Tundra : BaseTerrain
    {
        public Tundra(SpecialType SpecType)
        {
            Type = TerrainType.Tundra;
            Name = "Tundra";
            switch (SpecType)
            {
                case SpecialType.NoSpecial:
                    {
                        SpecName = "";
                        Movecost = 1;
                        Defense = 2;
                        Food = 1;
                        Shields = 0;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Game:
                    {
                        SpecName = "Game";
                        Movecost = 1;
                        Defense = 2;
                        Food = 3;
                        Shields = 1;
                        Trade = 0;
                        break;
                    }
                case SpecialType.Furs:
                    {
                        SpecName = "Furs";
                        Movecost = 1;
                        Defense = 2;
                        Food = 2;
                        Shields = 0;
                        Trade = 3;
                        break;
                    }
            }
            CanIrrigate = true;
            IrrigationResult = null;
            IrrigationBonus = 1;
            TurnsToIrrigate = 10;
            AIirrigation = 0;
            CanMine = false;
            MiningResult = null;
            MiningBonus = 0;
            TurnsToMine = 0;
            AImining = 0;
            TransformResult = TerrainType.Desert;
        }
    }
}
