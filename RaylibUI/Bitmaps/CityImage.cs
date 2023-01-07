using System.Numerics;
using Raylib_cs;

namespace RaylibUI
{
    public sealed class CityImage
    {
        public Image Bitmap { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 FlagLoc { get; set; }
        public Vector2 SizeLoc { get; set; }
    }
}