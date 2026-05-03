using Civ2engine.IO;
using Civ2engine.SaveLoad.SavFile;
using Model.Core;
using Model.Core.GameRules;

namespace Core.Tests.Mocks;

internal class MockSavFile : SavFileBase
{
    public override IGame LoadGame(byte[] fileData, Ruleset activeRuleSet, Rules rules)
    {
        return new MockGame();
    }
}
