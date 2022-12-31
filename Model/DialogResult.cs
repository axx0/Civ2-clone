namespace Model;

public record DialogResult(string SelectedButton, int SelectedIndex, IList<bool> CheckboxReturnStates);