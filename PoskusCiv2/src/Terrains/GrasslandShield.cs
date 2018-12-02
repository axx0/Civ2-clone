using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class GrasslandShield : BaseTerrain
    {
        public GrasslandShield() : base(2, 0, 0, 1, 0, 0, 0, 0, 0, 5, 10)
        {
            Type = TerrainType.GrasslandShield;
            Name = "Grassland";
            SpecialName = "Shield";
        }
    }
}
