using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Hills : BaseTerrain
    {
        public Hills() : base(2, 4, 1, 0, 0, true, 1, 10, 0, true, 3, 10, 1, TerrainType.Plains)
        {
            Type = TerrainType.Hills;
            Name = "Hills";            
        }
    }
}
