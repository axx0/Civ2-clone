using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Theology : BaseTech
    {
        public Theology() : base(3, 2, TechType.Monoteism, TechType.Feudalism, 1, 2)
        {
            Type = TechType.Theology;
            Name = "Theology";
        }
    }
}
