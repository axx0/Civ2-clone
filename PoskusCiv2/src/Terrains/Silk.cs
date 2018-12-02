using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Silk : BaseTerrain
    {
        public Silk() : base(1, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5)
        {
            Type = TerrainType.Silk;
            Name = "Forest";
            SpecialName = "Silk";
        }
    }
}
