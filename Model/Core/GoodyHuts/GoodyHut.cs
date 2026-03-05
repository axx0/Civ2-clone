using Model.Core.GoodyHuts.Outcomes;
using Model.Core.Units;

namespace Model.Core.GoodyHuts
{
    public class GoodyHut
    {
        private List<GoodyHutOutcome> _outcomes = new();

        public GoodyHut() 
        {
            _outcomes.Add(new GoldOutcome(50));
            // Mercenaries
            // Technology (scrolls)
            // Advanced Tribe (new city)
            // Barbarians
        }

        public GoodyHutOutcomeResult Trigger(Unit unit)
        {
            // Base chance of each outcome is equal. Certain game circumstances modify this ratio.
            // https://apolyton.net/forum/civilization-series/civilization-i-and-civilization-ii/82184-a-study-of-hut-outcomes
            var outcome = _outcomes[new Random().Next(0, _outcomes.Count)];
            return outcome.ApplyOutcome(unit);
        }
    }
}
