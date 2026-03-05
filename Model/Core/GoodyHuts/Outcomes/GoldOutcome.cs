using Model.Core.Units;

namespace Model.Core.GoodyHuts.Outcomes
{
    public class GoldOutcome : GoodyHutOutcome
    {
        private string _description => $"You have discovered valuable metal deposits worth {_amount} gold.";
        private readonly int _amount;
        public GoldOutcome(int amount)
        {
            _amount = amount;
        }
        public override GoodyHutOutcomeResult ApplyOutcome(Unit unit)
        {
            unit.Owner.Money += _amount;
            return new GoodyHutOutcomeResult(_description, true, "Gold");
        }
    }
}
