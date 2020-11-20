using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Writing : BaseTech
    {
        public Writing() : base(4, 2, TechType.Alphabet, TechType.None, 0, 3)
        {
            Type = TechType.Writing;
            Name = "Writing";
        }
    }
}
