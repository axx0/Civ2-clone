using Civ2engine.Scripting;
using Model;
using Model.Core;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.RunGame;

namespace RaylibUI;

public class LuaConsole : DynamicSizingDialog
{
    private readonly GameScreen _host;
    private readonly IScriptEngine _script;
    private readonly ListBox _listBox;
    private readonly TextBox _commandBox;

    private void ExecuteImmediate(string command)
    {
        if(string.IsNullOrWhiteSpace(command)) return;
        
        _script.Execute(command);
        _listBox.SetElements( _script.Log.Split('\n'), true, scrollToEnd: true);
        _commandBox.SetText(string.Empty);
    }

    public LuaConsole(GameScreen host) : base(host.Main, "Lua Console")
    {
        _host = host;
        _script = host.Game.Script;

        _listBox = new ListBox(this, true, 1,_script.Log.Split("\n"));
        Controls.Add(_listBox);

        var defaultButton = new Button(this, "Run Script");
        var abortButton = new Button(this, "Close");
        _commandBox = new TextBox(this, "", 250, ExecuteImmediate);

        var commands = new ControlGroup(this, flexElement: 0);
        commands.AddChild(_commandBox);
        commands.AddChild(defaultButton);
        commands.AddChild(abortButton);
        Controls.Add(commands);

        defaultButton.Click += (_, _) => ExecuteImmediate(_commandBox.Text);
        abortButton.Click += (_, _) => host.CloseDialog(this);
        this.Focused = _commandBox;
    }

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Escape:
                _host.CloseDialog(this);
                break;
            case KeyboardKey.KpEnter:
            case KeyboardKey.Enter:
                ExecuteImmediate(_commandBox.Text);
                break;
            default:
                base.OnKeyPress(key);
                break;
        }
    }
}