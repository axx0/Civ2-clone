using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Plains : BaseTerrain
    {
        public Plains() : base(1, 2, 1, 1, 0, true, 1, 5, 1, TerrainType.Forest, 0, 15, 0, TerrainType.Grassland)
        {
            Type = TerrainType.Plains;
            Name = "Plains";
        }
    }
}
