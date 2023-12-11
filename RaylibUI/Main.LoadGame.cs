
using Civ2engine.IO;
using Civ2engine;
using Civ2engine.Units;
using RaylibUI.RunGame;

namespace RaylibUI
{
    public partial class Main
    {
        private Unit activeUnit;

        
        public void StartGame(Game game)
        {
            game.UpdatePlayerViewData();
            
            _activeScreen = new GameScreen(this, game, Soundman);
        }

    }
}
