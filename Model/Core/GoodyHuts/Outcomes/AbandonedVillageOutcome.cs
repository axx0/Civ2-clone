using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    internal class AbandonedVillageOutcome : GoodyHutOutcome
    {
        public string Name => "Abandoned Village";
        public string Description => "Weeds grow in empty ruins.  This village has long\r\nbeen abandoned.";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
