using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Terrains;
using Civ2engine.Units;

namespace Civ2engine.MapObjects
{
    public class Tile : IMapItem
    {
        private City _workedBy;
        private Terrain _terrain;
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
        public Tile(int x, int y, Terrain terrain, int seed, Map map)
        {
            // Courtesy of Civfanatics
            // https://forums.civfanatics.com/threads/is-there-really-no-way-to-do-this-add-resources-on-map.518649/#post-13002282
            X = x;
            Y = y;
            Z = map.MapIndex;
            Odd = y % 2;
            Terrain = terrain;
            Map = map;

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
        public bool Irrigation { get; set; }
        public bool Mining { get; set; }
        public bool Road { get; set; }
        public bool Railroad { get; set; }
        public bool Fortress { get; set; }
        public bool Pollution { get; set; }
        public bool Farmland { get; set; }
        public bool Airbase { get; set; }
        public int Island { get; set; }
        public string Hexvalue { get; set; }
        public Bitmap Graphic { get; set; }
        public decimal Fertility { get; set; } = -1;
        public bool[] Visibility { get; set; }

        public List<Unit> UnitsHere { get; } = new();

        public City CityHere { get; set; }

        public City WorkedBy
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

        public void CompleteConstruction(TerrainImprovement improvement, AllowedTerrain terrain, int levelToBuild,
            Terrain[] terrains)
        {
            if (improvement.ExclusiveGroup > 0)
            {
                var previous = Improvements
                    .Where(i => i.Improvement != improvement.Id && i.Group == improvement.ExclusiveGroup).ToList();


                previous.ForEach(i => Improvements.Remove(i));

            }

            var transformEffect = terrain.Effects.FirstOrDefault(e => e.Target == ImprovementConstants.Transform);
            if (transformEffect != null)
            {
                Terrain = terrains[transformEffect.Value];
                return;
            }

            if (levelToBuild > 0)
            {
                var imp = Improvements.FirstOrDefault(i => i.Improvement == improvement.Id);
                if (imp != null)
                {
                    imp.Level = levelToBuild;
                    if (improvement.Levels[levelToBuild].Effects?.Count > 0)
                    {
                        EffectsList.AddRange(improvement.Levels[levelToBuild].Effects.Select(e => new ActiveEffect
                            { Target = e.Target, Action = e.Action, Value = e.Value, Source = improvement.Id }));
                    }

                    return;
                }
            }
            if (improvement.Levels[levelToBuild].Effects?.Count > 0)
            {
                EffectsList.AddRange(improvement.Levels[levelToBuild].Effects.Select(e => new ActiveEffect
                    { Target = e.Target, Action = e.Action, Value = e.Value, Source = improvement.Id }));
            }
            Improvements.Add(new ConstructedImprovement
                { Group = improvement.ExclusiveGroup, Improvement = improvement.Id, Level = levelToBuild });
            EffectsList.AddRange(terrain.Effects.Select(e => new ActiveEffect
                { Target = e.Target, Action = e.Action, Value = e.Value, Source = improvement.Id }));
        }
    }

    public class ActiveEffect
    {
        public int Target { get; set; }
        public int Action { get; set; }
        public int Value { get; set; }
        public int Source { get; set; }
    }
}