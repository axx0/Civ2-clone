using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Furs : BaseTerrain
    {
        public Furs() : base(1, 0, 0, 1, 0, 0, 0, 0, 0, 10, 0)
        {
            Type = TerrainType.Furs;
            Name = "Tundra";
            SpecialName = "Furs";
        }
    }
}
