using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using ExtensionMethods;
using System.Diagnostics;

namespace EtoFormsUI
{
    public class Civ2dialog_v2 : Dialog
    {
        private readonly int paddingTop, paddingBtm;
        private readonly RadioButtonList radioBtnList;
        private readonly FormattedText[] formattedOptionsTexts;
        private readonly PopupBox _popupBox;
        private readonly List<string> _replaceStrings;
        public int SelectedIndex;
        
        public Civ2dialog_v2(Main parent, PopupBox popupBox, List<string> replaceStrings = null)
        {
            _popupBox = popupBox;
            _replaceStrings = replaceStrings;

            foreach (MenuItem item in parent.Menu.Items) item.Enabled = false;

            paddingTop = 38;
            paddingBtm = 46;

            WindowStyle = WindowStyle.None;
            MovableByWindowBackground = true;

            // Replace %STRING in texts
            popupBox.Title = ReplaceSTRING(popupBox.Title, replaceStrings);
            if (popupBox.CenterText != null)
            {
                var copyList = new List<string>();
                foreach (var text in popupBox.CenterText) copyList.Add(ReplaceSTRING(text, replaceStrings));
                popupBox.CenterText = copyList;
            }
            if (popupBox.LeftText != null)
            {
                var copyList = new List<string>();
                foreach (var text in popupBox.LeftText) copyList.Add(ReplaceSTRING(text, replaceStrings));
                popupBox.LeftText = copyList;
            }

            // Determine size of inner panel
            int optionRows = popupBox.Options == null ? 0 : popupBox.Options.Count;
            int textRows = popupBox.CenterText == null ? 0 : popupBox.CenterText.Count;
            var innerSize = new Size(2 * 2 + MaxWidth(), 2 * 2 + optionRows * 32 + textRows * 30);
            Size = new Size(innerSize.Width + 2 * 11, innerSize.Height + paddingTop + paddingBtm);
            
            // Center the dialog on screen by default
            Location = new Point(parent.Width / 2 - this.Width / 2, parent.Height / 2 - this.Height / 2);

            var layout = new PixelLayout() { Size = new Size(this.Width, this.Height) };

            // Options (if they exist)
            if (popupBox.Options != null)
            {
                // Texts
                formattedOptionsTexts = new FormattedText[optionRows];
                for (int i = 0; i < optionRows; i++) formattedOptionsTexts[i] = new FormattedText() { Font = new Font("Times New Roman", 18), ForegroundBrush = new SolidBrush(Color.FromArgb(51, 51, 51)), Text = _popupBox.Options[i] };

                // Radio buttons (options)
                radioBtnList = new RadioButtonList() { DataStore = _popupBox.Options, Orientation = Orientation.Vertical };
                radioBtnList.SelectedIndexChanged += (sender, e) => Invalidate();
                radioBtnList.GotFocus += (sender, e) => Invalidate();
                layout.Add(radioBtnList, 11 + 10, 40);
            }

            // Drawable surface
            var surface = new Drawable() { Size = new Size(this.Width, this.Height), CanFocus = false };
            surface.Paint += Surface_Paint;
            layout.Add(surface, 0, 0);

            // Buttons
            var buttons = new Civ2button[_popupBox.Button.Count];
            int buttonWidth = (this.Width - 2 * 9 - 3 * (_popupBox.Button.Count - 1)) / _popupBox.Button.Count;
            for (int i = 0; i < _popupBox.Button.Count; i++)
            {
                buttons[i] = new Civ2button(_popupBox.Button[i], buttonWidth, 36, new Font("Times new roman", 11));
                layout.Add(buttons[i], 9 + buttonWidth * i + 3 * i, Height - 46);

                // Define abort button so that is also called with Esc
                if (_popupBox.Button[i] == "Cancel")
                {
                    AbortButton = buttons[i];
                    AbortButton.Click += (sender, e) =>
                    {
                        foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                        Application.Instance.Quit();
                    };
                }

                // Define default button so that it is also called with return key
                if (_popupBox.Button[i] == "OK")
                {
                    DefaultButton = buttons[i];
                    DefaultButton.Click += (sender, e) => 
                    { 
                        if (radioBtnList != null) SelectedIndex = radioBtnList.SelectedIndex; 
                        Close(); 
                    };
                }
            }

            Content = layout;
        }

