using Model;
using Raylib_cs;

namespace RaylibUI.Forms;

public static class CtrlHelpers
{
    // Return a list of strings for wrapped text
    public static List<string> GetWrappedTexts(IUserInterface active, string text, int maxWidth, int fontSize)
    {
        string[] words = text.Split();
        List<string> wrappedLines = new List<string>();

        string combinedWord = string.Empty;
        int combinedTextSize, combinedTextSizeNext;
        // Measure string Width by combining words
        bool newLine = true;
        for (int i = 0; i < words.Length; i++)
        {
            if (newLine)
            {
                combinedWord = words[i];
                newLine = false;
            }
            else
            {
                combinedWord = combinedWord + " " + words[i];
            }

            combinedTextSize = (int)Raylib.MeasureTextEx(active.Look.DefaultFont, combinedWord, fontSize, 1.0f).X;
            if (i != words.Length - 1)
            {
                combinedTextSizeNext = (int)Raylib.MeasureTextEx(active.Look.DefaultFont, combinedWord + " " + words[i + 1], fontSize, 1.0f).X;
            }
            else    // Last word
            {
                combinedTextSizeNext = combinedTextSize;
            }


            if (combinedTextSize < maxWidth && combinedTextSizeNext >= maxWidth)
            {
                wrappedLines.Add(combinedWord);
                newLine = true;
            }

            // Last line
            if (combinedTextSize < maxWidth && i == words.Length - 1)
            {
                wrappedLines.Add(combinedWord);
            }
        }

        int[] wrappedLinesLength = new int[wrappedLines.Count];
        for (int i = 0; i < wrappedLines.Count; i++)
        {
            wrappedLinesLength[i] = (int)Raylib.MeasureTextEx(active.Look.DefaultFont, wrappedLines[i], fontSize, 1.0f).X;
        }

        return wrappedLines;
    }
}
