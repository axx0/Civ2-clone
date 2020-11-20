using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Configuration;

namespace civ2.Forms
{
    public partial class IntroForm : Civ2form
    {
        private CheckBox MaxScrBox;
        private ComboBox ResolBox;
        private TextBox PathBox, SAVbox, ResultBox;
        private Civ2button StartButton;
        public List<Resolution> Resolutions = new List<Resolution>();
        
        public IntroForm()
        {
            InitializeComponent();            
            Size = new Size(230, 440);
            CenterToScreen();
            Paint += new PaintEventHandler(IntroForm_Paint);

            #region Define possible window sizes
            Resolutions.Add(new Resolution(640, 480, "640x480"));
            Resolutions.Add(new Resolution(800, 600, "800x600"));
            Resolutions.Add(new Resolution(1280, 720, "1280x720"));
            Resolutions.Add(new Resolution(1280, 720, "1280x960"));
            Resolutions.Add(new Resolution(1920, 1080, "1920x1080"));
            Resolutions.Add(new Resolution(-1, -1, "Maximized"));
            #endregion
            #region Define controls
            //Maximized screen checkbox
            MaxScrBox = new CheckBox 
            {
                Location = new Point(12, 100),
                BackColor = Color.Transparent 
            };
            Controls.Add(MaxScrBox);
            MaxScrBox.CheckedChanged += new EventHandler(MaxScrBox_CheckedChanged);

            //Resolution combobox
            ResolBox = new ComboBox 
            {
                Location = new Point(30, 150),
                BackColor = Color.LightGray,
                Font = new Font("Times New Roman", 11),
                DropDownStyle = ComboBoxStyle.DropDownList 
            };
            foreach (Resolution resol in Resolutions)
                if (resol.Name != "Maximized") ResolBox.Items.Add(resol.Name);
            ResolBox.SelectedIndex = 0;
            Controls.Add(ResolBox);

            //Start button
            StartButton = new Civ2button 
            {
                Location = new Point(10, 400),
                Size = new Size(100, 30),
                Text = "Start Game" 
            };
            Controls.Add(StartButton);
            StartButton.Click += new EventHandler(StartButton_Clicked);
           
            //Quit button
            Civ2button QuitButton = new Civ2button 
            {
                Location = new Point(120, 400),
                Size = new Size(100, 30),
                Text = "Quit" 
            };
            Controls.Add(QuitButton);
            QuitButton.Click += new EventHandler(QuitButton_Clicked);

            //Civ2-path textbox
            PathBox = new TextBox 
            {
                Location = new Point(30, 220),
                BackColor = Color.LightGray,
                Font = new Font("Times New Roman", 11),
                Size = new Size(160, 30) 
            };
            Controls.Add(PathBox);
            
            //SAV name textbox
            SAVbox = new TextBox 
            {
                Location = new Point(30, 280),
                Size = new Size(160, 30),
                BackColor = Color.LightGray,
                Font = new Font("Times New Roman", 11) };
            Controls.Add(SAVbox);

            //Result textbox
            ResultBox = new TextBox {
                Location = new Point(10, 320),
                Size = new Size(210, 70),
                Multiline = true,
                BackColor = Color.Black,
                Font = new Font("Times New Roman", 11),
                ForeColor = Color.Red 
            };
            Controls.Add(ResultBox);
            #endregion
        }

        private void IntroForm_Load(object sender, EventArgs e) 
        {
            //Load settings from App.config
            try
            {
                //Read from config file
                Settings.Civ2Path = ConfigurationManager.AppSettings.Get("path");
                Settings.SAVname = ConfigurationManager.AppSettings.Get("SAV file");
                Settings.WindowSize = ConfigurationManager.AppSettings.Get("window size");

                //Update controls in form
                PathBox.Text = Settings.Civ2Path;
                SAVbox.Text = Settings.SAVname;
                if (Settings.WindowSize == "Maximized") 
                {
                    MaxScrBox.Checked = true; 
                    ResolBox.Enabled = false; 
                }
                else 
                { 
                    ResolBox.Enabled = true; 
                    ResolBox.SelectedIndex = Resolutions.FindIndex(a => a.Name == Settings.WindowSize);
                    MaxScrBox.Checked = false; 
                }
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }

            CheckInput();   //Check if there are any problems with input from config file

            PathBox.TextChanged += new EventHandler(PathBox_TextChanged);   //Subscribe to text change events after config file was read, so that it doesn't interfere
            SAVbox.TextChanged += new EventHandler(SAVbox_TextChanged);
        }