        private void Surface_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.AntiAlias = false;

            // Paint outer wallpaper
            var imgSize = Images.PanelOuterWallpaper.Size;
            for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                {
                    e.Graphics.DrawImage(Images.PanelOuterWallpaper, col * imgSize.Width, row * imgSize.Height);
                }
            }

            // Paint panel borders
            // Outer border
            using var _pen1 = new Pen(Color.FromArgb(227, 227, 227));
            using var _pen2 = new Pen(Color.FromArgb(105, 105, 105));
            using var _pen3 = new Pen(Color.FromArgb(255, 255, 255));
            using var _pen4 = new Pen(Color.FromArgb(160, 160, 160));
            using var _pen5 = new Pen(Color.FromArgb(240, 240, 240));
            using var _pen6 = new Pen(Color.FromArgb(223, 223, 223));
            using var _pen7 = new Pen(Color.FromArgb(67, 67, 67));
            e.Graphics.DrawLine(_pen1, 0, 0, this.Width - 2, 0);   // 1st layer of border
            e.Graphics.DrawLine(_pen1, 0, 0, 0, this.Height - 2);
            e.Graphics.DrawLine(_pen2, this.Width - 1, 0, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen2, 0, this.Height - 1, this.Width - 1, this.Height - 1);
            e.Graphics.DrawLine(_pen3, 1, 1, this.Width - 3, 1);   // 2nd layer of border
            e.Graphics.DrawLine(_pen3, 1, 1, 1, this.Height - 3);
            e.Graphics.DrawLine(_pen4, this.Width - 2, 1, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen4, 1, this.Height - 2, this.Width - 2, this.Height - 2);
            e.Graphics.DrawLine(_pen5, 2, 2, this.Width - 4, 2);   // 3rd layer of border
            e.Graphics.DrawLine(_pen5, 2, 2, 2, this.Height - 4);
            e.Graphics.DrawLine(_pen5, this.Width - 3, 2, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen5, 2, this.Height - 3, this.Width - 3, this.Height - 3);
            e.Graphics.DrawLine(_pen6, 3, 3, this.Width - 5, 3);   // 4th layer of border
            e.Graphics.DrawLine(_pen6, 3, 3, 3, this.Height - 5);
            e.Graphics.DrawLine(_pen7, this.Width - 4, 3, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen7, 3, this.Height - 4, this.Width - 4, this.Height - 4);
            e.Graphics.DrawLine(_pen6, 4, 4, this.Width - 6, 4);   // 5th layer of border
            e.Graphics.DrawLine(_pen6, 4, 4, 4, this.Height - 6);
            e.Graphics.DrawLine(_pen7, this.Width - 5, 4, this.Width - 5, this.Height - 5);
            e.Graphics.DrawLine(_pen7, 4, this.Height - 5, this.Width - 5, this.Height - 5);

            // Inner border
            e.Graphics.DrawLine(_pen7, 9, paddingTop - 1, 9 + (Width - 18 - 1), paddingTop - 1);   // 1st layer of border
            e.Graphics.DrawLine(_pen7, 10, paddingTop - 1, 10, Height - paddingBtm - 1);
            e.Graphics.DrawLine(_pen6, Width - 11, paddingTop - 1, Width - 11, Height - paddingBtm - 1);
            e.Graphics.DrawLine(_pen6, 9, Height - paddingBtm, Width - 9 - 1, Height - paddingBtm);
            e.Graphics.DrawLine(_pen7, 10, paddingTop - 2, 9 + (Width - 18 - 2), paddingTop - 2);   // 2nd layer of border
            e.Graphics.DrawLine(_pen7, 9, paddingTop - 2, 9, Height - paddingBtm);
            e.Graphics.DrawLine(_pen6, Width - 10, paddingTop - 2, Width - 10, Height - paddingBtm);
            e.Graphics.DrawLine(_pen6, 9, Height - paddingBtm + 1, Width - 9 - 1, Height - paddingBtm + 1);

