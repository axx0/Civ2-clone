namespace Civ2engine.Terrains
{
    public class UnitActionAssessment
    {
        public bool Enabled { get; }
        public string CommandTitle { get; }
        public string ErrorPopup { get; }

        public UnitActionAssessment(bool enabled, string commandTitle = "", string errorPopup = "CANTIMPROVE")
        {
            Enabled = enabled;
            CommandTitle = commandTitle;
            ErrorPopup = string.IsNullOrWhiteSpace(errorPopup) ? "CANTIMPROVE" : errorPopup;
        }
    }
}