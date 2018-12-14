using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class DV_Workshop : BaseImprovement
    {
        public DV_Workshop() : base()
        {
            WType = WonderType.DV_Workshop;
            Name = "Da Vinci's Workshop";
        }
    }
}
