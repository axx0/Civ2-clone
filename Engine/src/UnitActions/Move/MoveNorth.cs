namespace Civ2engine.UnitActions.Move
{
    public class MoveNorth : IGameAction
    {
        public void TriggerAction()
        {
            if (Game.Instance.CurrentMap.ActiveXY[1] > 1)
            {
                MovementFunctions.MoveC2(0, -2);
            }
        }
    }
}