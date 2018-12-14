using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Espionage : BaseTech
    {
        public Espionage() : base(2, -1, TechType.Communism, TechType.Democracy, 3, 0)
        {
            Type = TechType.Espionage;
            Name = "Espionage";
        }
    }
}
