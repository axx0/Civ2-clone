using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Corporat : BaseTech
    {
        public Corporat() : base(4, 0, TechType.Industrializ, TechType.Economics, 2, 1)
        {
            Type = TechType.Corporat;
            Name = "The Corporation";
        }
    }
}
