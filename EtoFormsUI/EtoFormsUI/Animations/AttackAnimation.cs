using Civ2engine.Events;
using Civ2engine.Units;
using Eto.Drawing;

namespace EtoFormsUI.Animations
{
    public class AttackAnimation : BaseAnimation
    {
        private readonly Sound _sounds;
        private readonly string _combatSound;

        public AttackAnimation(CombatEventArgs unitEventArgs, Sound sounds) : base(
            GetAnimationFrames.UnitAttack(unitEventArgs).ToArray(), 6, 7, 0.07, unitEventArgs.Location[0])
        {
            _sounds = sounds;
            _combatSound = unitEventArgs.Sound;
        }

        public override float GetXDrawOffset(int mapXpx, int startX)
        {
            return mapXpx * (Location.X  -  startX - 2);
        }

        public override int GetYDrawOffset(int mapYpx, int startY)
        {
            return mapYpx * (Location.Y - startY - 3);
        }

        public override void Initialize()
        {
            _sounds.PlaySound(GameSounds.Attack, _combatSound);
        }
    }
}