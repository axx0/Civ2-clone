using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Civ2engine.MapObjects;
using Civ2engine.Terrains;
using Model.Core.Mapping;

namespace Civ2engine.SaveLoad;

public class ImprovementEncoder
{
    private readonly Dictionary<int,ImprovementCode> _mapping;

    public ImprovementEncoder(IDictionary<int, TerrainImprovement> allImprovements)
    {
        var mapping = new Dictionary<int, ImprovementCode>();
        int offset = 0;
        foreach (var terrainImprovement in allImprovements)
        {
            var bits = (int)Math.Floor(Math.Log2(terrainImprovement.Value.Levels.Count)) + 1;
            mapping[terrainImprovement.Key] = new ImprovementCode(offset, bits, terrainImprovement.Value.ExclusiveGroup);
            offset += bits;
        }
        _mapping = mapping;
    }

    public ImprovementEncoder(JsonElement improvementMapping)   
    {
        _mapping = new Dictionary<int, ImprovementCode>();
        foreach (var prop in improvementMapping.EnumerateObject())
        {
            if (int.TryParse(prop.Name, out var improvement))
            {
                var bits = prop.Value.GetString().Split("-").Select(int.Parse).ToArray();
                _mapping[improvement] = new ImprovementCode(bits[0], bits[1], bits[2]);
            }
        }
        
    }

    public IDictionary<string, string> EncoderData => _mapping.ToDictionary(kvp => kvp.Key.ToString(), kvp => string.Join("-", kvp.Value.Offset, kvp.Value.Bits, kvp.Value.Group));

    public string? Encode(IList<ConstructedImprovement> improvements)
    {
        byte encoded = 0;
        foreach (var improvement in improvements)
        {
            encoded += (byte)((improvement.Level+1) << _mapping[improvement.Improvement].Offset);
        }

        if (encoded != 0)
        {
            //Encode as base64 and dump the padding 
            return Convert.ToBase64String(new[] { encoded }).Replace("=","");
        }
        return null;
    }

    public IList<ConstructedImprovement> Decode(string? improvements)
    {
        if (string.IsNullOrEmpty(improvements)) return new List<ConstructedImprovement>();
        var ext = improvements.Length % 4;
        if (ext != 0)
        {
            improvements += new string('=', 4 - ext);
        }
        var encodedData = Convert.FromBase64String(improvements);
        int currentByte = encodedData[0];
        var result = new List<ConstructedImprovement>();
        foreach (var mapping in _mapping)
        {
            var code = mapping.Value;
            var level = (currentByte >> code.Offset) & ((int)Math.Pow( code.Bits,2)-1);
            if (level > 0)
            {
                result.Add(new ConstructedImprovement
                {
                    Improvement = mapping.Key, Level = level -1, Group = code.Group
                });
            }
        }
        return result;
    }

    public IList<string> EncodePlayer(PlayerTile?[]? playerKnowledge, bool[] tileVisibility, string? actual,
        City? cityHere)
    {
        var result = new List<string>();
        if (playerKnowledge != null)
        {
            for (int i = 0; i < playerKnowledge.Length; i++)
            {
                var playerTile = playerKnowledge[i];
                if (playerTile == null || i >= tileVisibility.Length || !tileVisibility[i]) continue;
                
                var res = i.ToString();
                var encoded = Encode(playerTile.Improvements);
                if (encoded != null)
                {
                    if (encoded != actual)
                    {
                        res += "-" + encoded;
                    }
                    else
                    {
                        res += "-A";
                    }

                    var playerCityHere = playerTile.CityHere;
                    if (playerCityHere != null)
                    {
                        if (cityHere != null &&
                            cityHere.Name == playerCityHere.Name && cityHere.Size == playerCityHere.Size &&
                            cityHere.OwnerId == playerCityHere.OwnerId)
                        {
                            res += "-A";
                        }
                        else
                        {
                            res += "-" + playerCityHere.Name + "-" + playerCityHere.Size + "-" + playerCityHere.OwnerId;
                        }
                    }
                }

                result.Add(res);
            }
        }

        return result;
    }

    public PlayerTile?[]? DecodePlayer(IList<string>? encodedPlayer, IList<ConstructedImprovement> improvements)
    {
        if(encodedPlayer == null || encodedPlayer.Count == 0) return null;
        var maxPlayer = -1;
        var parts = encodedPlayer.Select(p=>
        {
            if(string.IsNullOrEmpty(p)) return null;
            var parts = p.Split('-');
            int player = int.Parse(parts[0]);
            if(player > maxPlayer) maxPlayer = player;
            PlayerTile tile = new PlayerTile();
            if (parts.Length > 1)
            {
                tile.Improvements.AddRange(parts[1] == "A"
                    ? improvements.Select(i => new ConstructedImprovement
                        { Improvement = i.Improvement, Level = i.Level, Group = i.Group })
                    : Decode(parts[1]));
                if (parts.Length > 2)
                {
                    if (parts[2] == "A")
                    {
                        tile.CityHere = GameSerializer.DummyCityHere;
                    }
                    else
                    {
                        tile.CityHere = new CityInfo{ Name = parts[2] , Size = int.Parse(parts[3]) , OwnerId = int.Parse(parts[4]) };
                    }
                }
            }

            return new { player, tile };
        }).ToArray();
        var result = new PlayerTile[maxPlayer + 1];
        foreach (var parsed in parts)
        {
            if (parsed != null)
            {
                result[parsed.player] = parsed.tile;
            }
        }
        return result;
    }
}

struct ImprovementCode
{

    public ImprovementCode(int offset, int bits, int exclusiveGroup)
    {
        this.Offset = offset;
        this.Bits = bits;
        this.Group = exclusiveGroup;
    }

    public int Group { get; set; }

    public int Offset { get; set; }
    public int Bits { get; set; }
}