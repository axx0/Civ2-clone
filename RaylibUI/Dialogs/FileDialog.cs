using System.Numerics;
using Civ2engine;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI;

public class FileDialog : BaseDialog
{
    private const string ParentDirectory = "(Parent Directory)";
    private readonly Func<string, bool> _isValidSelectionCallback;
    private readonly Func<string, bool> _onSelectionCallback;
    private string _currentDirectory;
    private readonly ListBox listBox;
    private readonly TextBox textBox;
    private readonly Button okButton;

    public FileDialog(Main host, string title, string baseDirectory, Func<string, bool> isValidSelectionCallback,
        Func<string?, bool> onSelectionCallback) : base(host,title)
    {
        _isValidSelectionCallback = isValidSelectionCallback;
        _onSelectionCallback = onSelectionCallback;
        _currentDirectory = baseDirectory;
        listBox = new ListBox(this);
        listBox.ItemSelected += ItemSelected;
        textBox = new TextBox(this, baseDirectory, 600, TestSelection);
        okButton = new Button(this, "Ok", () =>
        {
            var path = Path.Combine(_currentDirectory, textBox.Text);
            if(_isValidSelectionCallback(path))
            {
                onSelectionCallback(path);
            }
        });
        Controls.Add(listBox);
        var menuBar = new ControlGroup(this, flexElement: 0);
        menuBar.AddChild(textBox);
        menuBar.AddChild(okButton);
        menuBar.AddChild(new Button(this, "Cancel", () => onSelectionCallback(null)));
        Controls.Add(menuBar);
        SetButtons(menuBar);
        
        BuildFileList(false);
    }

    private void ItemSelected(object sender, ListBoxSelectionEventArgs args)
    {
        var test = args.Text;
        if (test == ParentDirectory)
        {
            var parent = Path.GetDirectoryName(_currentDirectory);
            if (!string.IsNullOrWhiteSpace(parent))
            {
                _currentDirectory = parent;
                BuildFileList(true);
                return;
            }
        }

        var canPath = Path.Combine(_currentDirectory, test);

        if (Directory.Exists(canPath) && (!_isValidSelectionCallback(test) || test == textBox.Text))
        {
            _currentDirectory = canPath;
            
            BuildFileList(true);
            return;
        }
        textBox.SetText(args.Text);
    }

    private void TestSelection(string file)
    {
        var path = Path.Combine(_currentDirectory, file);
        if (Directory.Exists(file) || Directory.Exists(path))
        {
            _currentDirectory = Path.GetDirectoryName(path) ?? file;
            BuildFileList(true);
        }
        else if (_isValidSelectionCallback(path))
        {
            _onSelectionCallback(path);
        }
    }

    private void BuildFileList(bool refresh)
    {
        var list = new List<string> { ParentDirectory };
        var valid = new List<bool>() { false };
        foreach (var directory in Directory.EnumerateDirectories(_currentDirectory))
        {
            if (directory.StartsWith(".")) continue;
            list.Add(Path.GetFileName(directory));
            valid.Add(_isValidSelectionCallback(directory));
        }

        list.AddRange(Directory.EnumerateFiles(_currentDirectory).Where(file => _isValidSelectionCallback(file)).Select(Path.GetFileName)!);

        listBox.SetElements(list, valid, refresh);
    }
}