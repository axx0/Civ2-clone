using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PoskusCiv2.Imagery
{
    class Draw
    {
        //Draw entire game map
        public Bitmap DrawMap()
        {
            Bitmap map = new Bitmap(Game.Data.MapXdim * 64, Game.Data.MapYdim * 32);    //define a bitmap for drawing map

            Squares square = new Squares();
                       
            using (Graphics graphics = Graphics.FromImage(map))
            {
                for (int col = 0; col < Game.Data.MapXdim; col++)
                {
                    for (int row = 0; row < Game.Data.MapYdim; row++)
                    {
                        graphics.DrawImage(square.Terrain(col, row), 64 * col + 32 * (row % 2) + 1, 16 * row + 1);
                    }
                }
            }
            
            return map;
        }

        //Draw terrain in city form
        public Bitmap DrawCityFormMap(City city)
        {
            Bitmap map = new Bitmap(1 * 64, 1 * 32);    //define a bitmap for drawing map

            Squares square = new Squares();

            using (Graphics graphics = Graphics.FromImage(map))
            {
                graphics.DrawImage(square.Terrain(city.X, city.Y), 0, 0);
            }

            return map;

        }
    }
}
