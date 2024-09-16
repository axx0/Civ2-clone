namespace Model.Dialog;

public record DialogResult(string SelectedButton, int SelectedIndex, IList<bool>? CheckboxReturnStates = null, IDictionary<string, string>? TextValues = null)
{

}