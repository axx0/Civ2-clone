using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Economics : BaseTech
    {
        public Economics() : base(4, 1, TechType.University, TechType.Banking, 2, 1)
        {
            Type = TechType.Economics;
            Name = "Economics";
        }
    }
}
