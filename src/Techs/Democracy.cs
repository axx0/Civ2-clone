using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Democracy : BaseTech
    {
        public Democracy() : base(5, 1, TechType.Banking, TechType.Invention, 2, 2)
        {
            Type = TechType.Democracy;
            Name = "Democracy";
        }
    }
}
