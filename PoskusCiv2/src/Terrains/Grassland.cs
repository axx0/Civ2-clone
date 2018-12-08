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
        public Grassland() : base(2, 0, 0, 1, 0, 0, 0, 0, 0, 5, 10)
        {
            Type = TerrainType.Grassland;
            Name = "Grassland";            
        }
    }
}
