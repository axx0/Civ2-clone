using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Republic : BaseTech
    {
        public Republic() : base(5, 1, TechType.CodeLaws, TechType.Literacy, 0, 2)
        {
            Type = TechType.Republic;
            Name = "The Republic";
        }
    }
}
