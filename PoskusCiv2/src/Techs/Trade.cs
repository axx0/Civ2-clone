using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Trade : BaseTech
    {
        public Trade() : base(4, 2, TechType.Currency, TechType.CodeLaws, 0, 1)
        {
            Type = TechType.Trade;
            Name = "Trade";
        }
    }
}
