using Civ2engine;
using ImGuiNET;
using Model;
using Model.Interface;

namespace RaylibUI;

public class Dialog
{
    private int _selectedRadio = 0;
    private readonly string[] _textBoxValues;
    
    private readonly Action<string, int, IDictionary<string,string>?>[] _buttonHandlers;
    private readonly List<string> _text;
    private readonly string _title;
    private readonly IList<string> _options;
    private readonly List<TextBoxDefinition>? _textBoxes;
    private readonly IList<string> _buttons;

    public Dialog(PopupBox popupBox, Action<string, int, IDictionary<string, string>?>[] buttonHandlers,List<TextBoxDefinition>? textBoxes = null)
    {
        _title = popupBox.Title;
        _buttonHandlers = buttonHandlers;
        var text = popupBox.Text?.ToList() ?? new List<string>();

        if (textBoxes is not null)
        {
            if (popupBox.Options?.Count > 0)
            {
                SetTextBoxText(textBoxes, popupBox.Options);
            }
            else
            {
                SetTextBoxText(textBoxes, text);
                text = text.Take(textBoxes[0].index - 1).ToList();
            }

            _textBoxValues = textBoxes.Select(t => t.InitialValue).ToArray();
            _textBoxes = textBoxes;
        }
        else
        {
            _options = popupBox.Options;
        }

        _text = text;
        _buttons = popupBox.Button;
    }
    private void SetTextBoxText(List<TextBoxDefinition> textBoxes, IList<string> text)
    {
        foreach (var textBox in textBoxes)
            textBox.Text = text[textBox.index];
    }
    public void Draw()
    {
        int selectedButton = -1;
        if (ImGui.Begin(_title))
        {
            if (_options?.Count > 0)
            {
                if (ImGui.BeginTable("Table", 1))
                {
                    for (var i = 0; i < _options.Count; i++)
                    {
                        ImGui.TableNextColumn();
                        ImGui.RadioButton(_options[i], ref _selectedRadio, i);
                    }

                    ImGui.EndTable();
                }
            }

            if (_textBoxes != null)
            {
                for (var index = 0; index < _textBoxes.Count; index++)
                {
                    var textBox = _textBoxes[index];
                    ImGui.InputText(textBox.Text, ref _textBoxValues[index], (uint)textBox.Width);
                }
            }

            ImGui.Text($"selected={_selectedRadio}");

            for (int i = 0; i < _buttons.Count; i++)
            {
                if (i > 0)
                {
                    ImGui.SameLine();
                }

                if (ImGui.Button(_buttons[i]))
                {
                    selectedButton = i;
                }
            }


            ImGui.End();
        }

        if (selectedButton != -1)
        {
            _buttonHandlers[selectedButton >= _buttonHandlers.Length ? 0 : selectedButton](
                _buttons[selectedButton], _selectedRadio, FormatTextBoxReturn());
        }
    }

    private IDictionary<string,string>? FormatTextBoxReturn()
    {
        return _textBoxes?.Select((k, i) => new { k.Name, Value = _textBoxValues[i] })
            .ToDictionary(k => k.Name, v => v.Value);
    }
}