using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Medicine : BaseTech
    {
        public Medicine() : base(4, 0, TechType.Philosophy, TechType.Trade, 1, 1)
        {
            Type = TechType.Medicine;
            Name = "Medicine";
        }
    }
}
