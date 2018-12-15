using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Mountains : BaseTerrain
    {
        public Mountains() : base(3, 6, 0, 1, 0, false, 1, 10, 0, true, 1, 10, 6, TerrainType.Hills)
        {
            Type = TerrainType.Mountains;
            Name = "Mountains";            
        }
    }
}
