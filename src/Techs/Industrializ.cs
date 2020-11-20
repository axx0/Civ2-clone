using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Industrializ : BaseTech
    {
        public Industrializ() : base(6, 0, TechType.Railroad, TechType.Banking, 2, 1)
        {
            Type = TechType.Industrializ;
            Name = "Industrialization";
        }
    }
}
