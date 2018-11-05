using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    public interface IImprovement
    {
        int Id { get; }
        string Name { get; set; }
        ImprovementType Type { get; set; }
    }
}
