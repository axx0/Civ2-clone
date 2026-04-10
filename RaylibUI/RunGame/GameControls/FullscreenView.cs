using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Rendering;
using RaylibUI.RunGame.GameControls.Menu;

namespace RaylibUI.RunGame.GameControls;

public abstract class FullscreenView : BaseDialog
{
    private readonly GameScreen _gameScreen;

    public FullscreenView(GameScreen gameScreen) : base(gameScreen.Main)
    {
        _gameScreen = gameScreen;

        foreach (var ctrl in gameScreen.Controls)
        {
            ctrl.Visible = false;
            if (ctrl is GameMenu)
            {
                GameMenu? menu = ctrl as GameMenu;
                menu.Dropdown.Hide();
            }
        }

        Location = new(0, 0);
    }

    public override int Width => _gameScreen.Width;
    public override int Height => _gameScreen.Height;

    public override void Resize(int width, int height) { }

    public void Close()
    {
        _gameScreen.CloseDialog(this);

        foreach (var ctrl in _gameScreen.Controls)
        {
            ctrl.Visible = true;
        }
    }

    public override void Draw(bool pulse)
    {
        //Graphics.DrawRectangle(0, 0, Width, Height, Color.Black);

        base.Draw(pulse);
    }
}
