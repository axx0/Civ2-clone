using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace RTciv2.Forms
{
    public partial class IntroForm : Civ2form
    {
        private CheckBox FullscrBox;
        private ComboBox ResolBox;
        private TextBox PathBox, SAVbox, ResultBox;
        public List<Resolution> Resolutions = new List<Resolution>();
        private string Civ2Path, SAVname;
        private Civ2button StartButton;

        public IntroForm()
        {
            InitializeComponent();            
            Size = new Size(230, 440);
            CenterToScreen();
            Paint += new PaintEventHandler(IntroForm_Paint);

            #region Define possible game resolutions
            Resolutions.Add(new Resolution(640, 480, "640x480"));
            Resolutions.Add(new Resolution(800, 600, "800x600"));
            Resolutions.Add(new Resolution(1280, 720, "1280x720"));
            Resolutions.Add(new Resolution(1280, 720, "1280x960"));
            Resolutions.Add(new Resolution(1920, 1080, "1920x1080"));
            Resolutions.Add(new Resolution(-1, -1, "Fullscreen"));
            #endregion
            #region Define controls
            //Full screen checkbox
            FullscrBox = new CheckBox {
                Location = new Point(12, 100),
                BackColor = Color.Transparent };
            Controls.Add(FullscrBox);
            FullscrBox.CheckedChanged += new EventHandler(FullscrBox_CheckedChanged);

            //Resolution combobox
            ResolBox = new ComboBox {
                Location = new Point(30, 150),
                BackColor = Color.LightGray,
                Font = new Font("Times New Roman", 11),
                DropDownStyle = ComboBoxStyle.DropDownList };
            foreach (Resolution resol in Resolutions)
                if (resol.Name != "Fullscreen") ResolBox.Items.Add(resol.Name);
            ResolBox.SelectedIndex = 0;
            Controls.Add(ResolBox);

            //Start button
            StartButton = new Civ2button {
                Location = new Point(10, 400),
                Size = new Size(100, 30),
                Text = "Start Game" };
            Controls.Add(StartButton);
            StartButton.Click += new EventHandler(StartButton_Clicked);
           
            //Quit button
            Civ2button QuitButton = new Civ2button {
                Location = new Point(120, 400),
                Size = new Size(100, 30),
                Text = "Quit" };
            Controls.Add(QuitButton);
            QuitButton.Click += new EventHandler(QuitButton_Clicked);

            //Civ2-path textbox
            PathBox = new TextBox {
                Location = new Point(30, 220),
                BackColor = Color.LightGray,
                Font = new Font("Times New Roman", 11),
                Size = new Size(160, 30) };
            Controls.Add(PathBox);
            
            //SAV name textbox
            SAVbox = new TextBox {
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
                ForeColor = Color.Red };
            Controls.Add(ResultBox);
            #endregion
        }

        private void IntroForm_Load(object sender, EventArgs e) 
        {
            //Load settings from config file
            string line;
            int count = 0;
            StreamReader file = new StreamReader(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\Config.txt");
            while ((line = file.ReadLine()) != "#END")
            {
                if (line != "" && line[0].Equals('#'))
                {
                    switch (count)
                    {
                        case 0:     //1st # is civ2 path
                            {
                                Civ2Path = file.ReadLine();
                                PathBox.Text = Civ2Path;
                                break;                            
                            }
                        case 1:     //2nd # is SAV name
                            {
                                SAVname = file.ReadLine();
                                SAVbox.Text = SAVname;
                                break;
                            }
                        case 2:     //3rd # is resolution
                            {
                                line = file.ReadLine();
                                if (line == "-1") { FullscrBox.Checked = true; ResolBox.Enabled = false; }
                                else { ResolBox.Enabled = true;  ResolBox.SelectedIndex = Resolutions.FindIndex(a => a.Name == line); FullscrBox.Checked = false; }
                                break;
                            }
                        default: break;
                    }
                    count++;
                }
            }
            file.Close();
            CheckPaths();   //Check if there are any problems with input from config file

            PathBox.TextChanged += new EventHandler(PathBox_TextChanged);   //Subscribe to text change events after config file was read, so that it doesn't interfere
            SAVbox.TextChanged += new EventHandler(SAVbox_TextChanged);
        }

        private void IntroForm_Paint(object sender, PaintEventArgs e) {
            e.Graphics.DrawString("RTciv2", new Font("Times New Roman", 25), new SolidBrush(Color.Black), new Point(10, 10));
            e.Graphics.DrawString("RTciv2", new Font("Times New Roman", 25), new SolidBrush(Color.DarkRed), new Point(9, 9));
            e.Graphics.DrawString("launcher", new Font("Times New Roman", 15, FontStyle.Italic), new SolidBrush(Color.DarkBlue), new Point(12, 50));
            e.Graphics.DrawIcon(Properties.Resources.civ2, new Rectangle(160, 25, 32, 32));
            e.Graphics.DrawString("Fullscreen", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 100));
            e.Graphics.DrawString("Chose resolution:", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 130));
            e.Graphics.DrawString("Civ2 path:", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 200));
            e.Graphics.DrawString(".SAV name:", new Font("Times New Roman", 11), new SolidBrush(Color.Black), new Point(28, 260)); }

        //Checkbox fullscreen state changed
        private void FullscrBox_CheckedChanged(Object sender, EventArgs e) { 
            if (FullscrBox.Checked) ResolBox.Enabled = false;
            else ResolBox.Enabled = true; }

        //Path textbox text changed
        private void PathBox_TextChanged(Object sender, EventArgs e) { CheckPaths(); }

        //SAV textbox text changed
        private void SAVbox_TextChanged(Object sender, EventArgs e) { CheckPaths(); }

        //Start button clicked
        private void StartButton_Clicked(Object sender, EventArgs e) {
            this.Hide();
            int resolChoice;
            if (FullscrBox.Checked) resolChoice = Resolutions.FindIndex(a => a.Name == "Fullscreen");
            else resolChoice = ResolBox.SelectedIndex;
            WriteConfig();   //write a config file with current settings before closing form
            Game.LoadGame(Civ2Path, SAVname);
            var form2 = new MainCiv2Window(Resolutions[resolChoice], Civ2Path, SAVname);
            form2.Closed += (s, args) => this.Close();
            form2.Show(); }

        //Quit button clicked
        private void QuitButton_Clicked(Object sender, EventArgs e) { Close(); }

        //Check if directory and SAV file exist
        private void CheckPaths()
        {
            Civ2Path = PathBox.Text;
            SAVname = SAVbox.Text;

            bool resultDir = Directory.Exists(Civ2Path);
            bool resultSav = File.Exists(Civ2Path + SAVname + ".SAV");
            ResultBox.Text = "";
            if (!resultDir) 
            {
                ResultBox.AppendText("Directory doesn't exist.");
                PathBox.BackColor = Color.PaleVioletRed;
                SAVbox.BackColor = Color.PaleVioletRed;
                SAVbox.Enabled = false;
                StartButton.Enabled = false; 
            }
            else 
            {
                if (resultSav) 
                {
                    StartButton.Enabled = true;
                    SAVbox.Enabled = true;
                    PathBox.BackColor = Color.LightGray;
                    SAVbox.BackColor = Color.LightGray;
                }
                else 
                {
                    ResultBox.AppendText("SAV file doesn't exist.");
                    SAVbox.BackColor = Color.PaleVioletRed;
                    StartButton.Enabled = false; 
                } 
            }
            if (StartButton.Enabled) {
                StartButton.Focus();
                StartButton.Select(); }
        }

        //Write a config file with current settings
        private void WriteConfig()
        {
            string resol;
            if (FullscrBox.Checked) resol = "-1";
            else resol = Resolutions[ResolBox.SelectedIndex].Name;

            StreamWriter file = new StreamWriter(Directory.GetParent(Environment.CurrentDirectory).Parent.FullName + @"\Config.txt");
            file.WriteLine("#Civ2 PATH");
            file.WriteLine(Civ2Path);
            file.WriteLine("");
            file.WriteLine("#SAV file");
            file.WriteLine(SAVname);
            file.WriteLine("");
            file.WriteLine("#Resolution (-1 = fullscreen)");
            file.WriteLine(resol);
            file.WriteLine("");
            file.WriteLine("#END");
            file.Close();
        }
    }
}
