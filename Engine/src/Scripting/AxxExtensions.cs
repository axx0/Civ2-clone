using System;
using System.Text;
using System.Threading.Tasks;
using Civ2engine.Enums;
using Civ2engine.Improvements;
using Neo.IronLua;

// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting
{
    public class AxxExtensions
    {
        private readonly Game _game;
        private readonly StringBuilder _log;

        public AxxExtensions(Game game, StringBuilder log)
        {
            _game = game;
            _log = log;
        }

        public EffectsMap Effects { get; } = new();
        
        
        public UnitDomainMap UnitDomain { get; } = new();


        public AbilityMap Ability { get; } = new ();

        public void onCivInit(Func<Civilization, object> action)
        {
            _game.OnCivEvent += (sender, args) =>
            {
                if (args.EventType != CivEventType.Created) return;

                try
                {
                    action(args.Civ);
                }
                catch(LuaException ex)
                {
                    _log.AppendLine("Error running onCivInit for " + args.Civ.TribeName);
                    _log.AppendLine(ex.Message);
                }
            };
        }

        public Rules rules => _game.Rules;
    }

    public class AbilityMap
    {
        public ConstructionAbility BuildRoad = ConstructionAbility.Road;
        public ConstructionAbility BuildRailroad = ConstructionAbility.Railroad;
        public ConstructionAbility BuildIrrigation = ConstructionAbility.Irrigation;
        public ConstructionAbility BuildFarmland = ConstructionAbility.Farmland;
        public ConstructionAbility BuildFortress = ConstructionAbility.Fortress;
        public ConstructionAbility BuildAirbase = ConstructionAbility.Airbase;
        public ConstructionAbility BuildMine = ConstructionAbility.Mine;
    }

    public class UnitDomainMap
    {
        public int Land = (int)UnitGAS.Ground;
        public int Sea = (int)UnitGAS.Sea;
        public int Air = (int)UnitGAS.Air;
        public int Special = (int)UnitGAS.Special;
    }

    public class EffectsMap
    {
        public ImprovementEffect Unique = ImprovementEffect.Unique;
        public ImprovementEffect Capital = ImprovementEffect.Capital;
        public ImprovementEffect FoodStorage = ImprovementEffect.FoodStorage;
        public ImprovementEffect Veteran = ImprovementEffect.Veteran;
        public ImprovementEffect ContentFace = ImprovementEffect.ContentFace;
        public ImprovementEffect HappyFace = ImprovementEffect.HappyFace;
    }
}