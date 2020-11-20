﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Units
{
    internal class Settlers : BaseUnit
    {
        public Settlers() : base(40, 5, 4, 2, 1, 1)
        {
            Type = UnitType.Settlers;
            GAS = UnitGAS.Air;
            Name = "Settlers";
        }
    }
}