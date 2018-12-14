using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Literacy : BaseTech
    {
        public Literacy() : base(5, 2, TechType.Writing, TechType.CodeLaws, 0, 3)
        {
            Type = TechType.Literacy;
            Name = "Literacy";
        }
    }
}
