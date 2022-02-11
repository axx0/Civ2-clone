using System;
using System.IO;
using System.Collections.Generic;
using Civ2engine;
using EtoFormsUIExtensionMethods;
using Eto.Drawing;

namespace EtoFormsUI
{
    public static partial class Images
    {
        // From Tiles.dll
        public static Bitmap CityStatusWallpaper, DefenseMinWallpaper, ForeignMinWallpaper, AttitudeAdvWallpaper, TradeAdvWallpaper,
                             ScienceAdvWallpaper, WondersOfWorldWallpaper, Top5citiesWallpaper, DemographicsWallpaper, CivScoreWallpaper,
                             TaxRateSmallWallpaper, TaxRateWallpaper, CityConqueredAncientWallpaper, CityConqueredModernWallpaper,
                             CivilDisorderAncientWallpaper, CivilDisorderModernWallpaper, WeLoveKingAncientWallpaper, WeLoveKingModernWallpaper,
                             CityBuiltAncientWallpaper, CityBuiltModernWallpaper, MainScreenSymbol;

        // Offset/length pairs for images from DLLs
        public static readonly Dictionary<string, (int, int)> DllPics = new()
        {
            { "sinaiPic",           (0x1E630, 0x9F78) },
            { "stPeterburgPic",     (0x285A8, 0x15D04) },
            { "mingGeneralPic",     (0x3E2AC, 0x1D183) },
            { "ancientPersonsPic",  (0x5B430, 0x15D04) },
            { "barbariansPic",      (0x71134, 0x13D5B) },
            { "galleyWreckPic",     (0xB6A3C, 0xE77A) },
            { "peoplePic1",         (0x84E90, 0x129CE) },
            { "peoplePic2",         (0x97860, 0x139A0) },
            { "templePic",          (0xAB200, 0xB839) },
            { "islandPic",          (0xDA49C, 0x8980) },
            { "desertPic",          (0xD0140, 0xA35A) },
            { "snowPic",            (0xE2E1C, 0xA925) },
            { "canyonPic",          (0xC51B8, 0xAF88) },

            { "cityviewImprovements", (0x1E6E0, 0x24C0F) },
            { "cityviewWonders", (0x432F0, 0x35C79) },
            { "cityviewAlternative", (0x78F6C, 0x242E4) },
            { "cityviewBaseoceanempty", (0x9D250, 0x45423) },
            { "cityviewBaseoceanroad", (0xE2674, 0x46642) },
            { "cityviewBaseoceanasphaltroad", (0x128CB8, 0x44E6A) },
            { "cityviewBaseoceanhighway", (0x16DB24, 0x44B23) },
            { "cityviewBaseriverempty", (0x1B2648, 0x47B68) },
            { "cityviewBaseriverroad", (0x1FA1B0, 0x48F7A) },
            { "cityviewBaseriverasphaltroad", (0x24312C, 0x472FA) },
            { "cityviewBaseriverhighway", (0x28A428, 0x473B0) },
            { "cityviewBasecontinentempty", (0x2D17D8, 0x48D0A) },
            { "cityviewBasecontinentroad", (0x31A4E4, 0x4A859) },
            { "cityviewBasecontinentasphaltroad", (0x364D40, 0x483D5) },
            { "cityviewBasecontinenthighway", (0x3AD118, 0x48FFE) },
        };

        //  Manually read GIFs from DLLs based on their known address offsets and byte lenghts (obtained from Resource Hacker program)
        public static void ImportDLLimages()
        {
            ExtractTilesDLL();
            //ExtractCvDLL();
        }

