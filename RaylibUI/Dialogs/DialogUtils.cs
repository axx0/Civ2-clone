using Model;
using Raylib_CSharp.Fonts;

namespace RaylibUI;

public static class DialogUtils
{
    /// <summary>
    /// Returns a list of strings for wrapped text
    /// </summary>
    public static List<string> GetWrappedTexts(IUserInterface active, string text, int maxWidth, Font font, int fontSize)
    {
        string[] words = text.Split();
        List<string> wrappedLines = new ();

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

            combinedTextSize = (int)TextManager.MeasureTextEx(active.Look.DefaultFont, combinedWord, fontSize, 1.0f).X;
            if (i != words.Length - 1)
            {
                combinedTextSizeNext = (int)TextManager.MeasureTextEx(active.Look.DefaultFont, combinedWord + " " + words[i + 1], fontSize, 1.0f).X;
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
            wrappedLinesLength[i] = (int)TextManager.MeasureTextEx(active.Look.DefaultFont, wrappedLines[i], fontSize, 1.0f).X;
        }

        return wrappedLines;
    }

    /// <summary>
    /// Find occurences of %STRING and %NUMBER in text and replace it with other strings/numbers.
    /// </summary>
    /// <param name="text">Text where replacement takes place.</param>
    /// <param name="replacementStrings">A list of strings to replace %STRING0, %STRING1, %STRING2, etc.</param>
    /// <param name="replacementNumbers">A list of integers to replace %NUMBER0, %NUMBER1, %NUMBER2, etc.</param>
    public static string ReplacePlaceholders(string text, IList<string>? replacementStrings, IList<int>? replacementNumbers)
    {

        if (replacementStrings != null)
        {
            var index = text.IndexOf("%STRING", StringComparison.Ordinal);
            while (index != -1)
            {
                var numericChar = text[index + 7];
                text = text.Replace("%STRING" + numericChar,
                    replacementStrings[(int)char.GetNumericValue(numericChar)]);
                index = text.IndexOf("%STRING", StringComparison.Ordinal);
            }
        }

        if (replacementNumbers != null)
        {
            var index = text.IndexOf("%NUMBER", StringComparison.Ordinal);
            while (index != -1)
            {
                var numericChar = text[index + 7];
                text = text.Replace("%NUMBER" + numericChar,
                    replacementNumbers[(int)char.GetNumericValue(numericChar)].ToString());
                index = text.IndexOf("%NUMBER", StringComparison.Ordinal);
            }
        }

        return text;
    }
}
