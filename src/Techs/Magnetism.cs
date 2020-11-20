using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Magnetism : BaseTech
    {
        public Magnetism() : base(4, -1, TechType.Physics, TechType.IronWorking, 1, 3)
        {
            Type = TechType.Magnetism;
            Name = "Magnetism";
        }
    }
}
