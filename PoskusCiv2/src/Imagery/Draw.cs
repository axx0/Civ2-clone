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
            Bitmap map = new Bitmap(4 * 64, 4 * 32);    //define a bitmap for drawing map

            Squares square = new Squares();

            int x = 2 * city.X + (city.Y % 2);  //first convert coordinates to civ2-style
            int y = city.Y;
            using (Graphics graphics = Graphics.FromImage(map))
            {
                for (int col = 0; col < 4; col++)
                {
                    for (int row = 0; row < 7; row++)
                    {
                        int x_ = x - 3 + 2 * col + (y % 2);
                        int y_ = y - 3 + row;
                        if (!((row == 0 & col == 0) || (row == 0 & col == 3) || (row == 6 & col == 0) || (row == 6 & col == 3) || (col == 3 & (row == 1 || row == 3 || row == 5))))
                        {
                            graphics.DrawImage(square.Terrain((x_ - (y_ % 2)) / 2, y_), 64 * col + 32 * (row % 2), 16 * row);
                        }
                    }
                }
                //int x_, y_;
                //for (int stej = 0; stej < 1; stej++)
                //{
                //    x_ = x - 2 + stej;
                //    y_ = y + 2 - stej;
                //    //if (x_ >= 0 && x_ < 2 * Game.Data.MapXdim && y_ >= 0 && y_ < Game.Data.MapYdim)
                //    //{
                //        graphics.DrawImage(square.Terrain((x_ - (y_ % 2)) / 2, y_), x_ * 32, y_ * 16);
                //    //}
                //}
                ////graphics.DrawImage(square.Terrain(0, 0), 0 * 32, 0 * 16);
            }
            

            return map;

        }
    }
}
