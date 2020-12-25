using System;
using System.Drawing;
using System.IO;

namespace civ2.Bitmaps
{
    public static partial class Images
    {
        // From Tiles.dll
        public static Bitmap CityStatusWallpaper, DefenseMinWallpaper, ForeignMinWallpaper, AttitudeAdvWallpaper, TradeAdvWallpaper,
                             ScienceAdvWallpaper, WondersOfWorldWallpaper, Top5citiesWallpaper, DemographicsWallpaper, CivScoreWallpaper,
                             TaxRateSmallWallpaper, TaxRateWallpaper, CityConqueredAncientWallpaper, CityConqueredModernWallpaper,
                             CivilDisorderAncientWallpaper, CivilDisorderModernWallpaper, WeLoveKingAncientWallpaper, WeLoveKingModernWallpaper,
                             CityBuiltAncientWallpaper, CityBuiltModernWallpaper, MainScreenSymbol;

        // From Intro.dll
        public static Bitmap SinaiPic;

        // Import gifs from various DLL files
        public static void ImportDLLimages()
        {
            // Manually read GIFs based on their known address offsets and byte lenghts (obtained from Resource Hacker program)

            // ========================================================================================
            // TILES.DLL
            string DLLname = "Tiles.dll";

            // Read all bytes in dll
            string DLLpath = Settings.Civ2Path + DLLname;
            byte[] bytes = File.ReadAllBytes(DLLpath);

            // Extract GIF from bytes using known offsets and lengths of GIFS from DLL
            // (50) City status wallpaper
            Bitmap extractedGIF = ExtractBitmapFromDLL(bytes, "1E8B0", "13A3F");
            CityStatusWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (51) Defense minister wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "322F0", "DE6D");
            DefenseMinWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (52) Foreign minister wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "40160", "C9DB");
            ForeignMinWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (53) Attitude advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "4CB3C", "CDFA");
            AttitudeAdvWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (54) Trade advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "59938", "D878");
            TradeAdvWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (55) Science advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "671B0", "CFD2");
            ScienceAdvWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (56) Wonders of world wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "74184", "77E6");
            WondersOfWorldWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (57) Top5cities wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "7B96C", "B9E0");
            Top5citiesWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (58) Demographics wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "8734C", "12ACC");
            DemographicsWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (59) Civ score wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "99E18", "B823");
            CivScoreWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (65) Tax rate wallpaper (small)
            //extractedGIF = ExtractBitmapFromDLL(bytes, "A563C", "5CAC");
            //TaxRateSmallWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 400, 256));
            // (66) Tax rate wallpaper (large)
            extractedGIF = ExtractBitmapFromDLL(bytes, "AB2E8", "B271");
            TaxRateWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 600, 400));
            // (70) City conquered (ancient)
            CityConqueredAncientWallpaper = ExtractBitmapFromDLL(bytes, "B655C", "7DBE");
            // (71) City conquered (modern)
            CityConqueredModernWallpaper = ExtractBitmapFromDLL(bytes, "BE31C", "446F");
            // (72) Civil disorder (ancient)
            CivilDisorderAncientWallpaper = ExtractBitmapFromDLL(bytes, "C278C", "6C05");
            // (73) Civil disorder (modern)
            CivilDisorderModernWallpaper = ExtractBitmapFromDLL(bytes, "C9394", "5A6B");
            // (74) We love king day (ancient)
            WeLoveKingAncientWallpaper = ExtractBitmapFromDLL(bytes, "CEE00", "76B0");
            // (75) We love king day (modern)
            WeLoveKingModernWallpaper = ExtractBitmapFromDLL(bytes, "D64B0", "88F2");
            // (76) City built (ancient)
            CityBuiltAncientWallpaper = ExtractBitmapFromDLL(bytes, "DEDA4", "46FF");
            // (77) City built (modern)
            CityBuiltModernWallpaper = ExtractBitmapFromDLL(bytes, "E34A4", "4A42");
            // (90) Main screen
            MainScreenSymbol = ExtractBitmapFromDLL(bytes, "F7454", "1389D");

            // ========================================================================================
            // INTRO.DLL
            DLLname = "Intro.dll";
            DLLpath = Settings.Civ2Path + DLLname;
            bytes = File.ReadAllBytes(DLLpath);

            // (901) City status wallpaper
            SinaiPic = ExtractBitmapFromDLL(bytes, "1E630", "9F78");
        }

        // Extract GIF image from DLL bytes
        private static Bitmap ExtractBitmapFromDLL(byte[] byteArray, string GIFbyteOffset, string GIFbyteLength)
        {
            Bitmap returnImage;

            // Make empty byte array to hold GIF bytes
            byte[] newBytesRange = new byte[Convert.ToInt32(GIFbyteLength, 16)];

            // Copy GIF bytes in DLL byte array into empty array
            Array.Copy(byteArray, Convert.ToInt32(GIFbyteOffset, 16), newBytesRange, 0, Convert.ToInt32(GIFbyteLength, 16));

            // Convert GIF bytes into a bitmap
            using (MemoryStream ms = new MemoryStream(newBytesRange))
            {
                returnImage = new Bitmap(ms);
            }

            return returnImage;
        }
    }
}
