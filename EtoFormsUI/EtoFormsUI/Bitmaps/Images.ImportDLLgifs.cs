using System;
using System.IO;
using System.Collections.Generic;
using Eto.Drawing;

namespace EtoFormsUI
{
    public static partial class Images
    {
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

            { "cityStatusWallpaper", (0x1E8B0, 0x13A3F) },
            { "defenseMinWallpaper", (0x322F0, 0xDE6D) },
            { "foreignMinWallpaper", (0x40160, 0xC9DB) },
            { "attitudeAdvWallpaper", (0x4CB3C, 0xCDFA) },
            { "tradeAdvWallpaper", (0x59938, 0xD878) },
            { "scienceAdvWallpaper", (0x671B0, 0xCFD2) },
            { "wondersOfWorldWallpaper", (0x74184, 0x77E6) },
            { "top5citiesWallpaper", (0x7B96C, 0xB9E0) },
            { "demographicsWallpaper", (0x8734C, 0x12ACC) },
            { "civScoreWallpaper", (0x99E18, 0xB823) },
            { "taxRateWallpaper", (0xAB2E8, 0xB271) },
            { "cityConqueredAncientWallpaper", (0xB655C, 0x7DBE) },
            { "cityConqueredModernWallpaper", (0xBE31C, 0x446F) },
            { "civilDisorderAncientWallpaper", (0xC278C, 0x6C05) },
            { "civilDisorderModernWallpaper", (0xC9394, 0x5A6B) },
            { "weLoveKingAncientWallpaper", (0xCEE00, 0x76B0) },
            { "weLoveKingModernWallpaper", (0xD64B0, 0x88F2) },
            { "cityBuiltAncientWallpaper", (0xDEDA4, 0x46FF) },
            { "cityBuiltModernWallpaper", (0xE34A4, 0x4A42) },
            { "introScreenSymbol", (0xF7454, 0x1389D) },
        };

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
