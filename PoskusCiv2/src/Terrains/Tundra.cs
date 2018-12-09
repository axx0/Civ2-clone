﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Tundra : BaseTerrain
    {
        public Tundra() : base(1, 0, 0, 3, 1, 0, 2, 0, 3, 1, 0, 0, 0, 0, 0, 10, 0)
        {
            Type = TerrainType.Tundra;
            Name = "Tundra";            
        }
    }
}
