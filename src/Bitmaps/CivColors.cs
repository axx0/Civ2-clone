using System.Drawing;

namespace civ2.Bitmaps
{
    public static class CivColors
    {
        public static readonly Color[] Light =
        {
            Color.FromArgb(243, 0, 0),      //RED
            Color.FromArgb(239, 239, 239),  //WHITE
            Color.FromArgb(87, 171, 39),    //GREEN
            Color.FromArgb(75, 95, 183),    //BLUE
            Color.FromArgb(255, 255, 0),    //YELLOW
            Color.FromArgb(55, 175, 191),   //CYAN
            Color.FromArgb(235, 131, 11),   //ORANGE
            Color.FromArgb(131, 103, 179)   //PURPLE
        };

        public static readonly Color[] Dark =
        {
            Color.FromArgb(167, 0, 0),      //RED
            Color.FromArgb(175, 175, 175),  //WHITE
            Color.FromArgb(23, 83, 11),     //GREEN
            Color.FromArgb(7, 11, 103),     //BLUE
            Color.FromArgb(243, 183, 7),    //YELLOW
            Color.FromArgb(31, 123, 147),   //CYAN
            Color.FromArgb(227, 83, 15),    //ORANGE
            Color.FromArgb(111, 63, 135)    //PURPLE
        };

        public static readonly Color[] CityTextColor =
        {
            Color.FromArgb(243, 0, 0),      //RED
            Color.FromArgb(255, 255, 255),  //WHITE
            Color.FromArgb(111, 219, 51),     //GREEN
            Color.FromArgb(0, 115, 255),     //BLUE
            Color.FromArgb(255, 255, 0),    //YELLOW
            Color.FromArgb(63, 187, 199),   //CYAN
            Color.FromArgb(243, 183, 7),    //ORANGE
            Color.FromArgb(183, 147, 255)    //PURPLE
        };
    }
}
