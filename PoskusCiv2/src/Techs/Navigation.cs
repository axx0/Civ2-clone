using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Navigation : BaseTech
    {
        public Navigation() : base(6, -1, TechType.Seafaring, TechType.Astronomy, 1, 1)
        {
            Type = TechType.Navigation;
            Name = "Navigation";
        }
    }
}
