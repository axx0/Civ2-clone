using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Tundra : BaseTerrain
    {
        public Tundra() : base(1, 2, 1, 0, 0, true, 1, 10, 1, false, 0, 0, 0, TerrainType.Desert)
        {
            Type = TerrainType.Tundra;
            Name = "Tundra";            
        }
    }
}
