namespace Civ2engine.UnitActions.Move
{
    public class MoveSouthWest : IGameAction
    {
        public void TriggerAction()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] >= Game.Instance.CurrentMap.YDim -1)
            {
                return;
            }
            
            if (Game.Instance.CurrentMap.ActiveXY[0] > 0)
            {
                MovementFunctions.MoveC2(-1, 1, true);
            }
            else if(!Game.Instance.Options.FlatEarth)
            {
                MovementFunctions.MoveC2(Game.Instance.CurrentMap.XDim*2-2, 1, true);
            }
        }
    }
}