using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Pheasant : BaseTerrain
    {
        public Pheasant() : base(1, 2, 0, 0, 0, 0, 0, 0, 0, 5, 5)
        {
            Type = TerrainType.Pheasant;
            Name = "Forest";
            SpecialName = "Pheasant";
        }
    }
}
