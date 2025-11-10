using System.Collections.Generic;
using System.Linq;
using Civ2engine;
using Civ2engine.Terrains;

// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global

public class CosmicScripts(Game game)
{
    /// <summary>
    /// The distance from palace used in happiness calculations under Communism.
    /// </summary>
    public int communismPalaceDistance
    {
        get => game.Rules.Cosmic.CommunismEquivalentPalaceDistance; 
        set => game.Rules.Cosmic.CommunismEquivalentPalaceDistance = value;
    }

    /// <summary>
    /// The amount of food eaten by each citizen each turn.
    /// </summary>
    public int foodEaten { get => game.Rules.Cosmic.FoodEatenPerTurn; set => game.Rules.Cosmic.FoodEatenPerTurn = value; }

    /// <summary>
    /// The number of rows in the food box.
    /// </summary>
    public int foodRows { get => game.Rules.Cosmic.RowsFoodBox; set => game.Rules.Cosmic.RowsFoodBox = value; }

    /// <summary>
    /// The bitmask for goodie huts.
    /// </summary>
    public int goodieHutsMask { get => Utils.ToBitmask(game.Rules.Cosmic.MapHasGoddyHuts); set => Utils.FromBitmask(value); }

    /// <summary>
    /// Whether helicopters (domain 1, range 0 units) pick up huts or not.
    /// </summary>
    public int helisPickupHuts { get => game.Rules.Cosmic.HelicoptersCanCollectHuts ? 1:0; set=> game.Rules.Cosmic.HelicoptersCanCollectHuts = value != 0; }

    /// <summary>
    /// The mass/thrust paradigm.
    /// </summary>
    public int massThrustParadigm { get => game.Rules.Cosmic.MassThrustParadigm; set => game.Rules.Cosmic.MassThrustParadigm = value; }

    /// <summary>
    /// The number of unit types from the @COSMIC2 key of the same name.
    /// </summary>
    public int numberOfUnitTypes => game.Rules.UnitTypes.Length;

    /// <summary>
    /// The maximum paradrop range.
    /// </summary>
    public int paradropRange { get => game.Rules.Cosmic.MaxParadropRange; set => game.Rules.Cosmic.MaxParadropRange = value; }

    /// <summary>
    /// The penalty to the civilization score for each betrayal of another tribe.
    /// </summary>
    public int penaltyBetrayal { get => game.Rules.Cosmic.BetrayalValue; set => game.Rules.Cosmic.BetrayalValue = value; }

    /// <summary>
    /// The shield penalty percentage for changing production types.
    /// </summary>
    public int prodChangePenalty { get => game.Rules.Cosmic.ShieldPenaltyTypeChange; set=> game.Rules.Cosmic.ShieldPenaltyTypeChange = value; }

    /// <summary>
    /// The riot factor based on the number of cities.
    /// </summary>
    public int RiotFactor { get => game.Rules.Cosmic.RiotFactor; set => game.Rules.Cosmic.RiotFactor = value; }

    /// <summary>
    /// The road movement multiplier.
    ///
    /// Note this is the common movement multiplier which is only the same as road if all multipliers are the same, this is the behaviour of TOTIP 
    /// </summary>
    public int roadMultiplier
    {
        get => game.Rules.Cosmic.MovementMultiplier;
        set
        {
            if(value <= 0 || value == game.Rules.Cosmic.MovementMultiplier) return;
            throw new System.NotImplementedException();
            // TODO: finish this code 
            // var movements = new List<int> { game.Rules.Cosmic.RiverMovement, game.Rules.Cosmic.AlpineMovement, game.Rules.Cosmic.RoadMovement, game.Rules.Cosmic.RailroadMovement };
            //
            // TerrainImprovement? roads = null;
            // var otherActions = new List<TerrainImprovementAction>();
            // foreach (var improvement in game.Rules.TerrainImprovements)
            // {
            //     if (improvement.Id == ImprovementTypes.Road)
            //     {
            //         roads = improvement;
            //         break;
            //     }
            //     otherActions.AddRange(improvement.Levels[0].Effects.Where(e => e is { Target: ImprovementConstants.Movement, Value: > 0 }));
            // }
            //
            // if (roads == null)
            // {
            //     movements.RemoveRange(2, 2);
            // }
            // else
            // {
            //     var road = roads.Levels[0];
            //     var roadEffect = road.Effects.FirstOrDefault(e => e is { Target: ImprovementConstants.Movement});
            //     if (roadEffect != null)
            //     {
            //     }
            //     
            //     
            // }
            //
            // game.Rules.Cosmic.MovementMultiplier = value;
            // foreach (var VARIABLE in game.Rules.UnitTypes)  
            // {
            //     
            // }
        }
    }

    /// <summary>
    /// The percentage of science lost under Fundamentalism.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int scienceLostFundamentalism
    {
        get => game.Rules.Governments[4].GlobalResourceWastage.GetValueOrDefault("Science", 0);
        set => game.Rules.Governments[4].GlobalResourceWastage["Science"] = value;
    }

