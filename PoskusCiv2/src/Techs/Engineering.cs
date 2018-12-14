using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Engineering : BaseTech
    {
        public Engineering() : base(4, 0, TechType.Wheel, TechType.Construct, 0, 4)
        {
            Type = TechType.Engineering;
            Name = "Engineering";
        }
    }
}
