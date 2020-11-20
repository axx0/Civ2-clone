using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class IN_College : BaseImprovement
    {
        public IN_College() : base()
        {
            WType = WonderType.IN_College;
            Name = "Isaac Newton's College";
        }
    }
}
