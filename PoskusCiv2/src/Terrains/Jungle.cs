using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Jungle : BaseTerrain
    {
        public Jungle() : base(1, 0, 0, 1, 0, 4, 4, 0, 1, 0, 0, 0, 0, 0, 0, 15, 15)
        {
            Type = TerrainType.Jungle;
            Name = "Jungle";            
        }
    }
}
