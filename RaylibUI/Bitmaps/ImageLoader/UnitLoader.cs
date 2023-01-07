using Civ2engine;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI.ImageLoader
{
    public static class UnitLoader
    {
        //private static readonly Color _shadowColour = new Color(51, 51, 51, 255);

        public static void LoadUnits(Ruleset ruleset)
        {
            int _frames = 0;
            var unitsImage = Raylib.LoadImageAnim(ruleset.Paths[0] + Path.DirectorySeparatorChar + "UNITS.gif", out _frames);

            // Initialize objects
            var units = new List<UnitImage>();

            //// Define transparent colors
            //var transparentGray =
            //    unitsImage.GetPixel(unitsImage.Width - 1, unitsImage.Height - 1); //define transparent back color (gray)
            //var transparentPink = unitsImage.GetPixel(2, 2); //define transparent back color (pink)

            //var borderColour = unitsImage.GetPixel(0, 0);
            //var flagColour = borderColour;
            //for (var i = 0; i < 100; i++)
            //{
            //    flagColour = unitsImage.GetPixel(0, i);
            //    if (flagColour != borderColour) break;
            //}

            //var borderColours = new List<Color> {borderColour, flagColour};

            //var height = 0;
            //for (var i = 1; i < unitsImage.Height; i++)
            //{
            //    if (borderColours.IndexOf(unitsImage.GetPixel(1, i)) == -1) continue;
            //    height = i;
            //    break;
            //}

            //var width = 0;


            //for (var i = 1; i < unitsImage.Width; i++)
            //{
            //    if (borderColours.IndexOf(unitsImage.GetPixel(i, 1)) == -1) continue;
            //    width = i;
            //    break;
            //}

            //MakeSheilds(unitsImage, width, borderColour, transparentGray);

            // Make transparent colors
            Raylib.ImageColorReplace(ref unitsImage, new Color(135, 83, 135, 255), new Color(135, 83, 135, 0));
            Raylib.ImageColorReplace(ref unitsImage, new Color(255, 0, 255, 255), new Color(255, 0, 255, 0));

            for (var row = 0; row < 7; row++)
            {
                for (var col = 0; col <9; col++)
                {
                    //var flagX = 0;
                    //var flagY = 0;

                    //for (var i = col; i < col + width; i++)
                    //{
                    //    var colour = unitsImage.GetPixel(i, row);
                    //    if (colour == borderColour) continue;
                    //    flagX = i - col - 1;
                    //    break;
                    //}

                    //for (var i = row; i < row + height; i++)
                    //{
                    //    var colour = unitsImage.GetPixel(col, i);
                    //    if (colour == borderColour) continue;
                    //    flagY = i - row - 1;
                    //    break;
                    //}

                    //if (flagX == 0 || flagY == 0)
                    //{
                    //    continue;
                    //}

                    var img = Raylib.ImageFromImage(unitsImage, new Rectangle(1 + col * 65, 1 + row * 49, 64, 48));
                    units.Add(new UnitImage
                    {
                        Bitmap = img,
                        Texture = Raylib.LoadTextureFromImage(img),
                        FlagLoc = new Vector2(0, 0)
                    });
                    Raylib.UnloadImage(img);
                }
            }

            MapImages.Units = units.ToArray();
        }

        //private static void MakeSheilds(Bitmap unitsImage, int width, Color borderColour, Color transparentGray)
        //{
        //    int lastBorder;
        //    for (lastBorder = unitsImage.Width - 1; lastBorder > width; lastBorder--)
        //    {
        //        if (unitsImage.GetPixel(lastBorder, 0) == borderColour) break;
        //    }

        //    var shieldWidth = 0;
        //    for (var i = lastBorder - 1; i >= 0; i--)
        //    {
        //        if (unitsImage.GetPixel(i, 1) != borderColour) continue;
        //        shieldWidth = lastBorder - i;
        //        break;
        //    }

        //    var shieldHeight = 0;
        //    for (var i = 1; i < unitsImage.Height; i++)
        //    {
        //        if (unitsImage.GetPixel(lastBorder - 1, i) != borderColour) continue;
        //        shieldHeight = i;
        //        break;
        //    }


        //    var unitShield = unitsImage.Clone(new Rectangle(lastBorder - shieldWidth * (shieldWidth < shieldHeight ? 2 : 1) + 1,
        //        1, shieldWidth - 1, shieldHeight - 1));
        //    unitShield.SetTransparent(new Color[] { transparentGray });
        //    var firstColour = unitShield.GetPixel(3, 3);

        //    Bitmap MakeShield(Color colour)
        //    {
        //        var shield = unitShield.Clone();
        //        shield.ReplaceColors(firstColour, colour);
        //        return shield;
        //    }

        //    MapImages.Shields = MapImages.PlayerColours.Select(c=>c.LightColour).Select((Func<Color,Bitmap>) MakeShield).ToArray();
        //    MapImages.ShieldBack = MapImages.PlayerColours.Select(c=>c.DarkColour).Select((Func<Color,Bitmap>) MakeShield).ToArray();
        //    MapImages.ShieldShadow = MakeShield(_shadowColour);
        //}
    }

    public class UnitImage
    {
        public Image Bitmap { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 FlagLoc { get; set; }
    }
}