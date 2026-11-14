using Model;
using Raylib_CSharp.Fonts;

namespace RaylibUI;

public static class DialogUtils
{
    /// <summary>
    /// Splits text into lines narrower than maxWidth, breaking between words.
    /// No word is ever dropped: a word wider than maxWidth on its own gets a line of its own
    /// (and sticks out), and the last line is always kept.
    /// </summary>
    public static List<string> GetWrappedTexts(string text, int maxWidth, Font font, int fontSize)
    {
        string[] words = text.Split();
        List<string> wrappedLines = [];

        var line = words[0];
        for (int i = 1; i < words.Length; i++)
        {
            var extended = line + " " + words[i];
            if ((int)TextManager.MeasureTextEx(font, extended, fontSize, 0.0f).X < maxWidth)
            {
                line = extended;
            }
            else
            {
                // The next word doesn't fit: close this line (it has at least one word) and start a new one
                wrappedLines.Add(line);
                line = words[i];
            }
        }
        wrappedLines.Add(line);

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
