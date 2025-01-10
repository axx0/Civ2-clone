using System.Collections.Generic;
using Civ2engine.MapObjects;
using Civ2engine.OriginalSaves;
using Civ2engine.Units;
using Model;

namespace Civ2engine.SaveLoad;

public class JsonSaveObjects : ILoadedGameObjects
{
    public Unit ActiveUnit { get; set; }
    public Scenario Scenario { get; }
    public List<City> Cities { get; set; }
    
    public List<Transporter> Transporters { get; set; }
    public List<Civilization> Civilizations { get; set; }
    public List<Map> Maps { get; set; }
}