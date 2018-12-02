using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Gems : BaseTerrain
    {
        public Gems() : base(1, 0, 0, 0, 0, 0, 0, 0, 0, 15, 15)
        {
            Type = TerrainType.Gems;
            Name = "Jungle";
            SpecialName = "Gems";
        }
    }
}
