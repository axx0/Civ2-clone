using Civ2.Dialogs;

namespace RaylibUI;

public interface IScreen
{
    void Draw(bool pulse);

    void InterfaceChanged(Sound soundManager);
}