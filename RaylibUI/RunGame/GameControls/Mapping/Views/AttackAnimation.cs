using System.Numerics;
using Civ2engine.Events;
using Civ2engine.MapObjects;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

internal class AttackAnimation : BaseGameView
{
    public AttackAnimation(GameScreen gameScreen, CombatEventArgs args, IGameView? previousView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, args.Location.First(), previousView, viewHeight, viewWidth,
        false, 70, args.Location, forceRedraw)
    {

        var activeInterface = gameScreen.Main.ActiveInterface;

        var unitAnimations = new List<IViewElement>();
        ImageUtils.GetUnitTextures(args.Attacker, activeInterface, unitAnimations, ActivePos, true);
        var defPos = GetPosForTile(args.Defender.CurrentLocation);
        ImageUtils.GetUnitTextures(args.Defender, activeInterface, unitAnimations,
            defPos, true);
        var battleAnimation = activeInterface.UnitImages.BattleAnim;
        var explosion = 0;
        SetAnimation(unitAnimations);
        var attackerTexture = activeInterface.UnitImages.Units[(int)args.Attacker.Type].Texture;
        var attackPos = ActivePos + new Vector2(attackerTexture.width /2f- battleAnimation[0].width/2f, attackerTexture.height / -2f+ battleAnimation[0].height /2f);
        var defenderTexture = activeInterface.UnitImages.Units[(int)args.Defender.Type].Texture;

        defPos += new Vector2(defenderTexture.width /2f - battleAnimation[0].width/2f, defenderTexture.height / -2f + battleAnimation[0].height /2f);
        do
        {
            var attackerWins = args.CombatRoundsAttackerWins[explosion];

            unitAnimations = AddJustAnimations(unitAnimations, args.Attacker.Hitpoints[explosion],
                args.Defender.Hitpoints[explosion]);
            var expPos = attackerWins ? defPos : attackPos;
            foreach (var battleTexture in battleAnimation)
            {
                SetAnimation(unitAnimations.Concat(new[] { new TextureElement(battleTexture, expPos, Location) })
                    .ToList());
            }

            explosion += 5;
        } while (explosion < args.CombatRoundsAttackerWins.Count);
    }

    private List<IViewElement> AddJustAnimations(List<IViewElement> unitAnimations, params int[] hitpoints)
    {
        int idx = 0;
        return unitAnimations.Select(a =>
        {
            if (a is HealthBar health)
            {
                return new HealthBar(health.Location, health.Tile, hitpoints[idx++], health.BaseHitpoints);
            }

            return a;
        }).ToList();
    }
}