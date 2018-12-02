using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class GlacierOil : BaseTerrain
    {
        public GlacierOil() : base(0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 15)
        {
            Type = TerrainType.GlacierOil;
            Name = "Glacier";
            SpecialName = "Oil";
        }
    }
}
