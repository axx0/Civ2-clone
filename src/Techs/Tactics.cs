using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Tactics : BaseTech
    {
        public Tactics() : base(6, -1, TechType.Conscript, TechType.Leadership, 2, 0)
        {
            Type = TechType.Tactics;
            Name = "Tactics";
        }
    }
}
