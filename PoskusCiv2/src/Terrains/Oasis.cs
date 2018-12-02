using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Oasis : BaseTerrain
    {
        public Oasis() : base(0, 1, 0, 1, 0, 0, 0, 1, 0, 5, 5)
        {
            Type = TerrainType.Oasis;
            Name = "Desert";
            SpecialName = "Oasis";
        }
    }
}
