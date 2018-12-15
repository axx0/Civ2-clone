using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Glacier : BaseTerrain
    {
        public Glacier() : base(2, 2, 0, 0, 0, false, 0, 0, 0, true, 1, 15, 3, TerrainType.Tundra)
        {
            Type = TerrainType.Glacier;
            Name = "Glacier";            
        }
    }
}
