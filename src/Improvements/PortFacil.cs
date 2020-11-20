using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class PortFacil : BaseImprovement
    {
        public PortFacil() : base()
        {
            Type = ImprovementType.PortFacil;
            Name = "Port Facility";
        }
    }
}
