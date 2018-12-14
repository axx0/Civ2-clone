using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class University : BaseTech
    {
        public University() : base(5, 1, TechType.Mathematics, TechType.Philosophy, 1, 3)
        {
            Type = TechType.University;
            Name = "University";
        }
    }
}
