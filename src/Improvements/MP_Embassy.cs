using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class MP_Embassy : BaseImprovement
    {
        public MP_Embassy() : base()
        {
            WType = WonderType.MP_Embassy;
            Name = "Marco Polo's Embassy";
        }
    }
}
