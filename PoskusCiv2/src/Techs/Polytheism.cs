using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Polytheism : BaseTech
    {
        public Polytheism() : base(4, 0, TechType.CeremBurial, TechType.HorsebackRid, 0, 2)
        {
            Type = TechType.Polytheism;
            Name = "Polytheism";
        }
    }
}
