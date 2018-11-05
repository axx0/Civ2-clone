using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class Marketplace : BaseImprovement
    {
        public Marketplace() : base()
        {
            Type = ImprovementType.Marketplace;
            Name = "Marketplace";
        }
    }
}
