namespace Civ2engine.UnitActions.Move
{
    public class MoveEast : IGameAction
    {
        public void TriggerAction()
        {
            if (Game.Instance.CurrentMap.ActiveXY[0] < Game.Instance.CurrentMap.XDim *2 -2)
            {
                MovementFunctions.MoveC2(2, 0);
            }else if (!Game.Instance.Options.FlatEarth)
            {
                MovementFunctions.MoveC2(-Game.Instance.CurrentMap.XDim*2 +2 , 0);
            }
        }
    }
}