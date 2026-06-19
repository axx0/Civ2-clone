using Civ2engine;
using Raylib_CSharp.Images;
using Raylib_CSharp.Colors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.Unicode;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RaylibUtils;

public static partial class Images
{
    /// <summary>
    /// Load image from file and make colours transparent
    /// </summary>
    public static Image_and_bpp LoadImageFromFile(string filename, int dataStart = 0, int length = 0)
    {
        byte[] bytes;
        if (length != 0)
        {
            bytes = new byte[length];
            Array.Copy(File.ReadAllBytes(filename), dataStart, bytes, 0, length);
        }
        else
        {
            bytes = File.ReadAllBytes(filename);
        }
        Image img = new();
        int bpp = 0; // colour depth

        // 8bpp GI
        // transparent colours are defined in the palette
        if (System.Text.Encoding.UTF8.GetString(bytes, 0, 3).Equals("GIF"))
        {
            bpp = 8;

            // Colors are 32bit int uncompressed (4x8 bits = B+G+R+A)
            int width = BitConverter.ToInt16(bytes, 6);
            int height = BitConverter.ToInt16(bytes, 8);

            // Let raylib deal with LZW decompression
            img = Image.LoadFromMemory(".gif", bytes);

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

                img.ReplaceColor(transparent[i], new Color(transparent[i].R, transparent[i].G, transparent[i].B, 0));
            }
        }
        // BMP
        else if (System.Text.Encoding.UTF8.GetString(bytes, 0, 2).Equals("BM"))
        {
            var dataOffset = BitConverter.ToInt16(bytes, 10);
            var width = BitConverter.ToInt32(bytes, 18);
            var height = BitConverter.ToInt32(bytes, 22);
            bpp = BitConverter.ToInt16(bytes, 28);
            var size = BitConverter.ToInt32(bytes, 34);
            if (size == 0)
                size = bytes.Length - dataOffset;
            var extraBits = size / height - bpp / 8 * width;
            var imgData = new byte[4 * width * height];
            int flag1Color = Convert.ToInt32("0x0000FFFF", 16);
            int flag2Color = Convert.ToInt32("0xFF9C00FF", 16);

            switch (bpp)
            {
                // 8bpp
                // Three transparent colours are defined in the palette
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

                        // Assign colours from palette
                        for (int row = 0; row < height; row++)
                        {
                            for (int col = 0; col < width; col++)
                            {
                                var pos = dataOffset + width * (height - 1 - row) + col;
                                imgData[4 * (width * row + col) + 0] = palette[bytes[pos], 2];
                                imgData[4 * (width * row + col) + 1] = palette[bytes[pos], 1];
                                imgData[4 * (width * row + col) + 2] = palette[bytes[pos], 0];

                                // Transparent
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
                    }
                    break;

                // 16+24bpp have no palette
                // violet is always transparent
                // some images have additional upper-left transparency pixel
                case 16:
                case 24:
                    {
                        for (int row = 0; row < height; row++)
                        {
                            for (int col = 0; col < width; col++)
                            {
                                if (bpp == 16)
                                {
                                    var _15bitrgb = Get15BitRgb(bytes, dataOffset +
                                                    (2 * width + extraBits) * (height - 1 - row) + 2 * col);
                                    imgData[4 * (width * row + col) + 0] = (byte)(_15bitrgb[2] * 255 / 31);
                                    imgData[4 * (width * row + col) + 1] = (byte)(_15bitrgb[1] * 255 / 31);
                                    imgData[4 * (width * row + col) + 2] = (byte)(_15bitrgb[0] * 255 / 31);
                                    imgData[4 * (width * row + col) + 3] = 255;
                                }
                                else
                                {
                                    var off = dataOffset + (3 * width + extraBits) * (height - 1 - row) + 3 * col;
                                    imgData[4 * (width * row + col) + 0] = bytes[off + 2];
                                    imgData[4 * (width * row + col) + 1] = bytes[off + 1];
                                    imgData[4 * (width * row + col) + 2] = bytes[off + 0];
                                    imgData[4 * (width * row + col) + 3] = 255;
                                }
                                // Set alpha transparent for pink
                                if (imgData[4 * (width * row + col) + 0] == 255 &&
                                    imgData[4 * (width * row + col) + 1] == 0 &&
                                    imgData[4 * (width * row + col) + 2] == 255)
                                {
                                    imgData[4 * (width * row + col) + 3] = 0;
                                }
                            }
                        }
                    }
                    break;

                default: throw new ArgumentOutOfRangeException();
            };

            Image img2; 
            unsafe
            {
                fixed (byte* ptr = imgData)
                {
                    img2 = new Image
                    {
                        Data = (nint)ptr,
                        Format = PixelFormat.UncompressedR8G8B8A8,
                        Width = width,
                        Height = height,
                        Mipmaps = 1
                    };
                }
            }
            img = img2.Copy();
        }
        // PNG
        else if (System.Text.Encoding.UTF8.GetString(bytes, 1, 3).Equals("PNG"))
        {
            bpp = 32;
            img = Image.LoadFromMemory(Path.GetExtension(filename).ToLowerInvariant(), bytes);
            img.ReplaceColor(new Color(255, 0, 255, 255), new Color(255, 0, 255, 0));
        }
        // JPG/JPEG
        else if (bytes.Length > 2 && bytes[0] == 0xff && bytes[1] == 0xd8)
        {
            bpp = 24;

            // The FOSSart JPEGs are progressive JPEGs. The packaged raylib native
            // loader rejects those and prints repeated noisy "IMAGE: Data format not
            // supported" warnings. Skip raylib for progressive JPEGs and decode them
            // through common system decoders instead.
            if (IsProgressiveJpeg(bytes))
            {
                if (!TryLoadJpegWithExternalDecoder(filename, out img))
                {
                    img = Image.GenColor(1, 1, new Color(0, 0, 0, 0));
                }
            }
            else
            {
                img = Image.LoadFromMemory(Path.GetExtension(filename).ToLowerInvariant(), bytes);
                if ((img.Width <= 0 || img.Height <= 0) && !TryLoadJpegWithExternalDecoder(filename, out img))
                {
                    img = Image.GenColor(1, 1, new Color(0, 0, 0, 0));
                }
            }
        }

        return new Image_and_bpp
        {
            Image = img,
            ColourDepth = bpp
        };
    }



