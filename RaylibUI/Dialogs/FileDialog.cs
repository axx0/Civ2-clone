using System.Numerics;
using Civ2engine;
using Raylib_CSharp;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;

namespace RaylibUI;

public class FileDialog : DynamicSizingDialog
{
    private const string ParentDirectory = "(Parent Directory)";
    private readonly Func<string, bool> _isValidSelectionCallback;
    private readonly Func<string?, bool> _onSelectionCallback;
    private readonly bool _selectionMode;
    private string _currentDirectory;
    private readonly ListBox _listBox;
    private readonly TextBox _textBox;
    private readonly Button _okButton;
    private bool _isRoot;

    public FileDialog(Main host, string title, string baseDirectory, Func<string, bool> isValidSelectionCallback,
        Func<string?, bool> onSelectionCallback, string? initialFileName = null,  bool selectionMode = true) : base(host,title)
    {
        _isValidSelectionCallback = isValidSelectionCallback;
        _onSelectionCallback = onSelectionCallback;
        _selectionMode = selectionMode;
        SetDirectoryLocation(baseDirectory);
        
        _listBox = new ListBox(this);
        _listBox.ItemSelected += ItemSelected;
        _textBox = new TextBox(this, initialFileName ?? string.Empty, 600, TestSelection);
        _okButton = new Button(this, host.ActiveInterface == null ? Labels.Ok : Labels.For(LabelIndex.OK));
        _okButton.Click += OkClicked;
        Controls.Add(_listBox);
        var menuBar = new ControlGroup(this, flexElement: 0);
        menuBar.AddChild(_textBox);
        menuBar.AddChild(_okButton);
        var cancelButton = new Button(this, host.ActiveInterface == null ? Labels.Cancel : Labels.For(LabelIndex.Cancel));
        cancelButton.Click += CancelButtonOnClick;
        menuBar.AddChild(cancelButton);
        Controls.Add(menuBar);
        SetButtons(menuBar);

        _textBox.TextChanged += (sender, args) =>
        {
            _okButton.Enabled = _isValidSelectionCallback( Path.Combine(_currentDirectory, _textBox.Text));
        };
        
        BuildFileList(false);
    }

    private void SetDirectoryLocation(string directory)
    {
        var directoryLabel = new LabelControl(this, $" Contents of: {directory}", true, colorBack: new Color(207,207,207,255));
        _isRoot = string.IsNullOrWhiteSpace(Path.GetDirectoryName(directory));
        _currentDirectory = directory;
        
        if (Controls.Count > 1)
        {
            Controls[1] = directoryLabel;
            Resize(Window.GetScreenWidth(), Window.GetScreenHeight());
        }
        else
        {
            Controls.Add(directoryLabel);
        }
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

    private void ItemSelected(object? sender, ScrollBoxSelectionEventArgs args)
    {
        var text = args.Text;
        if (!args.Soft)
        {
            if (text == ParentDirectory)
            {
                var parent = Path.GetDirectoryName(_currentDirectory);
                if (!string.IsNullOrWhiteSpace(parent))
                {
                    SetDirectoryLocation(parent);
                    BuildFileList(true);
                    text = "";
                }
            }
            else
            {
                var canPath = Path.Combine(_currentDirectory, text);

                if (Directory.Exists(canPath) && (!_isValidSelectionCallback(text) || text == _textBox.Text))
                {
                    SetDirectoryLocation(canPath);

                    BuildFileList(true);
                    text = "";
                }
            }
        }

        if (_selectionMode)
        {
            _textBox.SetText(text);
        }
    }

    private void TestSelection(string file)
    {
        var path = Path.Combine(_currentDirectory, file);
        if (_isValidSelectionCallback(path))
        {
            _onSelectionCallback(path);
        }
        if (Directory.Exists(path))
        {
            SetDirectoryLocation(path);
            BuildFileList(true);
        }
    }

    private void BuildFileList(bool refresh)
    {
        var list = _isRoot ? new List<string>() : new List<string> { ParentDirectory };
        var valid = new List<bool>() { false };
        foreach (var directory in Directory.EnumerateDirectories(_currentDirectory))
        {
            if (directory.StartsWith(".")) continue;
            list.Add(Path.GetFileName(directory));
            valid.Add(_isValidSelectionCallback(directory));
        }

        list.AddRange(Directory.EnumerateFiles(_currentDirectory).Where(file => _isValidSelectionCallback(file)).Select(Path.GetFileName)!);

        _listBox.SetElements(list, refresh, valid);
    }
}