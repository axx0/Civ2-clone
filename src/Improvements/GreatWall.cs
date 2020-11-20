using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class GreatWall : BaseImprovement
    {
        public GreatWall() : base()
        {
            WType = WonderType.GreatWall;
            Name = "Great Wall";
        }
    }
}
