using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class Communism : BaseTech
    {
        public Communism() : base(5, 0, TechType.Philosophy, TechType.Industrializ, 2, 2)
        {
            Type = TechType.Communism;
            Name = "Communism";
        }
    }
}
