using System.Numerics;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using RaylibUI.RunGame.GameControls.Mapping.Views.ViewElements;
using Model;
using Model.ImageSets;

namespace RaylibUI.RunGame.GameControls.Mapping.Views;

internal class AttackAnimation : BaseGameView
{
    public AttackAnimation(GameScreen gameScreen, CombatEventArgs args, IGameView? previousView, int viewHeight,
        int viewWidth, bool forceRedraw) : base(gameScreen, args.Location.First(), previousView, viewHeight, viewWidth,
        false, 70, args.Location, forceRedraw)
    {
        var active = gameScreen.Main.ActiveInterface;
        var game = gameScreen.Game;

        var unitAnimations = new List<IViewElement>();
        var attackerPos  = ActivePos with{ Y = ActivePos.Y + Dimensions.TileHeight - active.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom) };
        ImageUtils.GetUnitTextures(args.Attacker, active, game, unitAnimations, attackerPos );
        var defPos = GetPosForTile(args.Defender.CurrentLocation);
        var defenderPos = defPos with { Y = defPos.Y + Dimensions.TileHeight - active.UnitImages.UnitRectangle.Height.ZoomScale(gameScreen.Zoom) };
        ImageUtils.GetUnitTextures(args.Defender, active, game, unitAnimations,
            defenderPos);
        var explosion = 0;
        //SetAnimation(unitAnimations);
        var battleAnimation = active.UnitImages.BattleAnim.Select(a => TextureCache.GetImage(a)).ToArray();
        var attackPos = ActivePos  + new Vector2(Dimensions.HalfWidth - battleAnimation[0].Width/2f, Dimensions.HalfHeight - battleAnimation[0].Height /2f);
        
        defPos += new Vector2(Dimensions.HalfWidth - battleAnimation[0].Width / 2f, Dimensions.HalfHeight - battleAnimation[0].Height /2f);
        do
        {
            var attackerWins = args.CombatRoundsAttackerWins[explosion];
            unitAnimations = AddJustAnimations(unitAnimations, active.UnitShield((int)args.Attacker.Type), args.Attacker.Hitpoints[explosion], args.Defender.Hitpoints[explosion]);
            var expPos = attackerWins ? defPos : attackPos;
            foreach (var battleTexture in battleAnimation)
            {
                SetAnimation(unitAnimations.Concat([new TextureElement(battleTexture, expPos, Location)])
                    .ToList());
            }

            explosion += 5;
        } while (explosion < args.CombatRoundsAttackerWins.Count);
    }

    private List<IViewElement> AddJustAnimations(List<IViewElement> unitAnimations, UnitShield shield, params int[] hitpoints)
    {
        int idx = 0;
        return unitAnimations.Select(a =>
        {
            if (a is HealthBar health)
            {
                return new HealthBar(health.Location, health.Tile, hitpoints[idx++], health.BaseHitpoints, health.Offset, shield);
            }

            return a;
        }).ToList();
    }
}