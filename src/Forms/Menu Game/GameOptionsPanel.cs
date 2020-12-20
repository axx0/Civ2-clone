using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace civ2.Forms
{
    public partial class GameOptionsPanel : Civ2panel
    {
        Game Game => Game.Instance;

        private readonly Main Main;
        private readonly Civ2button _OKButton, _cancelButton;
        private readonly List<DoubleBufferedPanel> _clickPanels;
        private readonly string[] _textOptions;
        private readonly bool[] _choiceOptions;        

        public GameOptionsPanel(Main parent, int _width, int _height) : base(_width, _height, "Civilization II Multiplayer Gold", true)
        {
            Main = parent;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;

            // OK button
            _OKButton = new Civ2button
            {
                Location = new Point(9, 398),
                Size = new Size(363, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(_OKButton);
            _OKButton.Click += new EventHandler(OKButton_Click);

            // Cancel button
            _cancelButton = new Civ2button
            {
                Location = new Point(374, 398),
                Size = new Size(363, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(_cancelButton);
            _cancelButton.Click += new EventHandler(CancelButton_Click);

            // Make an options array
            _choiceOptions = new bool[11] { Game.Options.SoundEffects, Game.Options.Music, Game.Options.AlwaysWaitAtEndOfTurn, Game.Options.AutosaveEachTurn,
                Game.Options.ShowEnemyMoves, Game.Options.NoPauseAfterEnemyMoves, Game.Options.FastPieceSlide, Game.Options.InstantAdvice, Game.Options.TutorialHelp,
                Game.Options.MoveUnitsWithoutMouse, Game.Options.EnterClosestCityScreen };
            // Individual options text
            _textOptions = new string[11] { "Sound Effects", "Music", "Always wait at end of turn.", "Autosave each turn.", "Show enemy moves.", "No pause after enemy moves.", 
                "Fast piece slide.", "Instant advice.", "Tutorial help.", "Move units w/ mouse (cursor arrows).", "ENTER key closes City Screen." };
            // Make click panels for each options
            _clickPanels = new List<DoubleBufferedPanel>();
            for (int i = 0; i < 11; i++)
            {
                DoubleBufferedPanel panel = new DoubleBufferedPanel
                {
                    Location = new Point(10, 32 * i + 4),
                    Size = new Size(100, 27),   // You will set the correct width once you measure text size below
                    BackColor = Color.Transparent
                };
                DrawPanel.Controls.Add(panel);
                panel.Click += new EventHandler(ClickPanels_Click);
                _clickPanels.Add(panel);
            }
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Show all options
            SizeF[] stringSize = new SizeF[11];
            for (int row = 0; row < 11; row++)
            {
                // Text                
                e.Graphics.DrawString(_textOptions[row], new Font("Times New Roman", 18), new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(36, 32 * row + 4));  // Text of option
                stringSize[row] = e.Graphics.MeasureString(_textOptions[row], new Font("Times New Roman", 18));  // Measure size of text
                _clickPanels[row].Size = new Size(30 + (int)(stringSize[row].Width), _clickPanels[row].Height);   // Set the correct size of click panel

                // Draw checkbox
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(13, 8 + 32 * row, 15, 17));
                e.Graphics.FillRectangle(new SolidBrush(Color.White), new Rectangle(12, 9 + 32 * row, 17, 15));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 128, 128)), new Rectangle(14, 9 + 32 * row, 13, 15));
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 128, 128)), new Rectangle(13, 10 + 32 * row, 15, 13));
                e.Graphics.DrawLine(new Pen(Color.Black), 14, 9 + 32 * row, 26, 9 + 32 * row);
                e.Graphics.DrawLine(new Pen(Color.Black), 14, 25 + 32 * row, 27, 25 + 32 * row);
                e.Graphics.DrawLine(new Pen(Color.Black), 13, 10 + 32 * row, 13, 22 + 32 * row);
                e.Graphics.DrawLine(new Pen(Color.Black), 29, 10 + 32 * row, 29, 24 + 32 * row);
                e.Graphics.DrawLine(new Pen(Color.Black), 13, 10 + 32 * row, 14, 10 + 32 * row);
                e.Graphics.DrawLine(new Pen(Color.Black), 28, 24 + 32 * row, 28, 25 + 32 * row);

                // Draw check marks
                if (_choiceOptions[row])
                {
                    e.Graphics.DrawString("ü", new Font("Wingdings", 18), new SolidBrush(Color.Black), new Point(10 + 1, 32 * row + 3 + 2));
                    e.Graphics.DrawString("ü", new Font("Wingdings", 18), new SolidBrush(Color.FromArgb(192, 192, 192)), new Point(10, 32 * row + 3));
                }
            }
        }

        // If OK is pressed --> update the options and close
        private void OKButton_Click(object sender, EventArgs e)
        {
            Game.Options.SoundEffects = _choiceOptions[0];
            Game.Options.Music = _choiceOptions[1];
            Game.Options.AlwaysWaitAtEndOfTurn = _choiceOptions[2];
            Game.Options.AutosaveEachTurn = _choiceOptions[3];
            Game.Options.ShowEnemyMoves = _choiceOptions[4];
            Game.Options.NoPauseAfterEnemyMoves = _choiceOptions[5];
            Game.Options.FastPieceSlide = _choiceOptions[6];
            Game.Options.InstantAdvice = _choiceOptions[7];
            Game.Options.TutorialHelp = _choiceOptions[8];
            Game.Options.MoveUnitsWithoutMouse = _choiceOptions[9];
            Game.Options.EnterClosestCityScreen = _choiceOptions[10];
            this.Visible = false;
            this.Dispose();
        }

        // If cancel is pressed --> just close
        private void CancelButton_Click(object sender, EventArgs e) 
        {
            this.Visible = false;
            this.Dispose();
        }

        // When an option is clicked, update the bool array of options
        private void ClickPanels_Click(object sender, EventArgs e)
        {
            Control clickedControl = (Control)sender;   // Sender gives you which control is clicked.
            int index = _clickPanels.FindIndex(a => a == clickedControl);    // Which control is clicked in a list
            _choiceOptions[index] = !_choiceOptions[index];
            DrawPanel.Invalidate();
        }
    }
}
