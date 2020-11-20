using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Invention : BaseTech
    {
        public Invention() : base(6, 0, TechType.Engineering, TechType.Literacy, 1, 4)
        {
            Type = TechType.Invention;
            Name = "Invention";
        }
    }
}
