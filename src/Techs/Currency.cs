using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Currency : BaseTech
    {
        public Currency() : base(4, 1, TechType.BronzeWork, TechType.None, 0, 1)
        {
            Type = TechType.Currency;
            Name = "Currency";
        }
    }
}
