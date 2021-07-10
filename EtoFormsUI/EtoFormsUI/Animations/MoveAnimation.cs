using System;
using Civ2engine.Units;
using Eto.Drawing;
using EtoFormsUI.Animations;

namespace EtoFormsUI
{
    internal class MoveAnimation : BaseAnimation
    {
        public MoveAnimation(Unit unit) : base(GetAnimationFrames.UnitMoving(unit).ToArray(), 6, 7, 0.02, unit.PrevXY)
        {
        }

        public override float GetXDrawOffset(int mapXpx, int startX)
        {
            return mapXpx * (XY[0] - startX - 2);
        }

        public override int GetYDrawOffset(int mapYpx, int startY)
        {
            return mapYpx * (XY[1] - startY - 3);
        }
    }
}