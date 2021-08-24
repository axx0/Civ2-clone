using System;
using Civ2engine.Events;
using Civ2engine.Units;
using Eto.Drawing;
using EtoFormsUI.Animations;

namespace EtoFormsUI
{
    internal class MoveAnimation : BaseAnimation
    {
        private readonly Sound _sound;

        public MoveAnimation(MovementEventArgs args, Sound sound) 
            : base(GetAnimationFrames.UnitMoving(args.Unit).ToArray(), 6, 7, 0.02, args.Location[0])
        {
            _sound = sound;
        }

        public override float GetXDrawOffset(int mapXpx, int startX)
        {
            return mapXpx * (Location.X - startX - 2);
        }

        public override int GetYDrawOffset(int mapYpx, int startY)
        {
            return mapYpx * (Location.Y - startY - 3);
        }

        public override void Initialize()
        {
            _sound.PlaySound(GameSounds.Move);
        }
    }
}