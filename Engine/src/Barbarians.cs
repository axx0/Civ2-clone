using System;

namespace Civ2engine;

public class Barbarians
{
    public static Civilization Civilization =>
        new()
        {
            Adjective = Labels.For(LabelIndex.Barbarian), 
            LeaderName = Labels.For(LabelIndex.Attila),
            TribeName = Labels.For(LabelIndex.Barbarians),
            Alive = true, Id = 0, TribeId = -1,
            PlayerType = PlayerType.Barbarians, 
            Advances = []
        };
}