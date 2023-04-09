namespace RaylibUI.Controls;

public interface IForm
{
    bool Hover { get; }
    bool Pressed { get; }
    void Draw();
}
