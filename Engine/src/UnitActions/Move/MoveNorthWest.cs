namespace Civ2engine.UnitActions.Move
{
    public class MoveNorthWest : IGameAction
    {
        public void TriggerAction()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] == 0)
            {
                return;
            }
            if (Game.Instance.CurrentMap.ActiveXY[0] > 0)
            {
                MovementFunctions.MoveC2(-1, -1);
            }
            else if(!Game.Instance.Options.FlatEarth)
            {
                MovementFunctions.MoveC2(Game.Instance.CurrentMap.XDim*2-1, -1);
            }
        }
    }
}