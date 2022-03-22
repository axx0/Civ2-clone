using Civ2engine.Enums;
using Civ2engine.Improvements;
// ReSharper disable UnusedMember.Global

namespace Civ2engine.Scripting
{
    public class AxxExtensions
    {
        private readonly Game _game;

        public AxxExtensions(Game game)
        {
            _game = game;
        }

        public EffectsMap Effects { get; } = new();
        
        
        public UnitDomainMap UnitDomain { get; } = new();
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