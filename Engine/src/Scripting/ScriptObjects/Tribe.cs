using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Scripting.ScriptObjects;

// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

namespace Civ2engine.Scripting;

public class Tribe(Civilization civ, Game game)
{
    public Civilization Civ { get; } = civ;

    public bool active => Civ.Alive;

    public string adjective
    {
        get => Civ.Adjective;
        set => Civ.Adjective = value;
    }

    public AttitudeProxy attitude => new AttitudeProxy(Civ);

    public int betrayals
    {
        get => Civ.Betrayals;
        set => Civ.Betrayals = value < 0 ? 0 : value;
    }

    public int futureTechs
    {
        get => Civ.FutureTechCount;
        set => Civ.FutureTechCount = value < 0 ? 0 : value;
    }

    public int government
    {
        get => Civ.Government;
        set => Civ.Government = value < 0 ? 0 : value;
    }

    public int id => Civ.Id;

    public bool isHuman
    {
        get => Civ.PlayerType is PlayerType.Local or PlayerType.Remote;
        set => Civ.PlayerType = value ? PlayerType.Local : PlayerType.Ai;
    }

    public Leader leader => new Leader(Civ);

    public int money
    {
        get => Civ.Money;
        set => Civ.Money = value;
    }

    public string name
    {
        get => Civ.TribeName;
        set => Civ.TribeName = value;
    }

    public int numCities => Civ.Cities.Count;

    public int numTechs
    {
        get => Civ.Advances.Count(a => a);
        set
        {
            // Read-only - a set accessor here makes no sence what does TOTIP do with this information
        }
    }

    public int numUnits => Civ.Units.Count;

    public int patience
    {
        get => Civ.Patience;
        set => Civ.Patience = value;
    }

    public ReputationProxy reputation => new (Civ);

    public int researchCost => AdvanceFunctions.CalculateScienceCost(game, Civ);

    public int researchProgress
    {
        get => Civ.Science;
        set => Civ.Science = value;
    }

    public Tech? researching
    {
        get => Civ.ReseachingAdvance == -1 ? null : new Tech(game.Rules.Advances, Civ.ReseachingAdvance);
        set => Civ.ReseachingAdvance = value?.id ?? -1;
    }

    public int scienceRate => Civ.ScienceRate;

    public Spaceship spaceship => new Spaceship();

    public int taxRate => Civ.TaxRate;

    public TreatiesProxy treaties => new TreatiesProxy(Civ);
}