using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Fanatics : BaseUnit
    {
        public Fanatics() : base(20, 4, 4, 2, 1, 1)
        {
            Type = UnitType.Fanatics;
            Name = "Fanatics";
        }
    }
}
