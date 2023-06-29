using System.Numerics;
using Civ2engine;
using Raylib_cs;
using RaylibUI.Controls;

namespace RaylibUI;

public class FileDialog : BaseDialog
{
    private readonly Func<string, bool> _isValidSelectionCallback;
    private readonly Func<string, bool> _onSelectionCallback;
    private string _currentDirectory;
    private readonly ListBox listBox;
    private readonly TextBox textBox;
    private readonly Controls.Button okButton;

    public FileDialog(string title, string baseDirectory, Func<string, bool> isValidSelectionCallback,
        Func<string, bool> onSelectionCallback) : base(title)
    {
        _isValidSelectionCallback = isValidSelectionCallback;
        _onSelectionCallback = onSelectionCallback;
        _currentDirectory = baseDirectory;
        listBox = new ListBox(this,columnWidth: 500);
        textBox = new TextBox(this, baseDirectory, 600, TestSelection);
        okButton = new Controls.Button(this, "Ok", () =>
        {
            if(_isValidSelectionCallback(textBox.Text))
            {
                onSelectionCallback(textBox.Text);
            }
        });
        Controls.Add(listBox);
        var menuBar = new ControlGroup(this);
        menuBar.AddChild(textBox);
        menuBar.AddChild(okButton);
        Controls.Add(menuBar);
        BuildFileList();
    }

    private void TestSelection(string dir)
    {
        if (Directory.Exists(dir))
        {
            _currentDirectory = dir;
            BuildFileList();
        }
        else
        {
            if (_isValidSelectionCallback(dir))
            {
                _onSelectionCallback(dir);
            }
        }
    }

    private void BuildFileList()
    {
        var list = new List<string> { "(Parent Directory)" };
        var valid = new List<bool>() { false };
        foreach (var directory in Directory.EnumerateDirectories(_currentDirectory))
        {
            list.Add(Path.GetFileName(directory));
            valid.Add(_isValidSelectionCallback(directory));
        }

        foreach (var file in Directory.EnumerateFiles(_currentDirectory))
        {
            if (_isValidSelectionCallback(file))
            {
                list.Add(file);
            }
        }

        listBox.SetElements(list, valid);
    }
}