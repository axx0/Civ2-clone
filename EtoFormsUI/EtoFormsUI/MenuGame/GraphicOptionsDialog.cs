using Eto.Forms;
using Civ2engine;

namespace EtoFormsUI
{
    public class GraphicOptionsDialog : CheckboxPanel
    {
        private Game Game => Game.Instance;

        public GraphicOptionsDialog(Main parent) : base(parent, 746, 280, "Select Graphic Options", new string[6] { "Throne Room (TODO)", "Diplomacy Screen (TODO)", "Animated Heralds (Requires 16 megabytes RAM) (TODO)", "Civilopedia for Advances (TODO)", "High Council (TODO)", "Wonder Movies (TODO)" }, new string[2] { "OK", "Cancel" })
        {
            var owner = this.Owner;

            // Put starting values into options
            CheckBox[0].Checked = Game.Options.ThroneRoomGraphics;
            CheckBox[1].Checked = Game.Options.DiplomacyScreenGraphics;
            CheckBox[2].Checked = Game.Options.AnimatedHeralds;
            CheckBox[3].Checked = Game.Options.CivilopediaForAdvances;
            CheckBox[4].Checked = Game.Options.HighCouncil;
            CheckBox[5].Checked = Game.Options.WonderMovies;

            // Define abort button (= Cancel) so that is also called with Esc
            AbortButton = Button[1];
            AbortButton.Click += (sender, e) =>
            {
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                Close();
            };

            // Define default button (= OK) so that it is also called with return key
            DefaultButton = Button[0];
            DefaultButton.Click += (sender, e) =>
            {
                Game.Options.ThroneRoomGraphics = CheckBox[0].Checked == true;
                Game.Options.DiplomacyScreenGraphics = CheckBox[1].Checked == true;
                Game.Options.AnimatedHeralds = CheckBox[2].Checked == true;
                Game.Options.CivilopediaForAdvances = CheckBox[3].Checked == true;
                Game.Options.HighCouncil = CheckBox[4].Checked == true;
                Game.Options.WonderMovies = CheckBox[5].Checked == true;
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                Close();
            };
        }
    }
}
