using System;
using System.IO;
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

        // From Intro.dll
        public static Bitmap SinaiPic, StPeterburgPic, MingGeneralPic, AncientPersonsPic, BarbariansPic, GalleyWreckPic, PeoplePic1, PeoplePic2, TemplePic;

        // From cv.dll
        public static Bitmap[] CityViewLand, CityViewOcean, CityViewRiver, CityViewTiles, CityViewImprovements;

        //  Manually read GIFs from DLLs based on their known address offsets and byte lenghts (obtained from Resource Hacker program)
        public static void ImportDLLimages()
        {
            ExtractTilesDLL();
            ExtractIntroDLL();
            ExtractCvDLL();
        }

        /// <summary>
        /// Extract bitmaps from Tiles.dll
        /// </summary>
        private static void ExtractTilesDLL()
        {
            // Read all bytes in dll
            byte[] bytes = File.ReadAllBytes(Settings.Civ2Path + "Tiles.dll");

            // Extract GIF from bytes using known offsets and lengths of GIFS from DLL
            // (50) City status wallpaper
            var extractedGIF = ExtractBitmapFromDLL(bytes, "1E8B0", "13A3F");
            CityStatusWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (51) Defense minister wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "322F0", "DE6D");
            DefenseMinWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (52) Foreign minister wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "40160", "C9DB");
            ForeignMinWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (53) Attitude advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "4CB3C", "CDFA");
            AttitudeAdvWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (54) Trade advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "59938", "D878");
            TradeAdvWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (55) Science advisor wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "671B0", "CFD2");
            ScienceAdvWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (56) Wonders of world wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "74184", "77E6");
            WondersOfWorldWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (57) Top5cities wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "7B96C", "B9E0");
            Top5citiesWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (58) Demographics wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "8734C", "12ACC");
            DemographicsWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (59) Civ score wallpaper
            extractedGIF = ExtractBitmapFromDLL(bytes, "99E18", "B823");
            CivScoreWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
            // (65) Tax rate wallpaper (small)
            //extractedGIF = ExtractBitmapFromDLL(bytes, "A563C", "5CAC");
            //TaxRateSmallWallpaper = ModifyImage.CropImage(extractedGIF, new Rectangle(0, 0, 400, 256));
            // (66) Tax rate wallpaper (large)
            extractedGIF = ExtractBitmapFromDLL(bytes, "AB2E8", "B271");
            TaxRateWallpaper = extractedGIF.CropImage(new Rectangle(0, 0, 600, 400));
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
        }

        /// <summary>
        /// Extract bitmaps from Intro.dll
        /// </summary>
        private static void ExtractIntroDLL()
        {
            // Read all bytes in dll
            byte[] bytes = File.ReadAllBytes(Settings.Civ2Path + "Intro.dll");

            SinaiPic = ExtractBitmapFromDLL(bytes, "1E630", "9F78");
            StPeterburgPic = ExtractBitmapFromDLL(bytes, "285A8", "15D04");
            MingGeneralPic = ExtractBitmapFromDLL(bytes, "3E2AC", "1D183");
            AncientPersonsPic = ExtractBitmapFromDLL(bytes, "5B430", "15D04");
            BarbariansPic = ExtractBitmapFromDLL(bytes, "71134", "13D5B");
            GalleyWreckPic = ExtractBitmapFromDLL(bytes, "B6A3C", "E77A");
            PeoplePic1 = ExtractBitmapFromDLL(bytes, "84E90", "129CE");
            PeoplePic2 = ExtractBitmapFromDLL(bytes, "97860", "139A0");
            TemplePic = ExtractBitmapFromDLL(bytes, "AB200", "B839");
        }

        /// <summary>
        /// Extract bitmaps from Cv.dll
        /// </summary>
        private static void ExtractCvDLL()
        {
            // Read all bytes in dll
            byte[] bytes = File.ReadAllBytes(Settings.Civ2Path + "cv.dll");

            // (310) Tiles with improvements in city view
            var extractedGIF = CreateNonIndexedImage(ExtractBitmapFromDLL(bytes, "1E6E0", "24C0F"));
            extractedGIF.ReplaceColors(Color.FromArgb(135, 135, 135), Colors.Transparent);
            extractedGIF.ReplaceColors(Color.FromArgb(255, 0, 255), Colors.Transparent);
            CityViewImprovements = new Bitmap[65];
            CityViewImprovements[0] = extractedGIF.Clone(new Rectangle(1, 1, 123, 82));    // Aqueduct
            CityViewImprovements[1] = extractedGIF.Clone(new Rectangle(125, 1, 123, 82));    // Courthouse
            CityViewImprovements[2] = extractedGIF.Clone(new Rectangle(249, 1, 123, 82));    // Factory
            CityViewImprovements[3] = extractedGIF.Clone(new Rectangle(373, 1, 123, 82));    // Granary
            CityViewImprovements[4] = extractedGIF.Clone(new Rectangle(497, 1, 123, 82));    // Library
            CityViewImprovements[5] = extractedGIF.Clone(new Rectangle(1, 84, 123, 82));    // Manufacturing Plant
            CityViewImprovements[6] = extractedGIF.Clone(new Rectangle(125, 84, 123, 82));    // Marketplace
            CityViewImprovements[7] = extractedGIF.Clone(new Rectangle(249, 84, 123, 82));    // Nuclear plant
            CityViewImprovements[8] = extractedGIF.Clone(new Rectangle(373, 84, 123, 82));    // Research lab
            CityViewImprovements[9] = extractedGIF.Clone(new Rectangle(497, 84, 123, 82));    // Supermarket
            CityViewImprovements[10] = extractedGIF.Clone(new Rectangle(1, 167, 123, 82));    // Airport
            CityViewImprovements[11] = extractedGIF.Clone(new Rectangle(125, 167, 123, 82));    // Bank
            CityViewImprovements[12] = extractedGIF.Clone(new Rectangle(249, 167, 123, 82));    // Colosseum
            CityViewImprovements[13] = extractedGIF.Clone(new Rectangle(373, 167, 123, 82));    // University
            CityViewImprovements[14] = extractedGIF.Clone(new Rectangle(497, 167, 123, 82));    // SDI defense
            CityViewImprovements[15] = extractedGIF.Clone(new Rectangle(1, 250, 123, 82));    // Coastal fortress
            CityViewImprovements[16] = extractedGIF.Clone(new Rectangle(125, 250, 123, 82));    // SAM Missile Battery
            CityViewImprovements[17] = extractedGIF.Clone(new Rectangle(249, 250, 123, 82));    // Barracks
            CityViewImprovements[18] = extractedGIF.Clone(new Rectangle(373, 250, 123, 82));    // Cathedral
            CityViewImprovements[19] = extractedGIF.Clone(new Rectangle(497, 250, 123, 82));    // Sewer system
            CityViewImprovements[20] = extractedGIF.Clone(new Rectangle(1, 333, 123, 82));    // Temple
            CityViewImprovements[21] = extractedGIF.Clone(new Rectangle(125, 333, 123, 82));    // Power plant
            CityViewImprovements[22] = extractedGIF.Clone(new Rectangle(249, 333, 123, 82));    // Hydro plant
            CityViewImprovements[23] = extractedGIF.Clone(new Rectangle(373, 333, 123, 82));    // Palace
            CityViewImprovements[24] = extractedGIF.Clone(new Rectangle(497, 333, 123, 82));    // ???
            CityViewImprovements[25] = extractedGIF.Clone(new Rectangle(1, 416, 123, 82));    // Recycling center
            CityViewImprovements[26] = extractedGIF.Clone(new Rectangle(125, 416, 123, 82));    // Stock exchange (???)
            CityViewImprovements[27] = extractedGIF.Clone(new Rectangle(249, 416, 357, 78));    // City walls
            CityViewImprovements[28] = extractedGIF.Clone(new Rectangle(1, 499, 220, 100));    // Harbor
            CityViewImprovements[29] = extractedGIF.Clone(new Rectangle(222, 499, 367, 118));    // Port facility
            CityViewImprovements[30] = extractedGIF.Clone(new Rectangle(590, 499, 105, 105));    // Offshore platform
            CityViewImprovements[31] = extractedGIF.Clone(new Rectangle(1, 618, 123, 82));    // Solar plant
            CityViewImprovements[32] = extractedGIF.Clone(new Rectangle(125, 618, 123, 82));    // Mass transit

            // (305) Tiles with wonders in city view
            extractedGIF = CreateNonIndexedImage(ExtractBitmapFromDLL(bytes, "432F0", "35C79"));
            extractedGIF.ReplaceColors(Color.FromArgb(135, 135, 135), Colors.Transparent);
            extractedGIF.ReplaceColors(Color.FromArgb(255, 0, 255), Colors.Transparent);
            CityViewImprovements[33] = extractedGIF.Clone(new Rectangle(1, 1, 158, 114));    // Appolo program
            CityViewImprovements[34] = extractedGIF.Clone(new Rectangle(160, 1, 158, 114));    // Women's suffrage
            CityViewImprovements[35] = extractedGIF.Clone(new Rectangle(319, 1, 158, 114));    // Hoover dam
            CityViewImprovements[36] = extractedGIF.Clone(new Rectangle(478, 1, 158, 114));    // Great library
            CityViewImprovements[37] = extractedGIF.Clone(new Rectangle(1, 116, 158, 114));    // Hanging gardens
            CityViewImprovements[38] = extractedGIF.Clone(new Rectangle(160, 116, 158, 114));    // Manhattan project
            CityViewImprovements[39] = extractedGIF.Clone(new Rectangle(319, 116, 158, 114));    // Michelangelo's chapel
            CityViewImprovements[40] = extractedGIF.Clone(new Rectangle(478, 116, 158, 114));    // Oracle
            CityViewImprovements[41] = extractedGIF.Clone(new Rectangle(1, 231, 158, 114));    // Shakespeare's theatre
            CityViewImprovements[42] = extractedGIF.Clone(new Rectangle(160, 231, 158, 114));    // United nations
            CityViewImprovements[43] = extractedGIF.Clone(new Rectangle(319, 231, 158, 114));    // SETI program
            CityViewImprovements[44] = extractedGIF.Clone(new Rectangle(478, 231, 158, 114));    // Copernicus observ.
            CityViewImprovements[45] = extractedGIF.Clone(new Rectangle(1, 346, 158, 114));    // Darwin's voyage
            CityViewImprovements[46] = extractedGIF.Clone(new Rectangle(160, 346, 158, 114));    // Isaac Newton's college
            CityViewImprovements[47] = extractedGIF.Clone(new Rectangle(319, 346, 158, 114));    // JS Bach's cathedral
            CityViewImprovements[48] = extractedGIF.Clone(new Rectangle(478, 346, 158, 114));    // Magellan's voyage
            CityViewImprovements[49] = extractedGIF.Clone(new Rectangle(1, 461, 158, 114));    // Leonardo's workshop
            CityViewImprovements[50] = extractedGIF.Clone(new Rectangle(160, 461, 158, 114));    // Sun Tzu's war academy
            CityViewImprovements[51] = extractedGIF.Clone(new Rectangle(319, 461, 158, 114));    // Marco Polo's embassy
            CityViewImprovements[52] = extractedGIF.Clone(new Rectangle(478, 461, 158, 114));    // King Richard's crusade
            CityViewImprovements[53] = extractedGIF.Clone(new Rectangle(1, 576, 158, 114));    // Pyramids
            CityViewImprovements[54] = extractedGIF.Clone(new Rectangle(160, 576, 158, 114));    // Adam Smith's trading co
            CityViewImprovements[55] = extractedGIF.Clone(new Rectangle(319, 576, 158, 114));    // Eiffel tower
            CityViewImprovements[56] = extractedGIF.Clone(new Rectangle(478, 576, 158, 114));    // Cure for cancer
            CityViewImprovements[57] = extractedGIF.Clone(new Rectangle(1, 691, 304, 160));    // Great wall v1
            CityViewImprovements[58] = extractedGIF.Clone(new Rectangle(306, 691, 304, 160));    // Great wall v2
            CityViewImprovements[59] = extractedGIF.Clone(new Rectangle(1, 852, 125, 253));    // Statue of liberty v1
            CityViewImprovements[60] = extractedGIF.Clone(new Rectangle(127, 852, 125, 253));    // Statue of liberty v2
            CityViewImprovements[61] = extractedGIF.Clone(new Rectangle(253, 852, 76, 133));    // Lighthouse v1
            CityViewImprovements[62] = extractedGIF.Clone(new Rectangle(330, 852, 76, 133));    // Lighthouse v2
            CityViewImprovements[63] = extractedGIF.Clone(new Rectangle(407, 852, 94, 160));    // Colossus v1
            CityViewImprovements[64] = extractedGIF.Clone(new Rectangle(502, 852, 94, 160));    // Colossus v2

            // (310) Normal tiles in city view
            extractedGIF = CreateNonIndexedImage(ExtractBitmapFromDLL(bytes, "78F6C", "242E4"));
            extractedGIF.ReplaceColors(Color.FromArgb(135, 135, 135), Colors.Transparent); // Replace gray
            extractedGIF.ReplaceColors(Color.FromArgb(255, 0, 255), Colors.Transparent);   // Replace pink
            CityViewTiles = new Bitmap[27];
            CityViewTiles[0] = extractedGIF.Clone(new Rectangle(1, 1, 158, 114));
            CityViewTiles[1] = extractedGIF.Clone(new Rectangle(160, 1, 158, 114));
            CityViewTiles[2] = extractedGIF.Clone(new Rectangle(319, 1, 158, 114));
            CityViewTiles[3] = extractedGIF.Clone(new Rectangle(478, 1, 158, 114));
            CityViewTiles[4] = extractedGIF.Clone(new Rectangle(1, 116, 158, 114));
            CityViewTiles[5] = extractedGIF.Clone(new Rectangle(160, 116, 158, 114));
            CityViewTiles[6] = extractedGIF.Clone(new Rectangle(319, 116, 158, 114));
            CityViewTiles[7] = extractedGIF.Clone(new Rectangle(478, 116, 158, 114));
            CityViewTiles[8] = extractedGIF.Clone(new Rectangle(1, 231, 158, 114));
            CityViewTiles[9] = extractedGIF.Clone(new Rectangle(160, 231, 158, 114));
            CityViewTiles[10] = extractedGIF.Clone(new Rectangle(319, 231, 158, 114));
            CityViewTiles[11] = extractedGIF.Clone(new Rectangle(478, 231, 158, 114));
            CityViewTiles[12] = extractedGIF.Clone(new Rectangle(1, 346, 123, 82));
            CityViewTiles[13] = extractedGIF.Clone(new Rectangle(125, 346, 123, 82));
            CityViewTiles[14] = extractedGIF.Clone(new Rectangle(249, 346, 123, 82));
            CityViewTiles[15] = extractedGIF.Clone(new Rectangle(373, 346, 123, 82));
            CityViewTiles[16] = extractedGIF.Clone(new Rectangle(497, 346, 123, 82));
            CityViewTiles[17] = extractedGIF.Clone(new Rectangle(1, 429, 123, 82));
            CityViewTiles[18] = extractedGIF.Clone(new Rectangle(125, 429, 123, 82));
            CityViewTiles[19] = extractedGIF.Clone(new Rectangle(249, 429, 123, 82));
            CityViewTiles[20] = extractedGIF.Clone(new Rectangle(373, 429, 123, 82));
            CityViewTiles[21] = extractedGIF.Clone(new Rectangle(497, 429, 123, 82));
            CityViewTiles[22] = extractedGIF.Clone(new Rectangle(1, 512, 123, 82));
            CityViewTiles[23] = extractedGIF.Clone(new Rectangle(125, 512, 123, 82));
            CityViewTiles[24] = extractedGIF.Clone(new Rectangle(249, 512, 123, 82));
            CityViewTiles[25] = extractedGIF.Clone(new Rectangle(373, 512, 123, 82));
            CityViewTiles[26] = extractedGIF.Clone(new Rectangle(497, 512, 123, 82));

            extractedGIF.Dispose();

            // (340) Empty city view (ocean)
            CityViewOcean = new Bitmap[4];
            CityViewOcean[0] = ExtractBitmapFromDLL(bytes, "9D250", "45423");   // No road
            CityViewOcean[1] = ExtractBitmapFromDLL(bytes, "E2674", "46642");   // Dirt road
            CityViewOcean[2] = ExtractBitmapFromDLL(bytes, "128CB8", "44E6A");  // Asphalt road
            CityViewOcean[3] = ExtractBitmapFromDLL(bytes, "16DB24", "44B23");  // Highway
            // (345) Empty city view (river)
            CityViewRiver = new Bitmap[4];
            CityViewRiver[0] = ExtractBitmapFromDLL(bytes, "1B2648", "47B68");  // No road
            CityViewRiver[1] = ExtractBitmapFromDLL(bytes, "1FA1B0", "48F7A");  // Dirt road
            CityViewRiver[2] = ExtractBitmapFromDLL(bytes, "24312C", "472FA");  // Asphalt road
            CityViewRiver[3] = ExtractBitmapFromDLL(bytes, "28A428", "473B0");  // Highway
            // (350) Empty city view (land)
            CityViewLand = new Bitmap[4];
            CityViewLand[0] = ExtractBitmapFromDLL(bytes, "2D17D8", "48D0A");   // No road
            CityViewLand[1] = ExtractBitmapFromDLL(bytes, "31A4E4", "4A859");   // Dirt road
            CityViewLand[2] = ExtractBitmapFromDLL(bytes, "364D40", "483D5");   // Asphalt road
            CityViewLand[3] = ExtractBitmapFromDLL(bytes, "3AD118", "48FFE");   // Highway

            
        }

        /// <summary>
        /// Extract GIF image from DLL bytes
        /// </summary>
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
