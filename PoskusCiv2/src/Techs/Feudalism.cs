using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Feudalism : BaseTech
    {
        public Feudalism() : base(4, -1, TechType.WarriorCode, TechType.Monoteism, 0, 0)
        {
            Type = TechType.Feudalism;
            Name = "Feudalism";
        }
    }
}
