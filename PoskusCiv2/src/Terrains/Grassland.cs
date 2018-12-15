using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Grassland : BaseTerrain
    {
        public Grassland() : base(1, 2, 2, 1, 0, true, 1, 5, 2, TerrainType.Forest, 0, 10, 0, TerrainType.Hills)
        {
            Type = TerrainType.Grassland;
            Name = "Grassland";            
        }
    }
}
