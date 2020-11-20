using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Recycling : BaseTech
    {
        public Recycling() : base(2, 1, TechType.MassProd, TechType.Democracy, 3, 2)
        {
            Type = TechType.Recycling;
            Name = "Recycling";
        }
    }
}
