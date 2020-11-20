using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class BaseImprovement : IImprovement
    {
        public string Name { get; set; }
        public ImprovementType Type { get; set; }
        public WonderType WType { get; set; }
        public int Id => (int)Type;
        public int WId => (int)WType;

        protected BaseImprovement()
        {

        }
    }
}
