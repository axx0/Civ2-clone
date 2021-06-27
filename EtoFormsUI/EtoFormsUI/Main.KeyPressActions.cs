using Eto.Forms;
using Civ2engine.Enums;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        private void KeyPressedEvent(object sender, KeyEventArgs e)
        {
            if (CurrentGameMode.Actions.ContainsKey(e.Key))
            {
                CurrentGameMode.Actions[e.Key]();
                return;
            }
        
            switch (e.Key)
            {
                case Keys.Enter:
                    {
                        if (statusPanel.WaitingAtEndOfTurn) statusPanel.End_WaitAtEndOfTurn();
                        break;
                    }
                case Keys.Space:
                    {
                        if (!Map.ViewPieceMode)
                        {
                            Game.IssueUnitOrder(OrderType.SkipTurn);
                        }
                        break;
                    }
                case Keys.S:
                    {
                        if (!Map.ViewPieceMode)
                        {
                            Game.IssueUnitOrder(OrderType.Sleep);
                        }
                        break;
                    }
            }
        }
    }
}
