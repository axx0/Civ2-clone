using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class AmphWarfare : BaseTech
    {
        public AmphWarfare() : base(3, -2, TechType.Navigation, TechType.Tactics, 3, 0)
        {
            Type = TechType.AmphWarfare;
            Name = "Amphibious Warfare";
        }
    }
}
