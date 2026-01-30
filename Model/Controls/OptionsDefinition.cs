using Model.Images;

namespace Model.Controls;

public class OptionsDefinition
{
    public bool IsCheckbox { get; set; }
    public IList<string>? Texts { get; set; }

    /// <summary>
    /// Texts with replaced strings/numbers.
    /// </summary>
    public IList<string> ReplacedTexts { get; set; } = [];

    public IList<bool>? CheckboxStates { get; set; }

    /// <summary>
    /// Total option columns (visible+invisible).
    /// </summary>
    public int Columns { get; set; } = 1;

    /// <summary>
    /// No of option columns that are visible on panel.
    /// </summary>
    public int MaxVisibleCols { get; set; } = 20;

    /// <summary>
    /// No of option rows that are visible on panel.
    /// </summary>
    public int MaxVisibleRows { get; set; } = 20;

    /// <summary>
    /// Initially selected option id.
    /// </summary>
    public int SelectedId { get; set; } = 0;

    /// <summary>
    /// Images that replace radio buttons.
    /// </summary>
    public IImageSource[]? Icons { get; set; }
}