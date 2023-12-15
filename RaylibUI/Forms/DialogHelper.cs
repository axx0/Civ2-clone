using Civ2engine;
using Model;
using Model.Interface;
using Raylib_cs;
using System.Numerics;

namespace RaylibUI.Forms;

public static class DialogHelper
{
    public static IList<FormattedText> GetFormattedTexts(IList<string> texts, IList<TextStyles> styles, IList<string> replaceStrings, IList<int> replaceNumbers)
    {
        // Group left-aligned texts
        int j = 0;
        while (j < texts.Count - 1)
        {
            j++;
            if (styles[j - 1] == TextStyles.Left && styles[j] == TextStyles.Left)
            {
                texts[j] = texts[j - 1] + " " + texts[j];
                texts.RemoveAt(j - 1);
                styles.RemoveAt(j - 1);
                j = 0;
            }
        }

        // Replace %STRING, %NUMBER
        texts = texts.Select(t => ReplacePlaceholders(t, replaceStrings, replaceNumbers)).ToList();
        texts = texts.Select(t => t.Replace("_", " ")).ToList();

        // Format texts
        var formattedTexts = new List<FormattedText>();
        int i = 0;
        foreach (var text in texts)
        {
            formattedTexts.Add(new FormattedText
            {
                Text = text == "" && styles[i] == TextStyles.LeftOwnLine ? " " : text,    // Add space if ^ is the only character
                //Font = new Font("Times New Roman", 18),
                HorizontalAlignment = styles[i] == TextStyles.Centered ? HorizontalAlignment.Center : HorizontalAlignment.Left
            });

            i++;
        }
        return formattedTexts;
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