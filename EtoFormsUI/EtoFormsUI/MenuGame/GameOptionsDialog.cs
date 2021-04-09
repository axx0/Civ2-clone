using Eto.Forms;
using Civ2engine;

namespace EtoFormsUI
{
    public class GameOptionsDialog : CheckboxPanel
    {
        private Game Game => Game.Instance;

        public GameOptionsDialog(Main parent) : base(parent, 746, 440, "Civilization II Multiplayer Gold", new string[11] { "Sound Effects (TODO)", "Music (TODO)", "Always wait at end of turn.", "Autosave each turn. (TODO)", "Show enemy moves. (TODO)", "No pause after enemy moves. (TODO)", "Fast piece slide. (TODO)", "Instant advice. (TODO)", "Tutorial help. (TODO)", "Move units w/ mouse (cursor arrows). (TODO)", "ENTER key closes City Screen. (TODO)" }, new string[2] { "OK", "Cancel" })
        {
            // Put starting values into options
            CheckBox[0].Checked = Game.Options.SoundEffects;
            CheckBox[1].Checked = Game.Options.Music;
            CheckBox[2].Checked = Game.Options.AlwaysWaitAtEndOfTurn;
            CheckBox[3].Checked = Game.Options.AutosaveEachTurn;
            CheckBox[4].Checked = Game.Options.ShowEnemyMoves;
            CheckBox[5].Checked = Game.Options.NoPauseAfterEnemyMoves;
            CheckBox[6].Checked = Game.Options.FastPieceSlide;
            CheckBox[7].Checked = Game.Options.InstantAdvice;
            CheckBox[8].Checked = Game.Options.TutorialHelp;
            CheckBox[9].Checked = Game.Options.MoveUnitsWithoutMouse;
            CheckBox[10].Checked = Game.Options.EnterClosestCityScreen;

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
                Game.Options.SoundEffects = CheckBox[0].Checked == true;
                Game.Options.Music = CheckBox[1].Checked == true;
                Game.Options.AlwaysWaitAtEndOfTurn = CheckBox[2].Checked == true;
                Game.Options.AutosaveEachTurn = CheckBox[3].Checked == true;
                Game.Options.ShowEnemyMoves = CheckBox[4].Checked == true;
                Game.Options.NoPauseAfterEnemyMoves = CheckBox[5].Checked == true;
                Game.Options.FastPieceSlide = CheckBox[6].Checked == true;
                Game.Options.InstantAdvice = CheckBox[7].Checked == true;
                Game.Options.TutorialHelp = CheckBox[8].Checked == true;
                Game.Options.MoveUnitsWithoutMouse = CheckBox[9].Checked == true;
                Game.Options.EnterClosestCityScreen = CheckBox[10].Checked == true;
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                Close();
            };
        }
    }
}