            // Paint inner wallpaper
            imgSize = Images.PanelInnerWallpaper.Size;
            Rectangle rectS;
            for (int row = 0; row < (this.Height - paddingTop - paddingBtm) / imgSize.Height + 1; row++)
            {
                for (int col = 0; col < (this.Width - 2 * 11) / imgSize.Width + 1; col++)
                {
                    rectS = new Rectangle(0, 0, Math.Min(this.Width - 2 * 11 - col * imgSize.Width, imgSize.Width), Math.Min(this.Height - paddingBtm - paddingTop - row * imgSize.Height, imgSize.Height));
                    e.Graphics.DrawImage(Images.PanelInnerWallpaper, rectS, new Point(col * imgSize.Width + 11, row * imgSize.Height + paddingTop));
                }
            }

            // Title
            Draw.Text(e.Graphics, _popupBox.Title, new Font("Times new roman", 17, FontStyle.Bold), Color.FromArgb(135, 135, 135), new Point(this.Width / 2, paddingTop / 2), true, true, Colors.Black, 1, 1);

            int y_offset = 0;

            // Centered text
            int rowCount;
            if (_popupBox.CenterText != null)
            {
                rowCount = 0;
                foreach (var text in _popupBox.CenterText)
                {
                    Draw.Text(e.Graphics, _popupBox.CenterText[rowCount], new Font("Times new roman", 18), Color.FromArgb(51, 51, 51), new Point(this.Width / 2, paddingTop + 5 + y_offset), true, false, Color.FromArgb(191, 191, 191), 1, 1);

                    y_offset += 30;
                    rowCount++;
                }
            }

            // Options (if they exist)
            if (_popupBox.Options != null)
            {
                rowCount = 0;
                foreach (var option in _popupBox.Options)
                {
                    Draw.RadioBtn(e.Graphics, radioBtnList.SelectedIndex == rowCount, new Point(21, paddingTop + 9 + y_offset));

                    e.Graphics.DrawText(formattedOptionsTexts[rowCount], new Point(47, paddingTop + 5 + y_offset));

                    using var _pen = new Pen(Color.FromArgb(64, 64, 64));
                    if (radioBtnList.SelectedIndex == rowCount) e.Graphics.DrawRectangle(_pen, new Rectangle(45, paddingTop + 5 + y_offset, this.Width - 45 - 14, 26));

                    y_offset += 32;
                    rowCount++;
                }
            }
        }

        // Determine max width of panel
        private int MaxWidth()
        {
            // 1) Input from Game.txt
            int width1 = (int)(_popupBox.Width * 1.5);

            // 2) Max length of strings
            // First title
            var titleText = new FormattedText { Text = _popupBox.Title, Font = new Font("Times new roman", 17, FontStyle.Bold) };
            int width2 = (int)titleText.Measure().Width;
            // Then options strings
            foreach (var text in _popupBox.Options ?? Enumerable.Empty<string>())
            {
                var textWidthCandidate = (int)(new FormattedText { Text = text, Font = new Font("Times new roman", 18) }.Measure().Width);
                var widthCandidate = textWidthCandidate + 32;   // Count in width of radio button
                if (widthCandidate > width2) width2 = widthCandidate;
            }

            return Math.Max(width1, width2);
        }

        // Find occurences of %STRING in text and replace it with strings
        private string ReplaceSTRING(string text, List<string> replacementStrings)
        {
            int replStringNo, pos;
            while (text.Contains("%STRING"))
            {
                pos = text.IndexOf("%STRING") + 7; // %STRING number position
                replStringNo = (int)Char.GetNumericValue(text[pos]);  // Get number x in %STRINGx
                text = text.Replace("%STRING".Insert(7, replStringNo.ToString()), replacementStrings[replStringNo]);
            }
            return text;
        }
    }
}
