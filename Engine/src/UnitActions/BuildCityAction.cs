using System;

namespace Civ2engine.UnitActions
{

    public class BuildCityAction : IGameAction
    {
        private readonly Func<string, BuildCityConfirmResult> _confirmCityBuild;

        public BuildCityAction(Func<string, BuildCityConfirmResult> confirmCityBuild)
        {
            _confirmCityBuild = confirmCityBuild;
        }
        public void TriggerAction()
        {
            var game = Game.Instance;
            if (game.ActiveUnit.TypeDefinition.CanBuildCities)
            {
                if (game.CurrentMap.ActiveTile.Terrain.CanHaveCity)
                {
                    var confirmed = _confirmCityBuild("Dummy Name");
                    if (confirmed.Build)
                    {
                    
                    }
                }
            }
        }
    }

    public class BuildCityConfirmResult
    {
        public string Name { get; set; }
        public bool Build { get; set; }
    }
}