namespace RaylibUI.Controls;

public abstract class Control : IControl
{
    protected List<IControl> Controls = new();

    public bool Enabled { get; set; } = true;

    public Control() { }
}
