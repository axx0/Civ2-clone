using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    internal class ScrollsOutcome : GoodyHutOutcome
    {
        public string Name => "Scrolls";
        public string Description => "You have discovered scrolls of ancient wisdom.";
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            throw new NotImplementedException();
        }
    }
}