        /// <summary>
        /// Extract bitmaps from Tiles.dll
        /// </summary>
        private static void ExtractTilesDLL()
        {
            // Read all bytes in dll
            var tilesDll = File.ReadAllBytes(Utils.GetFilePath( "Tiles.dll", Settings.SearchPaths));

            // Extract GIF from bytes using known offsets and lengths of GIFS from DLL
            // (50) City status wallpaper
            var extractedGIF = ExtractBitmapFromDLL(tilesDll, "1E8B0", "13A3F");
            CityStatusWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (51) Defense minister wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "322F0", "DE6D");
            DefenseMinWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (52) Foreign minister wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "40160", "C9DB");
            ForeignMinWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (53) Attitude advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "4CB3C", "CDFA");
            AttitudeAdvWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (54) Trade advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "59938", "D878");
            TradeAdvWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (55) Science advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "671B0", "CFD2");
            ScienceAdvWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (56) Wonders of world wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "74184", "77E6");
            WondersOfWorldWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (57) Top5cities wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "7B96C", "B9E0");
            Top5citiesWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (58) Demographics wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "8734C", "12ACC");
            DemographicsWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (59) Civ score wallpaper
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "99E18", "B823");
            CivScoreWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (65) Tax rate wallpaper (small)
            //extractedGIF = ExtractBitmapFromDLL(bytes, "A563C", "5CAC");
            //TaxRateSmallWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 400, 256));
            // (66) Tax rate wallpaper (large)
            extractedGIF = ExtractBitmapFromDLL(tilesDll, "AB2E8", "B271");
            TaxRateWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (70) City conquered (ancient)
            CityConqueredAncientWallpaper = ExtractBitmapFromDLL(tilesDll, "B655C", "7DBE");
            // (71) City conquered (modern)
            CityConqueredModernWallpaper = ExtractBitmapFromDLL(tilesDll, "BE31C", "446F");
            // (72) Civil disorder (ancient)
            CivilDisorderAncientWallpaper = ExtractBitmapFromDLL(tilesDll, "C278C", "6C05");
            // (73) Civil disorder (modern)
            CivilDisorderModernWallpaper = ExtractBitmapFromDLL(tilesDll, "C9394", "5A6B");
            // (74) We love king day (ancient)
            WeLoveKingAncientWallpaper = ExtractBitmapFromDLL(tilesDll, "CEE00", "76B0");
            // (75) We love king day (modern)
            WeLoveKingModernWallpaper = ExtractBitmapFromDLL(tilesDll, "D64B0", "88F2");
            // (76) City built (ancient)
            CityBuiltAncientWallpaper = ExtractBitmapFromDLL(tilesDll, "DEDA4", "46FF");
            // (77) City built (modern)
            CityBuiltModernWallpaper = ExtractBitmapFromDLL(tilesDll, "E34A4", "4A42");
            // (90) Main screen
            MainScreenSymbol = ExtractBitmapFromDLL(tilesDll, "F7454", "1389D");
        }

        /// <summary>
        /// Extract bitmap from DLL file
        /// </summary>
        /// <param name="byteArray">Byte array of DLL</param>
        /// <param name="GIFbyteOffset">Hex offset of image in DLL</param>
        /// <param name="GIFbyteLength">Hex length of image in DLL</param>
        /// <returns>Extracted Bitmap image</returns>
        public static Bitmap ExtractBitmapFromDLL(byte[] byteArray, string GIFbyteOffset, string GIFbyteLength)
        {
            Bitmap returnImage;

            // Make empty byte array to hold GIF bytes
            byte[] newBytesRange = new byte[Convert.ToInt32(GIFbyteLength, 16)];

            // Copy GIF bytes in DLL byte array into empty array
            Array.Copy(byteArray, Convert.ToInt32(GIFbyteOffset, 16), newBytesRange, 0, Convert.ToInt32(GIFbyteLength, 16));

            // Convert GIF bytes into a bitmap
            using (var ms = new MemoryStream(newBytesRange))
            {
                returnImage = new Bitmap(ms);
            }

            return returnImage;
        }

        public static Bitmap ExtractBitmap(byte[] byteArray, string name)
        {
            // Make empty byte array to hold GIF bytes
            byte[] newBytesRange = new byte[DllPics[name].Item2];

            // Copy GIF bytes in DLL byte array into empty array
            Array.Copy(byteArray, DllPics[name].Item1, newBytesRange, 0, DllPics[name].Item2);
            
            // Convert GIF bytes into a bitmap
            using var ms = new MemoryStream(newBytesRange);
            return new Bitmap(ms);
        }
    }
}
