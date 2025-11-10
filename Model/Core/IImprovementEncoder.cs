using System.Collections;
using Civ2engine;
using Civ2engine.Terrains;
using Model.Core.Mapping;

namespace Model.Core;

public interface IImprovementEncoder
{
    IDictionary<string, string> EncoderData { get; }
    int[] EncodeKnowledge(PlayerTile?[]? tilePlayerKnowledge, int civs);
    string? Encode(IList<ConstructedImprovement> tileImprovements);
    IList<string> EncodePlayer(PlayerTile?[]? tilePlayerKnowledge, bool[] tileVisibility, string? s, City? tileCityHere);
}