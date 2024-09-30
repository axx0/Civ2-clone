using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Civ2engine.Units;
using Model.Constants;
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

        public ResourceList Resources { get; } = new();

        public TerrainImprovements Improvements { get; } = new();
        public void OnCivInit(Func<Civilization, object> action)
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

        public Rules Rules => _game.Rules;

        public IDictionary<int, TerrainImprovement> TerrainImprovements => _game.TerrainImprovements;
    }

    public class TerrainImprovements
    {
        public int Irrigation = ImprovementTypes.Irrigation;
        public int Mining = ImprovementTypes.Mining;
        public int Road = ImprovementTypes.Road;
        public int Fortress = ImprovementTypes.Fortress;
        public int Pollution = ImprovementTypes.Pollution;
        public int Airbase = ImprovementTypes.Airbase;
    }

    public class ResourceList
    {
        public int Food = ImprovementConstants.Food;
        public int Shields = ImprovementConstants.Shields;
        public int Trade = ImprovementConstants.Trade;
    }


    public class UnitDomainMap
    {
        public int Land = (int)UnitGas.Ground;
        public int Sea = (int)UnitGas.Sea;
        public int Air = (int)UnitGas.Air;
        public int Special = (int)UnitGas.Special;
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
        public Effects Walled = Effects.Walled;
        public Effects AirDefence = Effects.AirDefence;
        public Effects SeaDefence = Effects.SeaDefence;
        public Effects MissileDefence = Effects.MissileDefence;
        public Effects ReduceCorruption = Effects.ReduceCorruption;

        public Effects OceanRequired = Effects.OceanRequired;
        
        public Effects EliminateIndustrialPollution = Effects.EliminateIndustrialPollution;
        public Effects IndustrialPollutionModifier = Effects.IndustrialPollutionModifier;
        public Effects EliminatePopulationPollution = Effects.EliminatePopulationPollution;
        public Effects PopulationPollutionModifier = Effects.PopulationPollutionModifier;

        public Effects Epoch = Effects.EpochTech;

        public UnitEffect Partisan = UnitEffect.Partisan;
        public UnitEffect SdiVulnerable = UnitEffect.SDIVulnerable;
    }
}