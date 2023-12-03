using Civ2engine;
using Raylib_cs;
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
            int width = BitConverter.ToInt16(bytes, 6);
            int height = BitConverter.ToInt16(bytes, 8);

            // Let raylib deal with LZW decompression
            var _img = Raylib.LoadImage(filename);

            // Get 3 transparent colors from palette
            int[] transparent = new int[3];
            for (int i = 0; i < 3; i++)
            {
                transparent[i] = BitConverter.ToInt32(new byte[]
                {
                    bytes[13 + 3 * (253 + i) + 0],  // palette starts @ offset=13
                    bytes[13 + 3 * (253 + i) + 1],
                    bytes[13 + 3 * (253 + i) + 2],
                    0xff                            // alpha
                });
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
                // Replace transparent colors in large image
                IntPtr ptr = (IntPtr)_img.data;
                for (int row = 0; row < height; row++)
                {
                    for (int col = 0; col < width; col++)
                    {
                        var color = Marshal.ReadInt32(ptr + 4 * (width * row + col));
                        if (transparent.Contains(color))
                        {
                            Marshal.WriteByte(ptr + 4 * (width * row + col) + 3, 0x00);
                        }
                    }
                }

                // Get subimages
                foreach (var prop in props)
                {
                    for (int i = 0; i < prop.Value.Count; i++)
                    {
                        var oneProp = prop.Value[i];

                        oneProp.Image = Raylib.ImageFromImage(_img, oneProp.Rect);

                        // Get locations of flags (for units, cities)
                        // Row
                        for (int col = 0; col < oneProp.Rect.width; col++)
                        {
                            var color = Marshal.ReadInt32(ptr + 4 * (((int)oneProp.Rect.y - 1) * 
                                        _img.width + (int)oneProp.Rect.x + col));
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
                        for (int row = 0; row < oneProp.Rect.height; row++)
                        {
                            var color = Marshal.ReadInt32(ptr + 4 * (((int)oneProp.Rect.y + row) * 
                                        _img.width + (int)oneProp.Rect.x - 1));
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
        // BMP 8bpp
        else if (System.Text.Encoding.UTF8.GetString(bytes, 0, 2).Equals("BM") && BitConverter.ToInt16(bytes, 28) == 8)
        {
            var dataOffset = BitConverter.ToInt16(bytes, 10);
            var width = BitConverter.ToInt32(bytes, 18);
            var height = BitConverter.ToInt32(bytes, 22);

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
            int flag1color = BitConverter.ToInt32(new byte[]
                {
                    palette[250, 2],
                    palette[250, 1],
                    palette[250, 0],
                    0xff
                });
            int flag2color = BitConverter.ToInt32(new byte[]
                {
                    palette[249, 2],
                    palette[249, 1],
                    palette[249, 0],
                    0xff
                });

            var imgData = new byte[4 * width * height];
            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    var pos = dataOffset + width * (height - 1 - row) + col;
                    imgData[4 * (width * row + col) + 0] = palette[bytes[pos], 2];
                    imgData[4 * (width * row + col) + 1] = palette[bytes[pos], 1];
                    imgData[4 * (width * row + col) + 2] = palette[bytes[pos], 0];
                    if (bytes[pos] == 253 || bytes[pos] == 254 || bytes[pos] == 255)
                    {
                        imgData[4 * (width * row + col) + 3] = 0;
                    }
                    else
                    {
                        imgData[4 * (width * row + col) + 3] = 255;
                    }
                }
            }

            Image _img;
            unsafe
            {
                fixed (byte* ptr = imgData)
                {
                    _img = new Image
                    {
                        data = ptr,
                        format = PixelFormat.PIXELFORMAT_UNCOMPRESSED_R8G8B8A8,
                        width = width,
                        height = height,
                        mipmaps = 1
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
                        for (int col = 0; col < oneProp.Rect.width; col++)
                        {
                            var color = BitConverter.ToInt32(imgData, 
                                4 * (((int)oneProp.Rect.y - 1) * _img.width + (int)oneProp.Rect.x + col));
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
                        for (int row = 0; row < oneProp.Rect.height; row++)
                        {
                            var color = BitConverter.ToInt32(imgData,
                                4 * (((int)oneProp.Rect.y + row) * _img.width + (int)oneProp.Rect.x - 1));
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
            throw new Exception("Image file type not supported!");
        }
    }
}