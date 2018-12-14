using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Mathematics : BaseTech
    {
        public Mathematics() : base(4, -1, TechType.Alphabet, TechType.Masonry, 0, 3)
        {
            Type = TechType.Mathematics;
            Name = "Mathematics";
        }
    }
}
