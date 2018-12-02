using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Coal : BaseTerrain
    {
        public Coal() : base(1, 0, 0, 1, 0, 0, 0, 3, 0, 10, 10)
        {
            Type = TerrainType.Coal;
            Name = "Hills";
            SpecialName = "Coal";
        }
    }
}
