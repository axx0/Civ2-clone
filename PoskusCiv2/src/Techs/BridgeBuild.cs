using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class BridgeBuild : BaseTech
    {
        public BridgeBuild() : base(4, 0, TechType.IronWorking, TechType.Construct, 0, 4)
        {
            Type = TechType.BridgeBuild;
            Name = "Bridge Building";
        }
    }
}
