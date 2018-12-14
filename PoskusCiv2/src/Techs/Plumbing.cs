using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Plumbing : BaseTech
    {
        public Plumbing() : base(4, 0, TechType.Construct, TechType.Pottery, 1, 4)
        {
            Type = TechType.Plumbing;
            Name = "Plumbing";
        }
    }
}
