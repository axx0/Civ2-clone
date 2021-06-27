using System;

namespace Civ2engine.UnitActions
{
    public static class BuildCity 
    {
        public static Action CreateCityBuild(Func<string, BuildCityConfirmResult> confirmCityBuild)
        {
            return () =>
            {
                var game = Game.Instance;
                
                if (!game.ActiveUnit.TypeDefinition.CanBuildCities) return;

                if (!game.CurrentMap.ActiveTile.Terrain.CanHaveCity) return;
                
                var confirmed = confirmCityBuild("Dummy Name");
                if (confirmed.Build)
                {

                }
            };
        }
    }
}