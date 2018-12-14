﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class WarAcademy : BaseImprovement
    {
        public WarAcademy() : base()
        {
            WType = WonderType.WarAcademy;
            Name = "Sun Tzu's War Academy";
        }
    }
}