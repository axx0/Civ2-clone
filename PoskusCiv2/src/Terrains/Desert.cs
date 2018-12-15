using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Desert : BaseTerrain
    {
        public Desert() : base(1, 2, 0, 1, 0, true, 1, 5, 5, true, 1, 5, 3, TerrainType.Plains)
        {
            Type = TerrainType.Desert;
            Name = "Desert";
        }
    }
}
