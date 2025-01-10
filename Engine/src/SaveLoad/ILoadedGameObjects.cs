using System.Collections.Generic;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model;

namespace Civ2engine.OriginalSaves;

public interface ILoadedGameObjects
{
    Unit ActiveUnit { get; }
    Scenario Scenario { get; }
    List<City> Cities { get; set; }
    List<Transporter> Transporters { get; set; }
    List<Civilization> Civilizations { get; set; }
    List<Map> Maps { get; set; }
}