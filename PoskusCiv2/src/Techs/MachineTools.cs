using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class MachineTools : BaseTech
    {
        public MachineTools() : base(4, -2, TechType.Steel, TechType.Tactics, 2, 4)
        {
            Type = TechType.MachineTools;
            Name = "Machine Tools";
        }
    }
}
