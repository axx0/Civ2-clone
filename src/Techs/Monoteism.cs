using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Monoteism : BaseTech
    {
        public Monoteism() : base(5, 1, TechType.Philosophy, TechType.Polytheism, 1, 2)
        {
            Type = TechType.Monoteism;
            Name = "Monoteism";
        }
    }
}
