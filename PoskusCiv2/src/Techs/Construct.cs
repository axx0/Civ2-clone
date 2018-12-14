using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Construct : BaseTech
    {
        public Construct() : base(4, 0, TechType.Masonry, TechType.Currency, 0, 4)
        {
            Type = TechType.Construct;
            Name = "Construction";
        }
    }
}
