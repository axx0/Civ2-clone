using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    internal class BarbariansOutcome : GoodyHutOutcome
    {
        public string Name => "Barbarians";
        public string Description => "You have unleashed a horde of barbarians!";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