        private void IntroForm_Paint(object sender, PaintEventArgs e) 
        {
            e.Graphics.DrawString("civ2", new Font("Times New Roman", 25), new SolidBrush(Color.Black), new Point(10, 10));
            e.Graphics.DrawString("civ2", new Font("Times New Roman", 25), new SolidBrush(Color.DarkRed), new Point(9, 9));
            e.Graphics.DrawString("launcher", new Font("Times New Roman", 15, FontStyle.Italic), new SolidBrush(Color.DarkBlue), new Point(12, 50));
            e.Graphics.DrawIcon(Properties.Resources.civ2, new Rectangle(160, 25, 32, 32));
            e.Graphics.DrawString("Maximized", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(26, 103));
            e.Graphics.DrawString("Chose screen size:", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 130));
            e.Graphics.DrawString("Civ2 path:", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 200));
            e.Graphics.DrawString(".SAV name:", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 260)); 
        }

        //Checkbox fullscreen state changed
        private void MaxScrBox_CheckedChanged(Object sender, EventArgs e) 
        {
            ResolBox.Enabled = !MaxScrBox.Checked;
        }

        //Path textbox text changed
        private void PathBox_TextChanged(Object sender, EventArgs e) 
        {
            CheckInput(); 
        }

        //SAV textbox text changed
        private void SAVbox_TextChanged(Object sender, EventArgs e) 
        {
            CheckInput(); 
        }

        //Start button clicked
        private void StartButton_Clicked(Object sender, EventArgs e) 
        {
            this.Hide();
            int resolChoice = MaxScrBox.Checked ? Resolutions.FindIndex(a => a.Name == "Maximized") : ResolBox.SelectedIndex;
            Settings.WindowSize = Resolutions[resolChoice].Name;
            UpdateConfig();   //update config file with current settings before closing form
            Game.Preloading(Settings.Civ2Path);
            Game.LoadGame(Settings.Civ2Path, Settings.SAVname);
            var form2 = new MainCiv2Window(Resolutions[resolChoice]);
            form2.Closed += (s, args) => this.Close();
            form2.Show(); 
        }

        //Quit button clicked
        private void QuitButton_Clicked(Object sender, EventArgs e) 
        { 
            Close();
        }

        //Check if there are any problems with input from config file (check if directory and SAV file from input exist)
        private void CheckInput()
        {
            string _Civ2Path = PathBox.Text;    //for checking, update Settings if both are ok
            string _SAVname = SAVbox.Text;

            bool dirExists = Directory.Exists(_Civ2Path);
            bool savExists = File.Exists(_Civ2Path + _SAVname + ".SAV");
            ResultBox.Text = "";
            if (!dirExists) 
            {
                ResultBox.AppendText("Directory doesn't exist.");
                PathBox.BackColor = Color.PaleVioletRed;
                SAVbox.BackColor = Color.PaleVioletRed;
                SAVbox.Enabled = false;
                StartButton.Enabled = false; 
            }
            else 
            {
                Settings.Civ2Path = _Civ2Path;
                SAVbox.Enabled = true;
                if (savExists) 
                {
                    StartButton.Enabled = true;
                    PathBox.BackColor = Color.LightGray;
                    SAVbox.BackColor = Color.LightGray;
                    Settings.SAVname = _SAVname;
                }
                else 
                {
                    ResultBox.AppendText("SAV file doesn't exist.");
                    SAVbox.BackColor = Color.PaleVioletRed;
                    StartButton.Enabled = false; 
                } 
            }
            if (StartButton.Enabled) 
            {
                StartButton.Focus();
                StartButton.Select(); 
            }
        }

        //Update config file with current settings
        private void UpdateConfig()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["path"].Value = Settings.Civ2Path;
            config.AppSettings.Settings["SAV file"].Value = Settings.SAVname;
            config.AppSettings.Settings["window size"].Value = Settings.WindowSize;
            config.AppSettings.SectionInformation.ForceSave = true;
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
