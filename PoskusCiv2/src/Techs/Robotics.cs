using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Robotics : BaseTech
    {
        public Robotics() : base(5, -2, TechType.Computers, TechType.MobileWarf, 3, 0)
        {
            Type = TechType.Robotics;
            Name = "Robotics";
        }
    }
}
