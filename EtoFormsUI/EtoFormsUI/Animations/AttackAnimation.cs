using Civ2engine.Events;
using Civ2engine.Units;
using Eto.Drawing;

namespace EtoFormsUI.Animations
{
    public class AttackAnimation : BaseAnimation
    {
        private readonly Sound _sounds;
        private readonly Unit _unit;

        public AttackAnimation(UnitEventArgs unitEventArgs, Sound sounds) : base(GetAnimationFrames.UnitAttack(unitEventArgs).ToArray(),6,7, 0.07, unitEventArgs.Unit.XY)
        {
            _sounds = sounds;
            _unit = unitEventArgs.Unit;
        }

        public override float GetXDrawOffset(int mapXpx, int startX)
        {
            return mapXpx * (XY[0]  -  startX - 2);
        }

        public override int GetYDrawOffset(int mapYpx, int startY)
        {
            return mapYpx * (XY[1] - startY - 3);
        }

        public override void Initialize()
        {
            _sounds.PlaySound(GameSounds.Attack, _unit.AttackSound);
        }
    }
}