using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    public class ScrollsOutcome : GoodyHutOutcome
    {
        public string Name => "Scrolls";
        public string Description => "You have discovered scrolls of ancient wisdom.";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            var advances = unit.Owner.Advances;
            if (advances == null || advances.Length == 0)
            {
                return new GoodyHutOutcomeResult(Description, false, "Scrolls");
            }

            var advanceIndex = Array.FindIndex(advances, known => !known);
            if (advanceIndex < 0)
            {
                return new GoodyHutOutcomeResult("Your sages find no new secrets in the ancient scrolls.", false, "Scrolls");
            }

            advances[advanceIndex] = true;
            return new GoodyHutOutcomeResult(Description, true, "Scrolls")
            {
                AdvanceIndex = advanceIndex
            };
        }
    }
}
