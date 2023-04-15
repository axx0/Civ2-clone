namespace RaylibUI.Forms;

public interface IControl
{
    bool Enabled { get; set; }
    int KeyPressed { get; set; }
}
