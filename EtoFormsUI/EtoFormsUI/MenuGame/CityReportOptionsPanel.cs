using Civ2engine;

namespace EtoFormsUI
{
    public class CityReportOptionsPanel : CheckboxPanel
    {
        private Game Game => Game.Instance;

        public CityReportOptionsPanel() : base(746, 440, "Select City Report Options", new string[11] {"Warn when city growth halted (Aqueduct/Sewer System).", "Show city improvements built.", "Show non-combat units built.", "Show invalid build instructions.", "Announce cities in disorder.", "Announce order restored in city.", "Announce \"We Love The King Day\".", "Warn when food dangerously low.", "Warn when new pollution occurs.", "Warn when changing production will cost shields.", "\"Zoom-to-City\" NOT default action." }, new string[2] { "OK", "Cancel" })
        {
            // Put starting values into options
            CheckBox[0].Checked = Game.Options.WarnWhenCityGrowthHalted;
            CheckBox[1].Checked = Game.Options.ShowCityImprovementsBuilt;
            CheckBox[2].Checked = Game.Options.ShowNonCombatUnitsBuilt;
            CheckBox[3].Checked = Game.Options.ShowInvalidBuildInstructions;
            CheckBox[4].Checked = Game.Options.AnnounceCitiesInDisorder;
            CheckBox[5].Checked = Game.Options.AnnounceOrderRestored;
            CheckBox[6].Checked = Game.Options.AnnounceWeLoveKingDay;
            CheckBox[7].Checked = Game.Options.WarnWhenFoodDangerouslyLow;
            CheckBox[8].Checked = Game.Options.WarnWhenPollutionOccurs;
            CheckBox[9].Checked = Game.Options.WarnChangProductWillCostShields;
            CheckBox[10].Checked = Game.Options.ZoomToCityNotDefaultAction;

            // Define abort button (= Cancel) so that is also called with Esc
            AbortButton = Button[1];
            AbortButton.Click += (sender, e) => Close();

            // Define default button (= OK) so that it is also called with return key
            DefaultButton = Button[0];
            DefaultButton.Click += (sender, e) =>
            {
                Game.Options.WarnWhenCityGrowthHalted = CheckBox[0].Checked == true;
                Game.Options.ShowCityImprovementsBuilt = CheckBox[1].Checked == true;
                Game.Options.ShowNonCombatUnitsBuilt = CheckBox[2].Checked == true;
                Game.Options.ShowInvalidBuildInstructions = CheckBox[3].Checked == true;
                Game.Options.AnnounceCitiesInDisorder = CheckBox[4].Checked == true;
                Game.Options.AnnounceOrderRestored = CheckBox[5].Checked == true;
                Game.Options.AnnounceWeLoveKingDay = CheckBox[6].Checked == true;
                Game.Options.WarnWhenFoodDangerouslyLow = CheckBox[7].Checked == true;
                Game.Options.WarnWhenPollutionOccurs = CheckBox[8].Checked == true;
                Game.Options.WarnChangProductWillCostShields = CheckBox[9].Checked == true;
                Game.Options.ZoomToCityNotDefaultAction = CheckBox[10].Checked == true;
                Close();
            };
        }
    }
}
