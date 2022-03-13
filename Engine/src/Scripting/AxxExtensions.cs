using System;
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
    }

    public class EffectsMap
    {
        public ImprovementEffect Unique = ImprovementEffect.Unique;
    }
}