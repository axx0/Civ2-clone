using Raylib_cs;

namespace RaylibUI.Forms;

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

            combinedTextSize = (int)Raylib.MeasureTextEx(Fonts.DefaultFont, combinedWord, fontSize, 1.0f).X;
            if (i != words.Length - 1)
            {
                combinedTextSizeNext = (int)Raylib.MeasureTextEx(Fonts.DefaultFont, combinedWord + " " + words[i + 1], fontSize, 1.0f).X;
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
            wrapped_lines_length[i] = (int)Raylib.MeasureTextEx(Fonts.DefaultFont, wrapped_lines[i], fontSize, 1.0f).X;
        }

        return wrapped_lines;
    }
}
