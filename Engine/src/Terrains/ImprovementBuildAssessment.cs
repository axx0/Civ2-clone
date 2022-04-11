namespace Civ2engine.Terrains
{
    public class ImprovementBuildAssessment
    {
        public bool CanBuild { get; }
        public string CommandTitle { get; }
        public string ErrorPopup { get; }

        public ImprovementBuildAssessment(bool canBuild, string commandTitle = "", string errorPopup = "CANTIMPROVE")
        {
            CanBuild = canBuild;
            CommandTitle = commandTitle;
            ErrorPopup = string.IsNullOrWhiteSpace(errorPopup) ? "CANTIMPROVE" : errorPopup;
        }
    }
}