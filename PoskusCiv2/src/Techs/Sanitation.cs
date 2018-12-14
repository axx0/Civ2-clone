using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Sanitation : BaseTech
    {
        public Sanitation() : base(4, 2, TechType.Medicine, TechType.Engineering, 2, 1)
        {
            Type = TechType.Sanitation;
            Name = "Sanitation";
        }
    }
}
