using System.Collections.Generic;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Constants;
using Model;
using Model.Core;
using Model.Core.Units;

namespace Civ2engine.OriginalSaves
{
    public class ClassicSaveObjects : ILoadedGameObjects
    {
        public Unit ActiveUnit { get; set; } = null!;
        public Scenario Scenario { get; set; }
        public List<City> Cities { get; set; }
        public List<Transporter> Transporters { get; set; }
        public List<Civilization> Civilizations { get; set; }
        public List<Map> Maps { get; set; }
        public IGameData GameData { get; set; }
        public Options Options { get; set; }
    }
}