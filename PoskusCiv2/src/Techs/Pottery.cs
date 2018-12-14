using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Pottery : BaseTech
    {
        public Pottery() : base(4, 1, TechType.None, TechType.None, 0, 1)
        {
            Type = TechType.Pottery;
            Name = "Pottery";
        }
    }
}
