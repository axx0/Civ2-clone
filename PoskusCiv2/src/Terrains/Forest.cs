using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Forest : BaseTerrain
    {
        public Forest() : base(2, 3, 1, 2, 0, TerrainType.Plains, 0, 5, 5, false, 0, 5, 0, TerrainType.Grassland)
        {
            Type = TerrainType.Forest;
            Name = "Forest";
        }
    }
}
