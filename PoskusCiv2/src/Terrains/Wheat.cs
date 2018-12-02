using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Wheat : BaseTerrain
    {
        public Wheat() : base(1, 1, 0, 1, 0, 0, 0, 0, 0, 5, 15)
        {
            Type = TerrainType.Wheat;
            Name = "Plains";
            SpecialName = "Wheat";
        }
    }
}
