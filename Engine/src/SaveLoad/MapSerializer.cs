using System.Text.Json;
using Civ2engine.MapObjects;

namespace Civ2engine.SaveLoad;

public class MapSerializer
{
    public void WriteMapObjects(Map[] maps, Utf8JsonWriter writer)
    {
        writer.WriteStartArray("map");
        foreach (var map in maps)
        {
            writer.WriteStartObject();
            writer.WriteBoolean("flat", map.Flat);
            writer.WriteNumber("x", map.XDimMax);
            writer.WriteEndObject();
        }
    }
}