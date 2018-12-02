using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Whales : BaseTerrain
    {
        public Whales() : base(1, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0)
        {
            Type = TerrainType.Whales;
            Name = "Ocean";
            SpecialName = "Whales";
        }
    }
}
