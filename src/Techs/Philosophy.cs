using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Philosophy : BaseTech
    {
        public Philosophy() : base(6, 1, TechType.Mysticism, TechType.Literacy, 1, 2)
        {
            Type = TechType.Philosophy;
            Name = "Philosophy";
        }
    }
}
