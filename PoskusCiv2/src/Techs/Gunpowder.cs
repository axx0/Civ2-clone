using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Gunpowder : BaseTech
    {
        public Gunpowder() : base(8, -2, TechType.Invention, TechType.IronWorking, 1, 0)
        {
            Type = TechType.Gunpowder;
            Name = "Gunpowder";
        }
    }
}
