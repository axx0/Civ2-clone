using Model.Core.Units;

namespace Civ2engine.UnitActions;

public class WaitAction(Unit unit, Game game, AiPlayer aiPlayer) : UnitAction(unit, "Wait")
{
    public override void Execute()
    {
        aiPlayer.WaitingList.Add(Unit);
        game.ChooseNextUnit();
    }
}