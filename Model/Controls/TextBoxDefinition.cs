namespace Model.Controls;

public class TextBoxDefinition
{
    /// <summary>
    /// The row of the popup to show on
    /// </summary>
    public int Index { get; set; }
        
    /// <summary>
    /// Name to identify the value
    /// </summary>
    public string Name { get; set; }
        
    public string InitialValue { get; set; }

    /// <summary>
    /// Text to the left of textbox
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Minimum numeric value
    /// </summary>
    public int? MinValue { get; set; }

    /// <summary>
    /// Width of textbox (note: approximate it as there is no logic to how this is set in the original)
    /// </summary>
    public int Width { get; set; } = 345;

    /// <summary>
    /// Max number of characters you can enter
    /// </summary>
    public int CharLimit { get; set; } = 15;
}
