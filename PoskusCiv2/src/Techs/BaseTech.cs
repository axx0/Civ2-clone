using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class BaseTech : ITech
    {
        public string Name { get; set; }
        public TechType Type { get; set; }
        public int Id => (int)Type;

        protected BaseTech(int AIvalue, int modifier, TechType preq1, TechType preq2, int epoch, int category)
        {
        }
    }
}
