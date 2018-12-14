using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class MapMaking : BaseTech
    {
        public MapMaking() : base(6, -1, TechType.Alphabet, TechType.None, 0, 1)
        {
            Type = TechType.MapMaking;
            Name = "Map Making";
        }
    }
}
