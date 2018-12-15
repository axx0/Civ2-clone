using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Swamp : BaseTerrain
    {
        public Swamp() : base(2, 3, 1, 0, 0, TerrainType.Grassland, 0, 15, 6, TerrainType.Forest, 0, 15, 0, TerrainType.Plains)
        {
            Type = TerrainType.Swamp;
            Name = "Swamp";            
        }
    }
}
