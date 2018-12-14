using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Physics : BaseTech
    {
        public Physics() : base(4, -1, TechType.Navigation, TechType.Literacy, 1, 3)
        {
            Type = TechType.Physics;
            Name = "Physics";
        }
    }
}
