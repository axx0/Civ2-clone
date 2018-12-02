using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Terrains
{
    internal class Game : BaseTerrain
    {
        public Game() : base(1, 0, 0, 1, 0, 0, 0, 0, 0, 10, 0)
        {
            Type = TerrainType.Game;
            Name = "Tundra";
            SpecialName = "Game";
        }
    }
}
