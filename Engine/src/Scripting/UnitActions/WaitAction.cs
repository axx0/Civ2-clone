using Model.Core.Units;

namespace Civ2engine.Scripting.UnitActions;

public class WaitAction(Unit baseUnit, Game game, AiPlayer aiPlayer) : UnitAction(baseUnit, "Wait", game)
{
    public override void Execute()
    {
        aiPlayer.WaitingList.Add(BaseUnit);
        game.ChooseNextUnit();
    }
}