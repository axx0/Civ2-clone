using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Conscript : BaseTech
    {
        public Conscript() : base(7, -1, TechType.Democracy, TechType.Metallurgy, 2, 0)
        {
            Type = TechType.Conscript;
            Name = "Conscription";
        }
    }
}
