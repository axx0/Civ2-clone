using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class IronWorking : BaseTech
    {
        public IronWorking() : base(5, -1, TechType.BronzeWork, TechType.WarriorCode, 0, 4)
        {
            Type = TechType.IronWorking;
            Name = "Iron Working";
        }
    }
}
