using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Techs
{
    internal class SteamEngine : BaseTech
    {
        public SteamEngine() : base(4, -1, TechType.Philosophy, TechType.Invention, 2, 3)
        {
            Type = TechType.SteamEngine;
            Name = "Steam Engine";
        }
    }
}
