using Eto.Forms;
using Civ2engine;

namespace EtoFormsUI
{
    public class GameOptionsPanel : CheckboxPanel
    {
        private Game Game => Game.Instance;

        public GameOptionsPanel() : base(746, 440, "Civilization II Multiplayer Gold", new string[11] {"Sound Effects", "Music", "Always wait at end of turn.", "Autosave each turn.", "Show enemy moves.", "No pause after enemy moves.", "Fast piece slide.", "Instant advice.", "Tutorial help.", "Move units w/ mouse (cursor arrows).", "ENTER key closes City Screen." }, new string[2] { "OK", "Cancel" })
        {
            Button[0].MouseUp += OKButton_Click;
            Button[1].MouseUp += CancelButton_Click;

            // Put starting values into options
            CheckboxState = new bool[11] { Game.Options.SoundEffects, Game.Options.Music, Game.Options.AlwaysWaitAtEndOfTurn, Game.Options.AutosaveEachTurn, Game.Options.ShowEnemyMoves, Game.Options.NoPauseAfterEnemyMoves, Game.Options.FastPieceSlide, Game.Options.InstantAdvice, Game.Options.TutorialHelp, Game.Options.MoveUnitsWithoutMouse, Game.Options.EnterClosestCityScreen };
        }

        // If OK is pressed --> update the options and close
        private void OKButton_Click(object sender, MouseEventArgs e)
        {
            Game.Options.SoundEffects = CheckboxState[0];
            Game.Options.Music = CheckboxState[1];
            Game.Options.AlwaysWaitAtEndOfTurn = CheckboxState[2];
            Game.Options.AutosaveEachTurn = CheckboxState[3];
            Game.Options.ShowEnemyMoves = CheckboxState[4];
            Game.Options.NoPauseAfterEnemyMoves = CheckboxState[5];
            Game.Options.FastPieceSlide = CheckboxState[6];
            Game.Options.InstantAdvice = CheckboxState[7];
            Game.Options.TutorialHelp = CheckboxState[8];
            Game.Options.MoveUnitsWithoutMouse = CheckboxState[9];
            Game.Options.EnterClosestCityScreen = CheckboxState[10];
            this.Dispose();
            //RemoveAll();
        }

        // If cancel is pressed --> just close
        private void CancelButton_Click(object sender, MouseEventArgs e)
        {
            this.Visible = false;
            this.Dispose();
        }

    }
}
