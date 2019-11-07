using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2
{
    public class Resolution
    {
        public int Height;
        public int Width;
        public string Name;
        
        public Resolution(int width, int height, string name)
        {
            Width = width;
            Height = height;
            Name = name;
        }
    }
}
