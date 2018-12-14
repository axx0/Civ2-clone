using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Radio : BaseTech
    {
        public Radio() : base(5, -1, TechType.Flight, TechType.Electricity, 3, 4)
        {
            Type = TechType.Radio;
            Name = "Radio";
        }
    }
}
