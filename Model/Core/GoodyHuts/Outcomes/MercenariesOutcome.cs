using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    internal class MercenariesOutcome : GoodyHutOutcome
    {
        public string Description => "You have discovered a friendly tribe of skilled mercenaries.";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
