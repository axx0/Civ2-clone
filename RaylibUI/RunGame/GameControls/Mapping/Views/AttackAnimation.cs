using Civ2engine.Events;
using Civ2engine.MapObjects;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

internal class AttackAnimation : BaseGameView
{
    public AttackAnimation(GameScreen gameScreen, CombatEventArgs args, IGameView? previousView, int viewHeight,
        int viewWidth) : base(gameScreen, args.Location.First(), previousView, viewHeight, viewWidth, false, 70, args.Location)
    {
    }
}