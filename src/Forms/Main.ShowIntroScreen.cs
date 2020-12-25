using System.Windows.Forms;
using System.Drawing;
using civ2.Bitmaps;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class Main : Form
    {
        private ChoiceMenuPanel _choiceMenu;
        private PictureBox _sinaiPanel;

        // Load intro screen
        public void ShowIntroScreen()
        {
            MainMenuStrip.Enabled = false;

            // Sinai panel
            _sinaiPanel = new PictureBox
            {
                Image = Images.SinaiPic,
                BackgroundImage = Images.PanelOuterWallpaper,
                Width = Images.SinaiPic.Width + 2 * 11,
                Height = Images.SinaiPic.Height + 2 * 11,
                Location = new Point((int)(ClientSize.Width * 0.08333), (int)(ClientSize.Height * 0.0933)),
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            Controls.Add(_sinaiPanel);
            _sinaiPanel.Paint += SinaiBorder_Paint;
            _sinaiPanel.Show();
            _sinaiPanel.BringToFront();

            // Disable any other panels if they exist
            //if (MapForm != null) MapForm.Close();
            //if (statusForm != null) statusForm.Close();
            //if (WorldMapForm != null) WorldMapForm.Close();

            // Choice menu panel
            _choiceMenu = new ChoiceMenuPanel(this)
            {
                Location = new Point((int)(ClientSize.Width * 0.745), (int)(ClientSize.Height * 0.570))
            };
            Controls.Add(_choiceMenu);
        }

        // Make actions based on choice menu results
        public void ChoiceMenuResult(IntroScreenChoiceType choice, string directoryPath, string SAVname)
        {
            switch (choice)
            {
                case IntroScreenChoiceType.StartNewGame:
                    {
                        break;
                    }
                case IntroScreenChoiceType.StartOnPremadeWorld:
                    {
                        break;
                    }
                case IntroScreenChoiceType.CustomizeWorld:
                    {
                        break;
                    }
                case IntroScreenChoiceType.BeginScenario:
                    {
                        break;
                    }
                case IntroScreenChoiceType.LoadGame:
                    {
                        Game.LoadGame(directoryPath, SAVname);
                        LoadPanelsAfterGameStart();
                        break;
                    }
                case IntroScreenChoiceType.MultiplayerGame:
                    {
                        break;
                    }
                case IntroScreenChoiceType.ViewHallOfFame:
                    {
                        break;
                    }
                case IntroScreenChoiceType.ViewCredits:
                    {
                        break;
                    }
            }
        }

        // Draw border around Sinai image
        private void SinaiBorder_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, _sinaiPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, _sinaiPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), _sinaiPanel.Width - 1, 0, _sinaiPanel.Width - 1, _sinaiPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, _sinaiPanel.Height - 1, _sinaiPanel.Width - 1, _sinaiPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, _sinaiPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, _sinaiPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), _sinaiPanel.Width - 2, 1, _sinaiPanel.Width - 2, _sinaiPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, _sinaiPanel.Height - 2, _sinaiPanel.Width - 2, _sinaiPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, _sinaiPanel.Width - 4, 2);   //3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, _sinaiPanel.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), _sinaiPanel.Width - 3, 2, _sinaiPanel.Width - 3, _sinaiPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, _sinaiPanel.Height - 3, _sinaiPanel.Width - 3, _sinaiPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, _sinaiPanel.Width - 5, 3);   //4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, _sinaiPanel.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), _sinaiPanel.Width - 4, 3, _sinaiPanel.Width - 4, _sinaiPanel.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, _sinaiPanel.Height - 4, _sinaiPanel.Width - 4, _sinaiPanel.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, _sinaiPanel.Width - 6, 4);   //5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, _sinaiPanel.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), _sinaiPanel.Width - 5, 4, _sinaiPanel.Width - 5, _sinaiPanel.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, _sinaiPanel.Height - 5, _sinaiPanel.Width - 5, _sinaiPanel.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9, _sinaiPanel.Width - 11, 9);   //1st layer border of sinai image
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9, 9, _sinaiPanel.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), _sinaiPanel.Width - 10, 9, _sinaiPanel.Width - 10, _sinaiPanel.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, _sinaiPanel.Height - 10, _sinaiPanel.Width - 10, _sinaiPanel.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10, _sinaiPanel.Width - 12, 10);   //2nd layer border of sinai image
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10, 10, _sinaiPanel.Height - 12);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), _sinaiPanel.Width - 11, 10, _sinaiPanel.Width - 11, _sinaiPanel.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, _sinaiPanel.Height - 11, _sinaiPanel.Width - 11, _sinaiPanel.Height - 11);
            e.Dispose();
        }
    }
}
