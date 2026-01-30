using Civ2engine.Scripting;
using Model;
using Model.Controls;
using Model.Core;
using Raylib_CSharp.Interact;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using RaylibUI.RunGame;
using System.Numerics;

namespace RaylibUI;

public class LuaConsole : DynamicSizingDialog
{
    private readonly GameScreen _host;
    private readonly IScriptEngine _script;
    private readonly Listbox _listbox;
    private readonly TextBox _commandBox;
    private readonly ListboxDefinition _def;

    private void ExecuteImmediate(string command)
    {
        if(string.IsNullOrWhiteSpace(command)) return;
        
        _script.Execute(command);
        _def.Update(_script.Log.Split(Environment.NewLine));
        _listbox.Update();
        _listbox.OnResize();
        _listbox.ScrollToEnd();
        _commandBox.SetText(string.Empty);
    }

    public LuaConsole(GameScreen host) : base(host.Main, "Lua Console", requestedWidth: host.MainWindow.ActiveInterface.DefaultDialogWidth)
    {
        _host = host;
        _script = host.Game.Script;

        var innerLayout = new TableLayout();
        _def = new ListboxDefinition()
        {
            Type = ListboxType.Default,
            Selectable = false
        };
        _def.Update(_script.Log.Split(Environment.NewLine));
        _listbox = new Listbox(this, _def);
        _listbox.ScrollToEnd();
        innerLayout.Add(_listbox, 0, 0, new Padding(2, 2, 2, 2));

        var _innerPanel = new TableLayoutPanel(this)
        {
            Location = new Vector2(LayoutPadding.Left, LayoutPadding.Top),
            TableLayout = innerLayout
        };
        Controls.Add(_innerPanel);

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