    /// <summary>
    /// The maximum effective science rate under Fundamentalism.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int scienceRateFundamentalism {
        get => game.Rules.Governments[4].MaxRates["Science"];
        set => game.Rules.Governments[4].MaxRates["Science"] = value;
    }

    /// <summary>
    /// The civilization score for each landing on Alpha Centauri first. Multiplied by number of habitats and success probability.
    /// </summary>
    public int scoreCentauri { get => game.Rules.Cosmic.AlphaCentariLandingValue; set => game.Rules.Cosmic.AlphaCentariLandingValue = value; }

    /// <summary>
    /// The civilization score for each citizen.
    /// </summary>
    public int scoreCitizen { get => game.Rules.Cosmic.CitizenValue; set => game.Rules.Cosmic.CitizenValue = value; }

    /// <summary>
    /// The civilization score for each future technology researched.
    /// </summary>
    public int scoreFutureTech { get => game.Rules.Cosmic.FutureTechValue; set => game.Rules.Cosmic.FutureTechValue = value; }

    /// <summary>
    /// The civilization score for each turn of peace after turn 199.
    /// </summary>
    public int scorePeace { get => game.Rules.Cosmic.PeaceValue; set => game.Rules.Cosmic.PeaceValue = value; }

    /// <summary>
    /// The civilization score for each extant non-AI controlled polluted tile. Normally a negative value.
    /// </summary>
    public int scorePollution { get => game.Rules.Cosmic.PolutionValue; set => game.Rules.Cosmic.PolutionValue = value; }

    /// <summary>
    /// The civilization score for each unit killed.
    /// </summary>
    public int scoreUnitKilled { get => game.Rules.Cosmic.UnitDestroyedValue; set => game.Rules.Cosmic.UnitDestroyedValue = value; }

    /// <summary>
    /// The civilization score for each wonder.
    /// </summary>
    public int scoreWonder { get => game.Rules.Cosmic.WonderValue; set => game.Rules.Cosmic.WonderValue = value; }

    /// <summary>
    /// The amount of food eaten by settlers for governments ≥ Communism.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int settlersEatHigh {         set
    {
        game.Rules.Cosmic.SettlersEatFromCommunism = value;
        for (var i = game.Rules.Governments.Length - 1; i >= 3; i--)
        {
            game.Rules.Governments[i].SettlersConsumption = value;
        }
    }}

    /// <summary>
    /// The amount of food eaten by settlers for governments ≤ Monarchy.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int settlersEatLow { get => game.Rules.Cosmic.SettlersEatTillMonarchy;
        set
        {
            game.Rules.Cosmic.SettlersEatTillMonarchy = value;
            for (var i = 0; i < 3; i++)
            {
                game.Rules.Governments[i].SettlersConsumption = value;
            }
        } 
    }

    /// <summary>
    /// The number of rows in the shield box.
    /// </summary>
    public int shieldRows { get => game.Rules.Cosmic.RowsShieldBox; set => game.Rules.Cosmic.RowsShieldBox = value; }

    /// <summary>
    /// The city size that cannot be exceeded without an Aquaduct.
    ///
    /// TODO: link to aquaduct improvement
    /// </summary>
    public int sizeAquaduct { get => game.Rules.Cosmic.ToExceedCitySizeAqueductNeeded; set => game.Rules.Cosmic.ToExceedCitySizeAqueductNeeded = value; }

    /// <summary>
    /// The city size that cannot be exceeded without a Sewer System.
    ///
    /// TODO: link to sewer improvement
    /// </summary>
    public int sizeSewer { get => game.Rules.Cosmic.SewerNeeded; set => game.Rules.Cosmic.SewerNeeded = value; }

    /// <summary>
    /// The city size at which the first unhappy citizen appears at Chieftain difficulty.
    /// </summary>
    public int sizeUnhappiness { get => game.Rules.Cosmic.CitySizeUnhappyChieftain; set => game.Rules.Cosmic.CitySizeUnhappyChieftain = value; }

    /// <summary>
    /// The number of units that are free of support under Communism.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int supportCommunism {        get => game.Rules.Governments[3].NumberOfFreeUnitsPerCity;
        set
        {
            game.Rules.Governments[3].NumberOfFreeUnitsPerCity = value;
            game.Rules.Cosmic.CommunismPaysSupport = value ;
        }  }

    /// <summary>
    /// The number of units that are free of support costs under Fundamentalism.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int supportFundamentalism { 
        get => game.Rules.Governments[4].NumberOfFreeUnitsPerCity;
        set
        {
            game.Rules.Governments[4].NumberOfFreeUnitsPerCity = value;
            game.Rules.Cosmic.FundamentalismPaysSupport = value ;
        } }

    /// <summary>
    /// The number of units that are free of support under Monarchy.
    ///
    /// This is supported for legacy purposes newer scripts should access the government object
    /// </summary>
    public int supportMonarchy
    {
        get => game.Rules.Governments[2].NumberOfFreeUnitsPerCity;
        set
        {
            game.Rules.Governments[2].NumberOfFreeUnitsPerCity = value;
            game.Rules.Cosmic.MonarchyPaysSupport = value ;
        }
    }

    /// <summary>
    /// The tech paradigm. Scenarios use civ.scen.params.techParadigm instead of this value.
    /// </summary>
    public int techParadigm { get => game.Rules.Cosmic.TechParadigm; set => game.Rules.Cosmic.TechParadigm = value; }

    /// <summary>
    /// The base time needed for engineers to transform terrain.
    /// </summary>
    public int transformBase { get => game.Rules.Cosmic.BaseTimeEngineersTransform; set => game.Rules.Cosmic.BaseTimeEngineersTransform = value; }

    /// <summary>
    /// The 1 in x chance of a trireme getting lost at sea.
    /// </summary>
    public int triremeLost { get => game.Rules.Cosmic.ChanceTriremeLost; set => game.Rules.Cosmic.ChanceTriremeLost = value; }
}