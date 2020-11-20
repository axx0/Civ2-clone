using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Fundament : BaseTech
    {
        public Fundament() : base(3, -2, TechType.Monoteism, TechType.Conscript, 2, 2)
        {
            Type = TechType.Fundament;
            Name = "Fundamentalism";
        }
    }
}
