using Civ2engine.Events;
using Eto.Drawing;

namespace EtoFormsUI.Animations
{
    public class AttackAnimation : BaseAnimation
    {
        public AttackAnimation(UnitEventArgs unitEventArgs) : base(GetAnimationFrames.UnitAttack(unitEventArgs).ToArray(),6,7, 0.07, unitEventArgs.Unit.XY)
        {
        }

        public override float GetXDrawOffset(int mapXpx, int startX)
        {
            return mapXpx * (XY[0]  -  startX - 2);
        }

        public override int GetYDrawOffset(int mapYpx, int startY)
        {
            return mapYpx * (XY[1] - startY - 3);
        }
    }
}