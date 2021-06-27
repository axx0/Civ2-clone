namespace Civ2engine.UnitActions.Move
{
    public class MoveNorthEast : IGameAction
    {
        public void TriggerAction()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] == 0)
            {
                return;
            }
            if (Game.Instance.CurrentMap.ActiveXY[0] < Game.Instance.CurrentMap.XDim*2-2)
            {
                MovementFunctions.MoveC2(1, -1);
            }else if (!Game.Instance.Options.FlatEarth)
            {
                MovementFunctions.MoveC2(-Game.Instance.CurrentMap.XDim*2 +2 , -1);
            }
        }
    }
}