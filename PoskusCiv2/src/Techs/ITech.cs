using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    public interface ITech
    {
        int Id { get; }
        string Name { get; set; }
        TechType Type { get; set; }
    }
}
