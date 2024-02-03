using System.Numerics;
using Civ2engine;
using Raylib_cs;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI;

public class FileDialog : DynamicSizingDialog
{
    private const string ParentDirectory = "(Parent Directory)";
    private readonly Func<string, bool> _isValidSelectionCallback;
    private readonly Func<string?, bool> _onSelectionCallback;
    private string _currentDirectory;
    private readonly ListBox _listBox;
    private readonly TextBox _textBox;
    private readonly Button _okButton;

    public FileDialog(Main host, string title, string baseDirectory, Func<string, bool> isValidSelectionCallback,
        Func<string?, bool> onSelectionCallback) : base(host,title)
    {
        _isValidSelectionCallback = isValidSelectionCallback;
        _onSelectionCallback = onSelectionCallback;
        _currentDirectory = baseDirectory;
        _listBox = new ListBox(this);
        _listBox.ItemSelected += ItemSelected;
        _textBox = new TextBox(this, baseDirectory, 600, TestSelection);
        _okButton = new Button(this, "Ok");
        _okButton.Click += OkClicked;
        Controls.Add(_listBox);
        var menuBar = new ControlGroup(this, flexElement: 0);
        menuBar.AddChild(_textBox);
        menuBar.AddChild(_okButton);
        var cancelButton = new Button(this, "Cancel");
        cancelButton.Click += CancelButtonOnClick;
        menuBar.AddChild(cancelButton);
        Controls.Add(menuBar);
        SetButtons(menuBar);
        
        BuildFileList(false);
    }

    private void CancelButtonOnClick(object? sender, MouseEventArgs e)
    {
        _onSelectionCallback(null);
    }

    private void OkClicked(object? sender, MouseEventArgs args)
    {
        var path = Path.Combine(_currentDirectory, _textBox.Text);
        if (_isValidSelectionCallback(path))
        {
            _onSelectionCallback(path);
        }
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

        if (Directory.Exists(canPath) && (!_isValidSelectionCallback(test) || test == _textBox.Text))
        {
            _currentDirectory = canPath;
            
            BuildFileList(true);
            return;
        }
        _textBox.SetText(args.Text);
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

        _listBox.SetElements(list, valid, refresh);
    }
}