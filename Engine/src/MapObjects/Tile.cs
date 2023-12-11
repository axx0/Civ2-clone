using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Civ2engine.Advances;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine.MapObjects
{
    public class Tile : IMapItem
    {
        private City? _workedBy;

        public int Owner { get; set; } = -1;
        
        private Terrain _terrain;
        
        private bool[] _visibility;
        public int X { get; }
        public int Y { get; }
        
        public int Z { get;}

        public int Odd { get; }

        public Terrain Terrain
        {
            get => _terrain;
            internal set
            {
                if (_terrain == value) return;
                _terrain = value;
                SetEffectiveTerrain();
            }
        }

        public Map Map { get; }
        public int XIndex { get; }

        private void SetEffectiveTerrain()
        {
            if (Special != -1 && Special < _terrain.Specials.Length)
            {
                EffectiveTerrain = _terrain.Specials[Special];
            }
            else
            {
                EffectiveTerrain = _terrain;
            }
        }

        internal ITerrain EffectiveTerrain { get; private set; }

        public TerrainType Type => Terrain.Type;

        public int Special { get; } = -1;


        // Get special resource type based on map seed & tile location
        public Tile(int x, int y, Terrain terrain, int seed, Map map, int xIndex, bool[] visibility)
        {
            _visibility = visibility;
            // Courtesy of Civfanatics
            // https://forums.civfanatics.com/threads/is-there-really-no-way-to-do-this-add-resources-on-map.518649/#post-13002282
            X = x;
            Y = y;
            Z = map.MapIndex;
            Odd = y % 2;
            Terrain = terrain;
            Map = map;
            XIndex = xIndex;

            HasShield = HasSheild();

            var a = (X + Y) >> 1;
            var b = X - a;
            var c = 13 * (b >> 2) + 11 * ((X + Y) >> 3) + seed;
            if ((a & 3) + 4 * (b & 3) != (c & 15)) return;

            var d = 1 << ((seed >> 4) & 3);
            Special = (d & a) == (d & b) ? 1 : 0;
            SetEffectiveTerrain();
        }

        public bool HasShield { get; }

        private bool HasSheild()
        {
            // Formula for determining where grassland shield is based on X-Y coords
            int rez4 = (Y / 2 + 2 * (Y % 2)) % 4;
            int rez3 = 8 - 2 * (rez4 % 4);

            int rez = (X - (Y % 2) + rez3) % 8;
            return rez < 4;
        }

        // From RULES.TXT
        public string Name => EffectiveTerrain.Name;

        public int MoveCost => EffectiveTerrain.MoveCost;
        public int Defense => (River ? EffectiveTerrain.Defense + 1 : EffectiveTerrain.Defense) / 2;

        public int GetFood(bool lowOrganisation)
        {
            decimal food = EffectiveTerrain.Food;

            var foodEffects = EffectsList.Where(e => e.Target == ImprovementConstants.Food).ToList();

            food += foodEffects.Where(e => e.Action == ImprovementActions.Add).Sum(e => e.Value);

            var cityImprovements = WorkedBy?.Improvements.SelectMany(ci =>
                ci.TerrainEffects?.Where(ef =>ef.Resource == ImprovementConstants.Food &&
                    (!ef.Terrain.HasValue || ef.Terrain.Value == (int)_terrain.Type ) && 
                    (!ef.Improvement.HasValue 
                     || Improvements.Any(i => i.Improvement == ef.Improvement && i.Level >= ef.Level))
                    ) ?? Array.Empty<CityTerrainEffect>())?? Array.Empty<CityTerrainEffect>();
            foreach (var cityTerrainEffect in cityImprovements.OrderBy(i=>i.Action))
            {
                switch (cityTerrainEffect.Action)
                {
                    case CityTerrainEffect.Add:
                        food += cityTerrainEffect.Value;
                        break;
                    case CityTerrainEffect.AddExtra:
                        if (food > 0)
                        {
                            food += cityTerrainEffect.Value;
                        }
                        break;
                    case CityTerrainEffect.Multiply:
                        food *= cityTerrainEffect.Value / 100m;
                        break;
                }
            }
            
            if (CityHere != null && food < 2)
            {
                food += 1;
            }
            if (food > 0)
            {
                var multiplier = foodEffects.Where(e => e.Action == ImprovementActions.Multiply)
                    .Sum(e => e.Value);
                if (multiplier != 0)
                {
                    food += food * (multiplier / 100m);
                }
            }

            if (lowOrganisation && food >= 3)
            {
                food -= 1;
            }

            return (int)food;
        }

        public int GetTrade(int organizationLevel)
        {
            decimal trade = EffectiveTerrain.Trade;

            if (River)
            {
                trade += Terrain.RoadBonus;
            }

            var tradeEffects = EffectsList.Where(e => e.Target == ImprovementConstants.Trade).ToList();

            trade += tradeEffects.Where(e => e.Action == ImprovementActions.Add).Sum(e => e.Value);
            if (organizationLevel > 1 && trade > 0)
            {
                trade += 1;
            }

            if (trade > 0)
            {
                var multiplier = tradeEffects.Where(e => e.Action == ImprovementActions.Multiply)
                    .Sum(e => e.Value);
                if (multiplier != 0)
                {
                    trade += trade * (multiplier / 100m);
                }
            }

            if (organizationLevel == 0 && trade >= 3)
            {
                trade -= 1;
            }

            return (int)trade;
        }

        public int GetShields(bool lowOrganization)
        {
            decimal shields = Type == TerrainType.Grassland && !HasShield ? 0 :  EffectiveTerrain.Shields;

            var productionEffects = EffectsList.Where(e => e.Target == ImprovementConstants.Shields).ToList();

            if (productionEffects.Count > 0)
            {
                shields += productionEffects.Where(e => e.Action == ImprovementActions.Add).Sum(e => e.Value);
                if (shields > 0)
                {
                    var multiplier = productionEffects.Where(e => e.Action == ImprovementActions.Multiply)
                        .Sum(e => e.Value);
                    if (multiplier != 0)
                    {
                        shields += shields * (multiplier / 100m);
                    }
                }
            }

            if (lowOrganization && shields >= 3)
            {
                shields -= 1;
            }

            return (int)shields;
        }

        public bool Resource { get; set; }
        public bool River { get; set; }
        public bool IsUnitPresent => UnitsHere.Count > 0;
        public bool IsCityPresent => CityHere != null;
        public int Island { get; set; }
        public decimal Fertility { get; set; } = -1;
        
        public void SetVisible(int civId, bool visible = true)
        {
            if (_visibility.Length <= civId)
            {
                _visibility = _visibility.Concat(Enumerable.Repeat(false, civId - _visibility.Length +1)).ToArray();
            }

            _visibility[civId] = visible;
        }

        public List<Unit> UnitsHere { get; } = new();

        public City? CityHere { get; set; }

        public City? WorkedBy
        {
            get => _workedBy;
            set
            {
                if (value != _workedBy)
                {
                    _workedBy?.WorkedTiles.Remove(this);
                    _workedBy = CityHere ??
                                value; //If there is a city here then that's to only city that can work this tile 
                }

                if (_workedBy?.WorkedTiles.Contains(this) == false)
                {
                    _workedBy.WorkedTiles.Add(this);
                }

                if (_workedBy != null)
                {
                    Owner = _workedBy.OwnerId;
                }
            }
        }

        public List<ConstructedImprovement> Improvements { get; init; } = new();

        public List<ActiveEffect> EffectsList { get; } = new();

        public Unit GetTopUnit(Func<Unit, bool> pred = null)
        {
            var units = pred != null ? UnitsHere.Where(pred) : UnitsHere;
            return (Terrain.Type == TerrainType.Ocean
                    ? units.OrderByDescending(u => u.Domain == UnitGAS.Sea ? 1 : 0)
                    : units.OrderByDescending(u => u.Domain == UnitGAS.Sea ? 0 : 1))
                .ThenBy(u => u.AttackBase).First();
        }

        public bool HasAirbase()
        {
            return EffectsList.Any(e => e.Target == ImprovementConstants.Airbase);
        }

        public bool IsVisible(int civId)
        {
            return civId < _visibility.Length && _visibility[civId];
        }
        
        private PlayerTile[]? PlayerKnowledge { get; set; }

        public void UpdatePlayer(int civilizationId)
        {
            if (PlayerKnowledge == null || PlayerKnowledge.Length <= civilizationId)
            {
                var know = new PlayerTile[civilizationId+1];
                if (PlayerKnowledge != null)
                {
                    for (int i = 0; i < PlayerKnowledge.Length; i++)
                    {
                        know[i] = PlayerKnowledge[i];
                    }
                }
                PlayerKnowledge = know;
            }

            PlayerKnowledge[civilizationId] = new PlayerTile(this);
        }

        /// <summary>
        /// Ensure player can see everything visible to them at game start or scenario start
        ///  This shouln't be called later (need to figure out how to exclude from loaded games)
        /// </summary>
        public void UpdateAllPlayers()
        {
            PlayerKnowledge = new PlayerTile[_visibility.Length];
            for (var i = 0; i < _visibility.Length; i++)
            {
                if(_visibility[i])
                {
                    PlayerKnowledge[i] = new PlayerTile(this);
                }
                else
                {
                    PlayerKnowledge[i] = new PlayerTile();
                }
            }
        }
    }
}