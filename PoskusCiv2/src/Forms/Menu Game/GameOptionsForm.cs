using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Imagery;

namespace PoskusCiv2.Forms
{
    public partial class GameOptionsForm : Civ2form
    {
        DoubleBufferedPanel MainPanel;
        //DoubleBufferedPanel[] ClickPanels = new DoubleBufferedPanel[11];
        List<DoubleBufferedPanel> ClickPanels = new List<DoubleBufferedPanel>();
        string[] textOptions;
        bool[] choiceOptions;
        SizeF[] stringSize = new SizeF[11];

        public GameOptionsForm()
        {
            InitializeComponent();
            this.Paint += new PaintEventHandler(GameOptionsForm_Paint);

            //Main panel
            MainPanel = new DoubleBufferedPanel
            {
                Location = new Point(9, 36),
                Size = new Size(728, 360),
                BackgroundImage = Images.WallpaperStatusForm
            };
            Controls.Add(MainPanel);
            MainPanel.Paint += new PaintEventHandler(MainPanel_Paint);

            //OK button
            Civ2button OKButton = new Civ2button
            {
                Location = new Point(9, 398),
                Size = new Size(363, 36),
                Font = new Font("Times New Roman", 11),
                Text = "OK"
            };
            Controls.Add(OKButton);
            OKButton.Click += new EventHandler(OKButton_Click);

            //Cancel button
            Civ2button CancelButton = new Civ2button
            {
                Location = new Point(374, 398),
                Size = new Size(363, 36),
                Font = new Font("Times New Roman", 11),
                Text = "Cancel"
            };
            Controls.Add(CancelButton);
            CancelButton.Click += new EventHandler(CancelButton_Click);

            //Make an options array
            choiceOptions = new bool[11] { Game.Options.SoundEffects, Game.Options.Music, Game.Options.AlwaysWaitAtEndOfTurn, Game.Options.AutosaveEachTurn, Game.Options.ShowEnemyMoves, Game.Options.NoPauseAfterEnemyMoves, Game.Options.FastPieceSlide, Game.Options.InstantAdvice, Game.Options.TutorialHelp, Game.Options.MoveUnitsWithoutMouse, Game.Options.EnterClosestCityScreen };
            //Write here individual options
            textOptions = new string[11] { "Sound Effects", "Music", "Always wait at end of turn.", "Autosave each turn.", "Show enemy moves.", "No pause after enemy moves.", "Fast piece slide.", "Instant advice.", "Tutorial help.", "Move units w/ mouse (cursor arrows).", "ENTER key closes City Screen." };
            //Make click panels for each options
            for (int i = 0; i < 11; i++)
            {
                DoubleBufferedPanel panel = new DoubleBufferedPanel
                {
                    Location = new Point(10, 32 * i + 4),
                    Size = new Size(100, 27),   //you will set the correct width once you measure text size below
                    BackColor = Color.Transparent
                };
                MainPanel.Controls.Add(panel);
                panel.Click += new EventHandler(ClickPanels_Click);
                ClickPanels.Add(panel);               
            }
        }

        private void GameOptionsForm_Load(object sender, EventArgs e) { }

        private void GameOptionsForm_Paint(object sender, PaintEventArgs e)
        {
            //Text
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.DrawString("Civilization II", new Font("Times New Roman", 20), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString("Civilization II", new Font("Times New Roman", 20), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
        }

        private void MainPanel_Paint(object sender, PaintEventArgs e)
        {
            //Add border lines
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, MainPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 0, 0, 0, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 1, 0, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 0, MainPanel.Height - 1, MainPanel.Width - 1, MainPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, MainPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 1, 1, 1, MainPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), MainPanel.Width - 2, 1, MainPanel.Width - 2, MainPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 1, MainPanel.Height - 2, MainPanel.Width - 2, MainPanel.Height - 2);

            //Show all options
            for (int row = 0; row < 11; row++)
            {
                //Text
                Font fontOptions = new Font("Times New Roman", 18);
                e.Graphics.DrawString(textOptions[row], fontOptions, new SolidBrush(Color.FromArgb(51, 51, 51)), new Point(36, 32 * row + 4));  //Text of option
                stringSize[row] = e.Graphics.MeasureString(textOptions[row], fontOptions);  //measure size of text
                ClickPanels[row].Size = new Size(30 + (int)(stringSize[row].Width), ClickPanels[row].Height);   //set the correct size of click panel

                //Draw checkbox
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

                //Draw check marks
                if (choiceOptions[row])
                {
                    e.Graphics.DrawString("ü", new Font("Wingdings", 18), new SolidBrush(Color.Black), new Point(10 + 1, 32 * row + 3 + 2));
                    e.Graphics.DrawString("ü", new Font("Wingdings", 18), new SolidBrush(Color.FromArgb(192, 192, 192)), new Point(10, 32 * row + 3));
                }
            }
        }

        //if OK is pressed --> update the options and close
        private void OKButton_Click(object sender, EventArgs e)
        {
            Game.Options.SoundEffects = choiceOptions[0];
            Game.Options.Music = choiceOptions[1];
            Game.Options.AlwaysWaitAtEndOfTurn = choiceOptions[2];
            Game.Options.AutosaveEachTurn = choiceOptions[3];
            Game.Options.ShowEnemyMoves = choiceOptions[4];
            Game.Options.NoPauseAfterEnemyMoves = choiceOptions[5];
            Game.Options.FastPieceSlide = choiceOptions[6];
            Game.Options.InstantAdvice = choiceOptions[7];
            Game.Options.TutorialHelp = choiceOptions[8];
            Game.Options.MoveUnitsWithoutMouse = choiceOptions[9];
            Game.Options.EnterClosestCityScreen = choiceOptions[10];
            Close();
        }

        //if cancel is pressed --> just close the form
        private void CancelButton_Click(object sender, EventArgs e) { Close(); }

        //When an option is clicked, update the bool array of options
        private void ClickPanels_Click(object sender, EventArgs e)
        {
            Control clickedControl = (Control)sender;   // Sender gives you which control is clicked.
            int index = ClickPanels.FindIndex(a => a == clickedControl);    //get which control is clicked in a list
            choiceOptions[index] = !choiceOptions[index];   //change the value to opposite
            MainPanel.Refresh();    //refresh the panel after changing the value
        }
    }
}
