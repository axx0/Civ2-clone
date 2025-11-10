namespace Civ2engine;

public static class HistoryUtils
{
    /*
     * Reconstructs an artificial History based on an initial game state.
     *
     * When creating a new Game, this ensures that initial technologies are recorded as discovered on turn 1.
     *
     * When creating a Game from a Legacy SAV/SCN file:
     * The legacy SAV/SCN format does not persist turn-by-turn historical information,
     * so this method allows events like CityBuilt and AdvanceDiscovered to be recorded in History.
     * But, as a compromise all events will appear to have happened on the turn that the game was loaded.
     */
    public static History ReconstructHistory(Game game)
    {
        History history = new History(game);
        foreach (var city in game.AllCities)
        {
            history.CityBuilt(city);
        }
        foreach (var civ in game.AllCivilizations)
        {
            if (civ.Advances != null)
            {
                for (int advanceNumber = 0; advanceNumber < civ.Advances.Length; advanceNumber++)
                {
                    if (civ.Advances[advanceNumber])
                    {
                        history.AdvanceDiscovered(advanceNumber, civ.Id);
                    }
                }
            }
        }
        return history;
    }
}