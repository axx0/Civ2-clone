using Civ2engine;
using Raylib_cs;
using System.Collections;
using System.Runtime.InteropServices;

namespace RayLibUtils;

public static class Images
{
    public static Image LoadImage(string filename, string[] searchPaths, params string[] extensions)
    {
        var path = Utils.GetFilePath(filename, searchPaths, extensions);
        return Raylib.LoadImageFromMemory(Path.GetExtension(path).ToLowerInvariant(), File.ReadAllBytes(path));
    }

    /// <summary>
    /// Get images and flag locs from GIF/BMP
    /// </summary>
    public static void LoadPropertiesFromPIC(string filename, Dictionary<string, List<ImageProps>> props)
    {
        byte[] bytes = File.ReadAllBytes(filename);

        if (System.Text.Encoding.UTF8.GetString(bytes, 0, 3).Equals("GIF"))
        {
            // Colors are 32bit int uncompressed (4x8 bits = B+G+R+A)
            int Width = BitConverter.ToInt16(bytes, 6);
            int Height = BitConverter.ToInt16(bytes, 8);

            // Let raylib deal with LZW decompression
            var _img = Raylib.LoadImageFromMemory(Path.GetExtension(filename).ToLowerInvariant(), bytes);

            // Get 3 transparent colours from palette and replace colours
            Color[] transparent = new Color[3];
            for (int i = 0; i < 3; i++)
            {
                transparent[i] = new Color()
                {
                    A = 0xff,
                    R = bytes[13 + 3 * (253 + i) + 0],
                    G = bytes[13 + 3 * (253 + i) + 1],
                    B = bytes[13 + 3 * (253 + i) + 2],
                };

                Raylib.ImageColorReplace(ref _img, transparent[i],
                    new Color(transparent[i].R, transparent[i].G, transparent[i].B, (byte)0));
            }

            // Get two flag colors from palette
            int flag1color = BitConverter.ToInt32(new byte[]
            {
                bytes[13 + 3 * 250 + 0],
                bytes[13 + 3 * 250 + 1],
                bytes[13 + 3 * 250 + 2],
                0xff
            });
            int flag2color = BitConverter.ToInt32(new byte[]
            {
                bytes[13 + 3 * 249 + 0],
                bytes[13 + 3 * 249 + 1],
                bytes[13 + 3 * 249 + 2],
                0xff
            });

            unsafe
            {
                IntPtr ptr = (IntPtr)_img.Data;
                
                // Get subimages
                foreach (var prop in props)
                {
                    for (int i = 0; i < prop.Value.Count; i++)
                    {
                        var oneProp = prop.Value[i];

                        oneProp.Image = Raylib.ImageFromImage(_img, oneProp.Rect);

                        // Get locations of flags (for units, cities)
                        // Row
                        for (int col = 0; col < oneProp.Rect.Width; col++)
                        {
                            var color = Marshal.ReadInt32(ptr + 4 * (((int)oneProp.Rect.Y - 1) *
                                        _img.Width + (int)oneProp.Rect.X + col));
                            if (color == flag1color)    // units flag (blue)
                            {
                                oneProp.Flag1x = col;
                            }
                            else if (color == flag2color)   // cities color (orange)
                            {
                                oneProp.Flag2x = col;
                            }
                        }

                        // Column
                        for (int row = 0; row < oneProp.Rect.Height; row++)
                        {
                            var color = Marshal.ReadInt32(ptr + 4 * (((int)oneProp.Rect.Y + row) *
                                        _img.Width + (int)oneProp.Rect.X - 1));
                            if (color == flag1color)
                            {
                                oneProp.Flag1y = row;
                            }
                            else if (color == flag2color)
                            {
                                oneProp.Flag2y = row;
                            }
                        }
                    }
                }
            }

            Raylib.UnloadImage(_img);
        }
        // BMP
        else if (System.Text.Encoding.UTF8.GetString(bytes, 0, 2).Equals("BM"))
        {
            var dataOffset = BitConverter.ToInt16(bytes, 10);
            var Width = BitConverter.ToInt32(bytes, 18);
            var Height = BitConverter.ToInt32(bytes, 22);
            var bpp = BitConverter.ToInt16(bytes, 28);
            var imgData = new byte[4 * Width * Height];
            int flag1color = Convert.ToInt32("0x0000FFFF", 16);
            int flag2color = Convert.ToInt32("0xFF9C00FF", 16);

            switch (bpp)
            {
                // 8bpp uses palette
                case 8:
                    {
                        // Get palette
                        byte[,] palette = new byte[256, 4];
                        for (int i = 0; i < 256; i++)
                        {
                            palette[i, 0] = bytes[54 + 4 * i + 0];  // R
                            palette[i, 1] = bytes[54 + 4 * i + 1];  // G
                            palette[i, 2] = bytes[54 + 4 * i + 2];  // B
                            palette[i, 3] = bytes[54 + 4 * i + 3];  //
                        }

                        // Get two flag colors from palette
                        flag1color = BitConverter.ToInt32(new byte[]
                        {
                            palette[250, 2],
                            palette[250, 1],
                            palette[250, 0],
                            0xff
                        });
                        flag2color = BitConverter.ToInt32(new byte[]
                        {
                            palette[249, 2],
                            palette[249, 1],
                            palette[249, 0],
                            0xff
                        });

                        for (int row = 0; row < Height; row++)
                        {
                            for (int col = 0; col < Width; col++)
                            {
                                var pos = dataOffset + Width * (Height - 1 - row) + col;
                                imgData[4 * (Width * row + col) + 0] = palette[bytes[pos], 2];
                                imgData[4 * (Width * row + col) + 1] = palette[bytes[pos], 1];
                                imgData[4 * (Width * row + col) + 2] = palette[bytes[pos], 0];
                                if (bytes[pos] == 253 || bytes[pos] == 254 || bytes[pos] == 255)
                                {
                                    imgData[4 * (Width * row + col) + 3] = 0;
                                }
                                else
                                {
                                    imgData[4 * (Width * row + col) + 3] = 255;
                                }
                            }
                        }
                    }
                    break;

                // 16+24bpp have no palette
                case 16:
                case 24:
                    {
                        for (int row = 0; row < Height; row++)
                        {
                            for (int col = 0; col < Width; col++)
                            {
                                if (bpp == 16)
                                {
                                    var _15bitrgb = Get15bitRGB(bytes, dataOffset + 
                                                    2 * Width * (Height - 1 - row) + 2 * col);
                                    imgData[4 * (Width * row + col) + 0] = (byte)(_15bitrgb[2] * 255 / 31);
                                    imgData[4 * (Width * row + col) + 1] = (byte)(_15bitrgb[1] * 255 / 31);
                                    imgData[4 * (Width * row + col) + 2] = (byte)(_15bitrgb[0] * 255 / 31);
                                    imgData[4 * (Width * row + col) + 3] = 255;
                                }
                                else
                                {
                                    var _off = dataOffset + 3 * Width * (Height - 1 - row) + 3 * col;
                                    imgData[4 * (Width * row + col) + 0] = bytes[_off + 2];
                                    imgData[4 * (Width * row + col) + 1] = bytes[_off + 1];
                                    imgData[4 * (Width * row + col) + 2] = bytes[_off + 0];
                                    imgData[4 * (Width * row + col) + 3] = 255;
                                }
                            }
                        }

                        // Make subimages transparent
                        // Pink + upper-left pixel in rectangle are transparent colors
                        var transparent1 = new byte[3] { 255, 0, 255 };
                        var transparent2 = new byte[3];
                        foreach (var prop in props)
                        {
                            for (int i = 0; i < prop.Value.Count; i++)
                            {
                                var rect = prop.Value[i].Rect;

                                // 2nd transparent color
                                transparent2 = new byte[3]
                                {
                                    imgData[4 * (Width * (int)rect.Y + (int)rect.X) + 0],
                                    imgData[4 * (Width * (int)rect.Y + (int)rect.X) + 1],
                                    imgData[4 * (Width * (int)rect.Y + (int)rect.X) + 2],
                                };

                                // Make part of image transparent
                                for (int row = (int)rect.Y; row < rect.Y + rect.Height; row++)
                                {
                                    for (int col = (int)rect.X; col < rect.X + rect.Width; col++)
                                    {
                                        if ((imgData[4 * (Width * row + col) + 0] == transparent1[0] &&
                                             imgData[4 * (Width * row + col) + 1] == transparent1[1] &&
                                             imgData[4 * (Width * row + col) + 2] == transparent1[2]) ||
                                            (imgData[4 * (Width * row + col) + 0] == transparent2[0] &&
                                             imgData[4 * (Width * row + col) + 1] == transparent2[1] &&
                                             imgData[4 * (Width * row + col) + 2] == transparent2[2]))
                                        {
                                            imgData[4 * (Width * row + col) + 3] = 0;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;

                default: throw new ArgumentOutOfRangeException();

            };

            Image _img;
            unsafe
            {
                fixed (byte* ptr = imgData)
                {
                    _img = new Image
                    {
                        Data = ptr,
                        Format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
                        Width = Width,
                        Height = Height,
                        Mipmaps = 1
                    };
                }

                // Get subimages
                foreach (var prop in props)
                {
                    for (int i = 0; i < prop.Value.Count; i++)
                    {
                        var oneProp = prop.Value[i];

                        oneProp.Image = Raylib.ImageFromImage(_img, oneProp.Rect);

                        // Get locations of flags(for units, cities)
                        // Row
                        for (int col = 0; col < oneProp.Rect.Width; col++)
                        {
                            var color = BitConverter.ToInt32(imgData,
                                4 * (((int)oneProp.Rect.Y - 1) * _img.Width + (int)oneProp.Rect.X + col));
                            if (color == flag1color)    // units flag (blue)
                            {
                                oneProp.Flag1x = col;
                            }
                            else if (color == flag2color)   // cities color (orange)
                            {
                                oneProp.Flag2x = col;
                            }
                        }

                        // Column
                        for (int row = 0; row < oneProp.Rect.Height; row++)
                        {
                            var color = BitConverter.ToInt32(imgData,
                                4 * (((int)oneProp.Rect.Y + row) * _img.Width + (int)oneProp.Rect.X - 1));
                            if (color == flag1color)
                            {
                                oneProp.Flag1y = row;
                            }
                            else if (color == flag2color)
                            {
                                oneProp.Flag2y = row;
                            }
                        }
                    }
                }
            }

            //Raylib.UnloadImage(_img);
        }
        else
        {
            throw new Exception($"Image file type for {filename} not supported!");
        }
    }

    private static int[] Get15bitRGB(byte[] byteArray, int offset)
    {
        var a = new BitArray(new byte[] { byteArray[offset + 0] });
        var b = new BitArray(new byte[] { byteArray[offset + 1] });

        return new int[3]
        {
                1 * (a[0] ? 1 : 0) + 2 * (a[1] ? 1 : 0) + 4 * (a[2] ? 1 : 0) +
                8 * (a[3] ? 1 : 0) + 16 * (a[4] ? 1 : 0),
                1 * (a[5] ? 1 : 0) + 2 * (a[6] ? 1 : 0) + 4 * (a[7] ? 1 : 0) +
                8 * (b[0] ? 1 : 0) + 16 * (b[1] ? 1 : 0),
                1 * (b[2] ? 1 : 0) + 2 * (b[3] ? 1 : 0) + 4 * (b[4] ? 1 : 0) +
                8 * (b[5] ? 1 : 0) + 16 * (b[6] ? 1 : 0),
        };
    }
}