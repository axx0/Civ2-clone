using System;

namespace Civ2engine;

public class Barbarians
{
    public static readonly Civilization Civilization = new()
    {
        Adjective = Labels.Items[17], LeaderName = Labels.Items[18], Alive = true, Id = 0, TribeId = -1,
        PlayerType = PlayerType.Barbarians, Advances = []
    };
}