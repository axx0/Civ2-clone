using System;
using Civ2engine.Units;
using Eto.Drawing;
using EtoFormsUI.Animations;

namespace EtoFormsUI
{
    internal class MoveAnimation : BaseAnimation
    {
        private readonly Sound _sound;

        public MoveAnimation(Unit unit, Sound sound) 
            : base(GetAnimationFrames.UnitMoving(unit).ToArray(), 6, 7, 0.02, unit.PrevXY)
        {
            _sound = sound;
        }

        public override float GetXDrawOffset(int mapXpx, int startX)
        {
            return mapXpx * (XY[0] - startX - 2);
        }

        public override int GetYDrawOffset(int mapYpx, int startY)
        {
            return mapYpx * (XY[1] - startY - 3);
        }

        public override void Initialize()
        {
            _sound.PlaySound(GameSounds.Move);
        }
    }
}