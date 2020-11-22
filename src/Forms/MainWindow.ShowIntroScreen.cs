using System.Windows.Forms;
using System.Drawing;
using civ2.Bitmaps;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class MainWindow : Form
    {
        ChoiceMenuPanel ChoiceMenu;
        PictureBox SinaiPanel;

        // Load intro screen
        public void ShowIntroScreen()
        {
            MainMenuStrip.Enabled = false;

            // Sinai panel
            SinaiPanel = new PictureBox
            {
                Image = Images.SinaiPic,
                BackgroundImage = Images.PanelOuterWallpaper,
                Width = Images.SinaiPic.Width + 2 * 11,
                Height = Images.SinaiPic.Height + 2 * 11,
                Location = new Point((int)(ClientSize.Width * 0.08333), (int)(ClientSize.Height * 0.0933)),
                SizeMode = PictureBoxSizeMode.CenterImage
            };
            Controls.Add(SinaiPanel);
            SinaiPanel.Paint += new PaintEventHandler(SinaiBorder_Paint);
            SinaiPanel.Show();
            SinaiPanel.BringToFront();

            // Disable any other panels if they exist
            //if (MapForm != null) MapForm.Close();
            //if (statusForm != null) statusForm.Close();
            //if (WorldMapForm != null) WorldMapForm.Close();

            // Choice menu panel
            ChoiceMenu = new ChoiceMenuPanel(this);
            ChoiceMenu.Location = new Point((int)(ClientSize.Width * 0.745), (int)(ClientSize.Height * 0.570));
            Controls.Add(ChoiceMenu);
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
                        //ArrangeWindowControlsAfterGameStart();
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
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, SinaiPanel.Width - 2, 0);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(227, 227, 227)), 0, 0, 0, SinaiPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), SinaiPanel.Width - 1, 0, SinaiPanel.Width - 1, SinaiPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(105, 105, 105)), 0, SinaiPanel.Height - 1, SinaiPanel.Width - 1, SinaiPanel.Height - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, SinaiPanel.Width - 3, 1);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, SinaiPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), SinaiPanel.Width - 2, 1, SinaiPanel.Width - 2, SinaiPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(160, 160, 160)), 1, SinaiPanel.Height - 2, SinaiPanel.Width - 2, SinaiPanel.Height - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, SinaiPanel.Width - 4, 2);   //3rd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, 2, 2, SinaiPanel.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), SinaiPanel.Width - 3, 2, SinaiPanel.Width - 3, SinaiPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(240, 240, 240)), 2, SinaiPanel.Height - 3, SinaiPanel.Width - 3, SinaiPanel.Height - 3);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, SinaiPanel.Width - 5, 3);   //4th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 3, 3, 3, SinaiPanel.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), SinaiPanel.Width - 4, 3, SinaiPanel.Width - 4, SinaiPanel.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 3, SinaiPanel.Height - 4, SinaiPanel.Width - 4, SinaiPanel.Height - 4);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, SinaiPanel.Width - 6, 4);   //5th layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 4, 4, 4, SinaiPanel.Height - 6);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), SinaiPanel.Width - 5, 4, SinaiPanel.Width - 5, SinaiPanel.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 4, SinaiPanel.Height - 5, SinaiPanel.Width - 5, SinaiPanel.Height - 5);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9, SinaiPanel.Width - 11, 9);   //1st layer border of sinai image
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 9, 9, SinaiPanel.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), SinaiPanel.Width - 10, 9, SinaiPanel.Width - 10, SinaiPanel.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, SinaiPanel.Height - 10, SinaiPanel.Width - 10, SinaiPanel.Height - 10);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10, SinaiPanel.Width - 12, 10);   //2nd layer border of sinai image
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 10, 10, SinaiPanel.Height - 12);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), SinaiPanel.Width - 11, 10, SinaiPanel.Width - 11, SinaiPanel.Height - 11);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, SinaiPanel.Height - 11, SinaiPanel.Width - 11, SinaiPanel.Height - 11);
            e.Dispose();
        }
    }
}
