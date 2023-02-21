using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Controls;

public static class CtrlHelpers
{
    // Return a list of strings for wrapped text
    public static List<string> GetWrappedTexts(string text, int max_width, int fontSize)
    {
        string[] words = text.Split();
        List<string> wrapped_lines = new List<string>();

        string combinedWord = string.Empty;
        int combinedTextSize, combinedTextSizeNext;
        // Measure string width by combining words
        bool new_line = true;
        for (int i = 0; i < words.Length; i++)
        {
            if (new_line)
            {
                combinedWord = words[i];
                new_line = false;
            }
            else
            {
                combinedWord = combinedWord + " " + words[i];
            }

            combinedTextSize = Raylib.MeasureText(combinedWord, fontSize);
            if (i != words.Length - 1)
            {
                combinedTextSizeNext = Raylib.MeasureText(combinedWord + " " + words[i + 1], fontSize);
            }
            else    // Last word
            {
                combinedTextSizeNext = combinedTextSize;
            }


            if (combinedTextSize < max_width && combinedTextSizeNext >= max_width)
            {
                wrapped_lines.Add(combinedWord);
                new_line = true;
            }

            // Last line
            if (combinedTextSize < max_width && i == words.Length - 1)
            {
                wrapped_lines.Add(combinedWord);
            }
        }

        int[] wrapped_lines_length = new int[wrapped_lines.Count];
        for (int i = 0; i < wrapped_lines.Count; i++)
        {
            wrapped_lines_length[i] = Raylib.MeasureText(wrapped_lines[i], fontSize);
        }

        return wrapped_lines;
    }

    // Return height of lines of text
    private static int MeasureLinesHeight(List<string> lines, int fontSize)
    {
        int height = 0;
        foreach (var line in lines)
        {
            height += (int)Raylib.MeasureTextEx(Raylib.GetFontDefault(), line, fontSize, 1.0f).Y;
        }

        return height;
    }

    public static void DrawWrappedText(string text, Vector2 pos, int width, int fontSize)
    {
        var lines = GetWrappedTexts(text, width, fontSize);

        Vector2 linePos;
        int lineHeight = 0;
        foreach (var line in lines)
        {
            linePos = new Vector2(pos.X, pos.Y + lineHeight);
            Raylib.DrawTextEx(Raylib.GetFontDefault(), line, linePos, fontSize, 1.0f, Color.BLACK);
            lineHeight += (int)Raylib.MeasureTextEx(Raylib.GetFontDefault(), line, fontSize, 1.0f).Y;
        }
    }
}
