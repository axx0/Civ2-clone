using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    public class AbandonedVillageOutcome : GoodyHutOutcome
    {
        public string Name => "Abandoned Village";
        public string Description => "Weeds grow in empty ruins.  This village has long\r\nbeen abandoned.";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            return new GoodyHutOutcomeResult(Description, true, "AbandonedVillage");
        }
    }
}
