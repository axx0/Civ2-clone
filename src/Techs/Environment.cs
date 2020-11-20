using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Environment : BaseTech
    {
        public Environment() : base(3, 1, TechType.Recycling, TechType.SpaceFlight, 3, 2)
        {
            Type = TechType.Environment;
            Name = "Environmentalism";
        }
    }
}
