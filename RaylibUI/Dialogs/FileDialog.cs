using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Model;
using Model.Images;
using Model.Interface;
using Raylib_CSharp.Colors;
using Raylib_CSharp.Interact;
using Raylib_CSharp.Windowing;
using RaylibUI.BasicTypes;
using RaylibUI.BasicTypes.Controls;
using RaylibUI.Controls;
using System.IO;
using System.Numerics;

namespace RaylibUI;

public class FileDialog : DynamicSizingDialog
{
    private const string ParentDirectory = "(Parent Directory)";
    private readonly Func<string, bool> _isValidSelectionCallback;
    private readonly Func<string?, bool> _onSelectionCallback;
    private readonly bool _selectionMode;
    private string _currentDirectory;
    private readonly Listbox _listbox;
    private readonly LabelControl _directoryLabel;
    private readonly TextBox _textBox;
    private readonly Button _okButton;
    private readonly TableLayoutPanel _innerPanel;
    private bool _isRoot;
    private readonly IUserInterface? _active;
    private Dictionary<string, GameVersionType?> _fileList;
    private ListboxDefinition _listboxDef;

    public FileDialog(Main host, string title, string baseDirectory, Func<string, bool> isValidSelectionCallback,
        Func<string?, bool> onSelectionCallback, string? initialFileName = null, bool selectionMode = true) : base(host, title, requestedWidth: 950)
    {
        _isValidSelectionCallback = isValidSelectionCallback;
        _onSelectionCallback = onSelectionCallback;
        _selectionMode = selectionMode;
        _active = host.ActiveInterface;

        var innerLayout = new TableLayout();

        _directoryLabel = new LabelControl(this, "", true, font: _active?.Look.StatusPanelLabelFont, 
            colorFront: _active?.Look.StatusPanelLabelColor, colorShadow: _active?.Look.StatusPanelLabelColorShadow, shadowOffset: new(1, 1));
        innerLayout.Add(_directoryLabel, 0, 0);

        SetDirectoryLocation(baseDirectory);
        BuildFileList(false);
        _listboxDef = new ListboxDefinition() 
        { 
            Groups = MakeListboxEntries(),
            Columns = 5,
            VerticalScrollbar = false
        };
        if (_active != null)
        {
            _listboxDef.Looks = _active.GetListboxLooks(ListboxType.Default);
        }
        _listbox = new Listbox(this, _listboxDef);
        _listbox.ItemSelected += ItemSelected;
        innerLayout.Add(_listbox, 1, 0, new Padding(2, 2, 2, 2));

        _innerPanel = new TableLayoutPanel(this)
        {
            Location = new Vector2(LayoutPadding.Left, LayoutPadding.Top),
            Width = 600,
            TableLayout = innerLayout
        };
        Controls.Add(_innerPanel);

        _textBox = new TextBox(this, initialFileName ?? string.Empty, 600, TestSelection);
        _okButton = new Button(this, _active == null ? Labels.Ok : Labels.For(LabelIndex.OK));
        _okButton.Click += OkClicked;
        var menuBar = new ControlGroup(this, flexElement: 0);
        menuBar.AddChild(_textBox);
        menuBar.AddChild(_okButton);
        var cancelButton = new Button(this, _active == null ? Labels.Cancel : Labels.For(LabelIndex.Cancel));
        cancelButton.Click += CancelButtonOnClick;
        menuBar.AddChild(cancelButton);
        Controls.Add(menuBar);
        SetButtons(menuBar);

        _textBox.TextChanged += (sender, args) =>
        {
            _okButton.Enabled = _isValidSelectionCallback(Path.Combine(_currentDirectory, _textBox.Text));
        };

        Focused = _listbox;
    }

    private List<ListboxGroup> MakeListboxEntries()
    {
        List<ListboxGroup> lists = [];
        for (int i = 0; i < _fileList.Count; i++)
        {
            var iconIndex = _fileList.ElementAt(i).Value switch
            {
                null => 0,
                GameVersionType.CiC => 1,
                GameVersionType.Fw => 2,
                GameVersionType.Mge => 3,
                GameVersionType.ToT10 => 4,
                GameVersionType.Json => 5,
                _ => 1
            };

            lists.Add(new ListboxGroup { Elements = 
                [ new ListboxGroupElement { Icon = _active?.Look.DiskIcons[iconIndex] },
                  new ListboxGroupElement { Text = _fileList.ElementAt(i).Key } ]
            });
        }
        return lists;
    }
    
    private void SetDirectoryLocation(string directory)
    {
        _directoryLabel.Text = $" Contents of: {directory}";

        _isRoot = string.IsNullOrWhiteSpace(Path.GetDirectoryName(directory));
        _currentDirectory = directory;
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

    public override void OnKeyPress(KeyboardKey key)
    {
        switch (key)
        {
            case KeyboardKey.Enter:
                _listbox.EnterPressed();
                var path = Path.Combine(_currentDirectory, _textBox.Text);
                if (_isValidSelectionCallback(path))
                {
                    _onSelectionCallback(path);
                }
                return;
            case KeyboardKey.Escape:
                _onSelectionCallback(null);
                return;
        }

        base.OnKeyPress(key);
    }

    private void ItemSelected(object? sender, ListboxSelectionEventArgs args)
    {
        var text = _fileList.ElementAt(args.Index).Key;
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
                    _listboxDef.Groups = MakeListboxEntries();
                    _listbox.Update();
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
                    _listboxDef.Groups = MakeListboxEntries();
                    _listbox.Update(true);
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
        _fileList = _isRoot ? new() : new() { { ParentDirectory, null } };
        var valid = new List<bool>() { false };
        foreach (var directory in Directory.EnumerateDirectories(_currentDirectory))
        {
            if (directory.StartsWith('.')) continue;
            _fileList.Add(Path.GetFileName(directory), null);
            valid.Add(_isValidSelectionCallback(directory));
        }

        var files = Directory.EnumerateFiles(_currentDirectory).Where(file => _isValidSelectionCallback(file)).Select(Path.GetFileName)!;

        // Get civ2 version of files
        for (int i = 0; i < files.Count(); i++)
        {
            _fileList.Add(files.ElementAt(i), GetCiv2Version(Path.Combine(_currentDirectory, files.ElementAt(i))));
        }
    }

    private GameVersionType GetCiv2Version(string filePath)
    {
        var buffer = new byte[11];
        using FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
        int bytesRead = fs.Read(buffer, 0, 11);
        if (bytesRead < 11)
        {
            throw new Exception($"Cannot determine civ2 version for file {filePath}!");
        }
        return buffer[0] == 123 ? GameVersionType.Json : (GameVersionType)buffer[10];  // json always starts with {
    }
}
