using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Chemistry : BaseTech
    {
        public Chemistry() : base(5, -1, TechType.University, TechType.Medicine, 1, 3)
        {
            Type = TechType.Chemistry;
            Name = "Chemistry";
        }
    }
}
