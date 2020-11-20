using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Leadership : BaseTech
    {
        public Leadership() : base(5, -1, TechType.Chivalry, TechType.Gunpowder, 1, 0)
        {
            Type = TechType.Leadership;
            Name = "Leadership";
        }
    }
}
