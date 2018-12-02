using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Gold : BaseTerrain
    {
        public Gold() : base(0, 1, 0, 1, 0, 0, 0, 1, 0, 10, 10)
        {
            Type = TerrainType.Gold;
            Name = "Mountains";
            SpecialName = "Gold";
        }
    }
}
