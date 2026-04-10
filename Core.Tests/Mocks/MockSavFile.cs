using Civ2engine;
using Civ2engine.IO;
using Civ2engine.SaveLoad;
using Civ2engine.SaveLoad.SavFile;
using Model.Core;

namespace Core.Tests.Mocks;

internal class MockSavFile : SavFileBase
{
    public override IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules)
    {
        return new MockGame();
    }
}
