using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Alphabet : BaseTech
    {
        public Alphabet() : base(5, 1, TechType.None, TechType.None, 0, 3)
        {
            Type = TechType.Alphabet;
            Name = "Alphabet";
        }
    }
}
