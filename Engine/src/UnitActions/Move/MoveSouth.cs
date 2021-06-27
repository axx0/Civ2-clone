namespace Civ2engine.UnitActions.Move
{
    public class MoveSouth : IGameAction
    {
        public void TriggerAction()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] < Game.Instance.CurrentMap.YDim-2)
            {
                MovementFunctions.MoveC2(0, 2);
            }
        }
    }
}