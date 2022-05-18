using System;
using System.Collections.Generic;
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

        public IDictionary<int, TerrainImprovement> TerrainImprovements => _game.TerrainImprovements;
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
        public Effects Unique = Effects.Unique;
        public Effects Capital = Effects.Capital;
        public Effects FoodStorage = Effects.FoodStorage;
        public Effects Veteran = Effects.Veteran;
        public Effects ContentFace = Effects.ContentFace;
        public Effects HappyFace = Effects.HappyFace;
        public Effects TaxMultiplier = Effects.TaxMultiplier;
        public Effects LuxMultiplier = Effects.LuxMultiplier;
        public Effects ScienceMultiplier = Effects.ScienceMultiplier;
        
        public Effects EliminateIndustrialPollution = Effects.EliminateIndustrialPollution;
        public Effects IndustrialPollutionModifier = Effects.IndustrialPollutionModifier;
        public Effects EliminatePopulationPollution = Effects.EliminatePopulationPollution;
        public Effects PopulationPollutionModifier = Effects.PopulationPollutionModifier;
    }
}