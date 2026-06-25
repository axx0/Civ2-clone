using Model.Images;

namespace Model.Controls;

/// <summary>
/// Use this for customizing options in dialogs from game.txt.
/// </summary>
public class OptionsDefinition
{
    public bool? IsCheckbox { get; set; }
    public IList<string>? Texts { get; set; }

    /// <summary>
    /// Texts with replaced strings/numbers.
    /// </summary>
    public IList<string> ReplacedTexts { get; set; }

    public IList<bool>? CheckboxStates { get; set; }

    /// <summary>
    /// Total option columns (visible+invisible).
    /// </summary>
    public int? Columns { get; set; }

    /// <summary>
    /// No of option columns that are visible on panel.
    /// </summary>
    public int? MaxVisibleCols { get; set; }

    /// <summary>
    /// No of option rows that are visible on panel.
    /// </summary>
    public int? MaxVisibleRows { get; set; }

    /// <summary>
    /// Initially selected option id.
    /// </summary>
    public int? SelectedId { get; set; }

    /// <summary>
    /// Images that replace radio buttons.
    /// </summary>
    public IImageSource[]? Icons { get; set; }
}