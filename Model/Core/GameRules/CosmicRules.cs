namespace Civ2engine
{
    public class CosmicRules
    {
        public int MovementMultiplier { get; set; } = 3;
        public int ChanceTriremeLost { get; private set; } = 2;
        public int FoodEatenPerTurn { get; private set; } = 2;
        public int RowsFoodBox { get; private set; } = 10;
        public int RowsShieldBox { get; private set; } = 10;
        public int SettlersEatTillMonarchy { get; private set; } = 1;
        public int SettlersEatFromCommunism { get; private set; } = 2;
        public int CitySizeUnhappyChieftain { get; private set; } = 7;
        public int RiotFactor { get; private set; } = 14;
        public int ToExceedCitySizeAqueductNeeded { get; private set; } = 8;
        public int SewerNeeded { get; private set; } = 12;
        public int TechParadigm { get; private set; } = 10;
        public int BaseTimeEngineersTransform { get; private set; } = 20;
        public int MonarchyPaysSupport { get; private set; } = 3;
        public int CommunismPaysSupport { get; private set; } = 3;
        public int FundamentalismPaysSupport { get; private set; } = 10;
        public int CommunismEquivalentPalaceDistance { get; private set; } = 0;
        public int FundamentalismScienceLost { get; private set; } = 50;
        public int ShieldPenaltyTypeChange { get; private set; } = 50;
        public int MaxParadropRange { get; private set; } = 10;
        public int MassThrustParadigm { get; private set; } = 75;

        // Max effective science rate in fundamentalism (x10, so 5 = 50%)
        public int MaxEffectiveScienceRate { get; private set; } = 5;

        /// Value of each citizen to the Civilization Score.
        public int CitizenValue { get; private set; } = 1;

        ///	 Value of each wonder to the Civilization Score.
        public int WonderValue { get; private set; } = 20;

        /// Reward for landing on A. Centauri first = this multiplier*(# of habitats)*(prob. of success) 
        public int AlphaCentariLandingValue { get; set; } = 1;

        /// Cost to Civilization Score (+ or -) for each extant non-AI controlled polluted tile.
        public int PolutionValue { get; set; } = -10;

        /// For each turn of peace after turn 199, this amount *3 is added to Civilization Score.
        public int PeaceValue { get; set; } = 1;

        /// Value to the Civilization Score of each future tech researched.
        public int FutureTechValue { get; set; } = 5;

        /// Penalty assessed to Civilization Score each time player betrays another race.
        public int BetrayalValue { get; set; } = 0;

        /// Cost to Civilization Score (+ or -) for each unit destroyed.
        public int UnitDestroyedValue { get; set; } = 0;

        /// <summary>
        /// bitmask for goodie huts, right bit =map0, 0=no goodie huts
        /// </summary>
        public bool[] MapHasGoddyHuts { get; set; }

        /// Helicopters pick up huts 0=no 1=yes
        public bool HelicoptersCanCollectHuts { get; set; } = false;

        public int RoadMovement { get; set; } = 1;
        public int RiverMovement { get; set; } = 1;
        public int AlpineMovement { get; set; } = 1;
        public int RailroadMovement { get; set; } = 0;
    }
}