    private static bool TryLoadJpegWithExternalDecoder(string filename, out Image image)
    {
        image = new Image();

        // Try several common decoders. Any of these is enough to turn the
        // progressive JPEG into a simple PPM stream that this file can parse
        // without adding a new NuGet dependency.
        if (TryDecodeJpegToPpm(["djpeg", "-ppm", "-outfile", "-", filename], out var ppmBytes) ||
            TryDecodeJpegToPpm(["ffmpeg", "-v", "error", "-i", filename, "-f", "image2pipe", "-vcodec", "ppm", "-"], out ppmBytes) ||
            TryDecodeJpegToPpm(["magick", filename, "ppm:-"], out ppmBytes) ||
            TryDecodeJpegToPpm(["convert", filename, "ppm:-"], out ppmBytes) ||
            TryDecodeJpegToPpm(["python3", "-c",
                "from PIL import Image; import sys; im=Image.open(sys.argv[1]).convert('RGB'); sys.stdout.buffer.write(('P6\\n%d %d\\n255\\n' % im.size).encode('ascii')); sys.stdout.buffer.write(im.tobytes())",
                filename], out ppmBytes))
        {
            return TryCreateImageFromPpm(ppmBytes, out image);
        }

        return false;
    }

    private static bool TryDecodeJpegToPpm(string[] command, out byte[] ppmBytes)
    {
        ppmBytes = [];
        if (command.Length == 0)
        {
            return false;
        }

        try
        {
            var startInfo = new ProcessStartInfo(command[0])
            {
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            for (var i = 1; i < command.Length; i++)
            {
                startInfo.ArgumentList.Add(command[i]);
            }

            using var process = Process.Start(startInfo);
            if (process == null)
            {
                return false;
            }

            using var output = new MemoryStream();
            process.StandardOutput.BaseStream.CopyTo(output);
            _ = process.StandardError.ReadToEnd();

            if (!process.WaitForExit(15000))
            {
                try
                {
                    process.Kill(entireProcessTree: true);
                }
                catch
                {
                    // ignored
                }
                return false;
            }

            if (process.ExitCode != 0)
            {
                return false;
            }

            ppmBytes = output.ToArray();
            return ppmBytes.Length > 0;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryCreateImageFromPpm(byte[] ppmBytes, out Image image)
    {
        image = new Image();
        var offset = 0;
        if (!TryReadPpmToken(ppmBytes, ref offset, out var magic) || magic != "P6" ||
            !TryReadPpmToken(ppmBytes, ref offset, out var widthText) ||
            !TryReadPpmToken(ppmBytes, ref offset, out var heightText) ||
            !TryReadPpmToken(ppmBytes, ref offset, out var maxText) ||
            !int.TryParse(widthText, out var width) ||
            !int.TryParse(heightText, out var height) ||
            !int.TryParse(maxText, out var maxValue) ||
            width <= 0 || height <= 0 || maxValue <= 0)
        {
            return false;
        }

        if (offset < ppmBytes.Length && char.IsWhiteSpace((char)ppmBytes[offset]))
        {
            offset++;
        }

        var requiredRgbBytes = checked(width * height * 3);
        if (ppmBytes.Length - offset < requiredRgbBytes)
        {
            return false;
        }

        var rgbaBytes = new byte[width * height * 4];
        for (var source = offset; source < offset + requiredRgbBytes; source += 3)
        {
            var target = ((source - offset) / 3) * 4;
            rgbaBytes[target] = ScalePpmChannel(ppmBytes[source], maxValue);
            rgbaBytes[target + 1] = ScalePpmChannel(ppmBytes[source + 1], maxValue);
            rgbaBytes[target + 2] = ScalePpmChannel(ppmBytes[source + 2], maxValue);
            rgbaBytes[target + 3] = 255;
        }

        image = CreateImageFromRgbaBytes(rgbaBytes, width, height);
        return image.Width > 0 && image.Height > 0;
    }

    private static bool TryReadPpmToken(byte[] bytes, ref int offset, out string token)
    {
        token = string.Empty;

        while (offset < bytes.Length)
        {
            var value = bytes[offset];
            if (value == '#')
            {
                while (offset < bytes.Length && bytes[offset] != '\n')
                {
                    offset++;
                }
                continue;
            }

            if (!char.IsWhiteSpace((char)value))
            {
                break;
            }

            offset++;
        }

        if (offset >= bytes.Length)
        {
            return false;
        }

        var start = offset;
        while (offset < bytes.Length && !char.IsWhiteSpace((char)bytes[offset]))
        {
            offset++;
        }

        token = System.Text.Encoding.ASCII.GetString(bytes, start, offset - start);
        return token.Length > 0;
    }

    private static byte ScalePpmChannel(byte value, int maxValue)
    {
        return maxValue == 255 ? value : (byte)Math.Clamp(value * 255 / maxValue, 0, 255);
    }

    private static Image CreateImageFromRgbaBytes(byte[] rgbaBytes, int width, int height)
    {
        unsafe
        {
            fixed (byte* ptr = rgbaBytes)
            {
                var image = new Image
                {
                    Data = (nint)ptr,
                    Format = PixelFormat.UncompressedR8G8B8A8,
                    Width = width,
                    Height = height,
                    Mipmaps = 1
                };
                return image.Copy();
            }
        }
    }

    private static bool IsProgressiveJpeg(byte[] bytes)
    {
        var index = 2;

        while (index + 3 < bytes.Length)
        {
            if (bytes[index] != 0xff)
            {
                index++;
                continue;
            }

            while (index < bytes.Length && bytes[index] == 0xff)
            {
                index++;
            }

            if (index >= bytes.Length)
            {
                return false;
            }

            var marker = bytes[index++];

            if (marker == 0xc2 || marker == 0xc6 || marker == 0xca || marker == 0xce)
            {
                return true;
            }

            if (marker == 0xd8 || marker == 0xd9 || (marker >= 0xd0 && marker <= 0xd7) || marker == 0x01)
            {
                continue;
            }

            if (index + 1 >= bytes.Length)
            {
                return false;
            }

            var segmentLength = (bytes[index] << 8) | bytes[index + 1];
            if (segmentLength < 2)
            {
                return false;
            }

            index += segmentLength;
        }

        return false;
    }

    /// <summary>
    /// Get images and flag locs from GIF/BMP
    /// </summary>
    //public static void LoadPropertiesFromPic(string filename, Dictionary<string, List<ImageProps>> props)
    //{
    //    byte[] bytes = File.ReadAllBytes(filename);

    //    if (System.Text.Encoding.UTF8.GetString(bytes, 0, 3).Equals("GIF"))
    //    {
    //        // Colors are 32bit int uncompressed (4x8 bits = B+G+R+A)
    //        int width = BitConverter.ToInt16(bytes, 6);
    //        int height = BitConverter.ToInt16(bytes, 8);

    //        // Let raylib deal with LZW decompression
    //        var img = Raylib.LoadImageFromMemory(Path.GetExtension(filename).ToLowerInvariant(), bytes);

    //        // Get 3 transparent colours from palette and replace colours
    //        Color[] transparent = new Color[3];
    //        for (int i = 0; i < 3; i++)
    //        {
    //            transparent[i] = new Color()
    //            {
    //                A = 0xff,
    //                R = bytes[13 + 3 * (253 + i) + 0],
    //                G = bytes[13 + 3 * (253 + i) + 1],
    //                B = bytes[13 + 3 * (253 + i) + 2],
    //            };

    //            Raylib.ImageColorReplace(ref img, transparent[i],
    //                new Color(transparent[i].R, transparent[i].G, transparent[i].B, (byte)0));
    //        }

    //        // Get two flag colors from palette
    //        int flag1Color = BitConverter.ToInt32(new byte[]
    //        {
    //            bytes[13 + 3 * 250 + 0],
    //            bytes[13 + 3 * 250 + 1],
    //            bytes[13 + 3 * 250 + 2],
    //            0xff
    //        });
    //        int flag2Color = BitConverter.ToInt32(new byte[]
    //        {
    //            bytes[13 + 3 * 249 + 0],
    //            bytes[13 + 3 * 249 + 1],
    //            bytes[13 + 3 * 249 + 2],
    //            0xff
    //        });

    //        unsafe
    //        {
    //            IntPtr ptr = (IntPtr)img.Data;
                
    //            // Get subimages
    //            foreach (var prop in props)
    //            {
    //                for (int i = 0; i < prop.Value.Count; i++)
    //                {
    //                    var oneProp = prop.Value[i];

    //                    oneProp.Image = Raylib.ImageFromImage(img, oneProp.Rect);

    //                    // Get locations of flags (for units, cities)
    //                    // Row
    //                    for (int col = 0; col < oneProp.Rect.Width; col++)
    //                    {
    //                        var color = Marshal.ReadInt32(ptr + 4 * (((int)oneProp.Rect.Y - 1) *
    //                                    img.Width + (int)oneProp.Rect.X + col));
    //                        if (color == flag1Color)    // units flag (blue)
    //                        {
    //                            oneProp.Flag1X = col;
    //                        }
    //                        else if (color == flag2Color)   // cities color (orange)
    //                        {
    //                            oneProp.Flag2X = col;
    //                        }
    //                    }

    //                    // Column
    //                    for (int row = 0; row < oneProp.Rect.Height; row++)
    //                    {
    //                        var color = Marshal.ReadInt32(ptr + 4 * (((int)oneProp.Rect.Y + row) *
    //                                    img.Width + (int)oneProp.Rect.X - 1));
    //                        if (color == flag1Color)
    //                        {
    //                            oneProp.Flag1Y = row;
    //                        }
    //                        else if (color == flag2Color)
    //                        {
    //                            oneProp.Flag2Y = row;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        Raylib.UnloadImage(img);
    //    }
    //    // BMP
    //    else if (System.Text.Encoding.UTF8.GetString(bytes, 0, 2).Equals("BM"))
    //    {
    //        var dataOffset = BitConverter.ToInt16(bytes, 10);
    //        var width = BitConverter.ToInt32(bytes, 18);
    //        var height = BitConverter.ToInt32(bytes, 22);
    //        var bpp = BitConverter.ToInt16(bytes, 28);
    //        var imgData = new byte[4 * width * height];
    //        int flag1Color = Convert.ToInt32("0x0000FFFF", 16);
    //        int flag2Color = Convert.ToInt32("0xFF9C00FF", 16);

    //        switch (bpp)
    //        {
    //            // 8bpp uses palette
    //            case 8:
    //                {
    //                    // Get palette
    //                    byte[,] palette = new byte[256, 4];
    //                    for (int i = 0; i < 256; i++)
    //                    {
    //                        palette[i, 0] = bytes[54 + 4 * i + 0];  // R
    //                        palette[i, 1] = bytes[54 + 4 * i + 1];  // G
    //                        palette[i, 2] = bytes[54 + 4 * i + 2];  // B
    //                        palette[i, 3] = bytes[54 + 4 * i + 3];  //
    //                    }

    //                    // Get two flag colors from palette
    //                    flag1Color = BitConverter.ToInt32(new byte[]
    //                    {
    //                        palette[250, 2],
    //                        palette[250, 1],
    //                        palette[250, 0],
    //                        0xff
    //                    });
    //                    flag2Color = BitConverter.ToInt32(new byte[]
    //                    {
    //                        palette[249, 2],
    //                        palette[249, 1],
    //                        palette[249, 0],
    //                        0xff
    //                    });

    //                    for (int row = 0; row < height; row++)
    //                    {
    //                        for (int col = 0; col < width; col++)
    //                        {
    //                            var pos = dataOffset + width * (height - 1 - row) + col;
    //                            imgData[4 * (width * row + col) + 0] = palette[bytes[pos], 2];
    //                            imgData[4 * (width * row + col) + 1] = palette[bytes[pos], 1];
    //                            imgData[4 * (width * row + col) + 2] = palette[bytes[pos], 0];
    //                            if (bytes[pos] == 253 || bytes[pos] == 254 || bytes[pos] == 255)
    //                            {
    //                                imgData[4 * (width * row + col) + 3] = 0;
    //                            }
    //                            else
    //                            {
    //                                imgData[4 * (width * row + col) + 3] = 255;
    //                            }
    //                        }
    //                    }
    //                }
    //                break;

    //            // 16+24bpp have no palette
    //            case 16:
    //            case 24:
    //                {
    //                    for (int row = 0; row < height; row++)
    //                    {
    //                        for (int col = 0; col < width; col++)
    //                        {
    //                            if (bpp == 16)
    //                            {
    //                                var _15bitrgb = Get15BitRgb(bytes, dataOffset + 
    //                                                2 * width * (height - 1 - row) + 2 * col);
    //                                imgData[4 * (width * row + col) + 0] = (byte)(_15bitrgb[2] * 255 / 31);
    //                                imgData[4 * (width * row + col) + 1] = (byte)(_15bitrgb[1] * 255 / 31);
    //                                imgData[4 * (width * row + col) + 2] = (byte)(_15bitrgb[0] * 255 / 31);
    //                                imgData[4 * (width * row + col) + 3] = 255;
    //                            }
    //                            else
    //                            {
    //                                var off = dataOffset + 3 * width * (height - 1 - row) + 3 * col;
    //                                imgData[4 * (width * row + col) + 0] = bytes[off + 2];
    //                                imgData[4 * (width * row + col) + 1] = bytes[off + 1];
    //                                imgData[4 * (width * row + col) + 2] = bytes[off + 0];
    //                                imgData[4 * (width * row + col) + 3] = 255;
    //                            }
    //                        }
    //                    }

    //                    // Make subimages transparent
    //                    // Pink + upper-left pixel in rectangle are transparent colors
    //                    var transparent1 = new byte[3] { 255, 0, 255 };
    //                    var transparent2 = new byte[3];
    //                    foreach (var prop in props)
    //                    {
    //                        for (int i = 0; i < prop.Value.Count; i++)
    //                        {
    //                            var rect = prop.Value[i].Rect;

    //                            // 2nd transparent color
    //                            transparent2 = new byte[3]
    //                            {
    //                                imgData[4 * (width * (int)rect.Y + (int)rect.X) + 0],
    //                                imgData[4 * (width * (int)rect.Y + (int)rect.X) + 1],
    //                                imgData[4 * (width * (int)rect.Y + (int)rect.X) + 2],
    //                            };

    //                            // Make part of image transparent
    //                            for (int row = (int)rect.Y; row < rect.Y + rect.Height; row++)
    //                            {
    //                                for (int col = (int)rect.X; col < rect.X + rect.Width; col++)
    //                                {
    //                                    if ((imgData[4 * (width * row + col) + 0] == transparent1[0] &&
    //                                         imgData[4 * (width * row + col) + 1] == transparent1[1] &&
    //                                         imgData[4 * (width * row + col) + 2] == transparent1[2]) ||
    //                                        (imgData[4 * (width * row + col) + 0] == transparent2[0] &&
    //                                         imgData[4 * (width * row + col) + 1] == transparent2[1] &&
    //                                         imgData[4 * (width * row + col) + 2] == transparent2[2]))
    //                                    {
    //                                        imgData[4 * (width * row + col) + 3] = 0;
    //                                    }
    //                                }
    //                            }
    //                        }
    //                    }
    //                }
    //                break;

    //            default: throw new ArgumentOutOfRangeException();

    //        };

    //        Image img;
    //        unsafe
    //        {
    //            fixed (byte* ptr = imgData)
    //            {
    //                img = new Image
    //                {
    //                    Data = ptr,
    //                    Format = PixelFormat.UncompressedR8G8B8A8,
    //                    Width = width,
    //                    Height = height,
    //                    Mipmaps = 1
    //                };
    //            }

    //            // Get subimages
    //            foreach (var prop in props)
    //            {
    //                for (int i = 0; i < prop.Value.Count; i++)
    //                {
    //                    var oneProp = prop.Value[i];

    //                    oneProp.Image = Raylib.ImageFromImage(img, oneProp.Rect);

    //                    // Get locations of flags(for units, cities)
    //                    // Row
    //                    for (int col = 0; col < oneProp.Rect.Width; col++)
    //                    {
    //                        var color = BitConverter.ToInt32(imgData,
    //                            4 * (((int)oneProp.Rect.Y - 1) * img.Width + (int)oneProp.Rect.X + col));
    //                        if (color == flag1Color)    // units flag (blue)
    //                        {
    //                            oneProp.Flag1X = col;
    //                        }
    //                        else if (color == flag2Color)   // cities color (orange)
    //                        {
    //                            oneProp.Flag2X = col;
    //                        }
    //                    }

    //                    // Column
    //                    for (int row = 0; row < oneProp.Rect.Height; row++)
    //                    {
    //                        var color = BitConverter.ToInt32(imgData,
    //                            4 * (((int)oneProp.Rect.Y + row) * img.Width + (int)oneProp.Rect.X - 1));
    //                        if (color == flag1Color)
    //                        {
    //                            oneProp.Flag1Y = row;
    //                        }
    //                        else if (color == flag2Color)
    //                        {
    //                            oneProp.Flag2Y = row;
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        //Raylib.UnloadImage(_img);
    //    }
    //    else
    //    {
    //        throw new Exception($"Image file type for {filename} not supported!");
    //    }
    //}

    private static int[] Get15BitRgb(byte[] byteArray, int offset)
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

public class Image_and_bpp
{
    public Image Image { get; set; }
    public int ColourDepth { get; set; }
}
