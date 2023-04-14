namespace RaylibUI.Forms;

public abstract class Control : IControl
{
    public bool Enabled { get; set; } = true;
    public int KeyPressed { get; set; }

    public Control() { }
}
