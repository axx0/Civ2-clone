using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace Model.Core.GoodyHuts
{
    public class GoodyHut
    {
        private readonly List<GoodyHutOutcome> _outcomes;
        private readonly Random _random;

        public GoodyHut() : this(null, null)
        {
        }

        public GoodyHut(IEnumerable<GoodyHutOutcome>? outcomes, Random? random = null)
        {
            _outcomes = outcomes?.ToList() ?? new List<GoodyHutOutcome>
            {
                new GoldOutcome(50),
                new AbandonedVillageOutcome(),
                new ScrollsOutcome(),
                new MercenariesOutcome(),
                new TribeOutcome(),
                new BarbariansOutcome()
            };
            _random = random ?? new Random();
        }

        public GoodyHutOutcomeResult Trigger(Unit unit)
        {
            if (_outcomes.Count == 0)
            {
                return new GoodyHutOutcomeResult("The village is empty.", true, "AbandonedVillage");
            }

            // Base chance of each outcome is equal. Certain game circumstances modify this ratio.
            // https://apolyton.net/forum/civilization-series/civilization-i-and-civilization-ii/82184-a-study-of-hut-outcomes
            var outcome = _outcomes[_random.Next(0, _outcomes.Count)];
            return outcome.ApplyOutcome(unit);
        }
    }
}
