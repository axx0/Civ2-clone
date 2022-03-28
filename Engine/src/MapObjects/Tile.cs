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

        public int GetFood(bool lowOrganisation, bool hasSupermarket)
        {
            decimal food = EffectiveTerrain.Food;
            var hasCity = CityHere != null;
            if (Irrigation || hasCity)
            {
                food += Terrain.IrrigationBonus;
            }

            if (hasCity && food < 2)
            {
                food += 1;
            }

            if (hasSupermarket && (Farmland || hasCity))
            {
                food *= 1.5m;
            }

            if (lowOrganisation && food >= 3)
            {
                food -= 1;
            }

            return (int)food;
        }

        public int GetTrade(int organizationLevel, bool hasSuperhighways)
        {
            decimal trade = EffectiveTerrain.Trade;

            var hasRoad = Road || Railroad || CityHere != null;
            if (hasRoad || River)
            {
                trade += Terrain.RoadBonus;
            }

            if (organizationLevel > 1 && trade > 0)
            {
                trade += 1;
            }

            if (hasSuperhighways && hasRoad)
            {
                trade *= 1.5m;
            }

            if (organizationLevel == 0 && trade >= 3)
            {
                trade -= 1;
            }

            return (int)trade;
        }

        public int GetShields(bool lowOrganization)
        {
            if (Type == TerrainType.Grassland && !HasShield)
            {
                return 0;
            }

            decimal shields = EffectiveTerrain.Shields;

            if (Mining)
            {
                shields += Terrain.MiningBonus;
            }

            if (Railroad)
            {
                shields *= 1.5m;
            }

            if (lowOrganization && shields >= 3)
            {
                shields -= 1;
            }

            return (int)shields;
        }

        public bool CanBeIrrigated =>
            Terrain.CanIrrigate != -2 &&
            (!Irrigation || !Farmland); // yes meaning the result can be irrigation or transform. of terrain

        /// <summary>
        /// If result == type of terrain before irrigation, this means that it's regular irrigation.
        /// (If it can actually be irrigated is determined by CanBeIrrigated.)
        /// Otherwise the result is the type of terrain which is formed.
        /// </summary>
        public TerrainType IrrigationResult => Terrain.CanIrrigate < 0 ? Type : (TerrainType)Terrain.CanIrrigate;

        public GovernmentType MinGovrnLevelAItoPerformIrrigation { get; set; } // Be careful, 0=never!

        public bool CanBeMined =>
            Terrain.CanMine != -2 && (!Mining); // yes meaning the result can be mining or transform. of terrain

        /// <summary>
        /// If result == type of terrain before mining, this means that it's regular mine.
        /// (If it can actually be mined is determined by CanBeMined.)
        /// Otherwise the result is the type of terrain which is formed.
        /// </summary>
        public TerrainType MiningResult => Terrain.CanMine < 0 ? Type : (TerrainType)Terrain.CanMine;

        public GovernmentType MinGovrnLevelAItoPerformMining { get; set; } // Be careful, 0=never!
        public bool CanBeTransformed => Terrain.Transform != -2; // usually only ocean can't be transformed

        /// <summary>
        ///  If result == type of terrain before transformation, it means it can't be transformed.
        /// Otherwise the result is the type of terrain which is transformed.
        /// </summary>
        public TerrainType TransformResult => Terrain.Transform < 0 ? Type : (TerrainType)Terrain.Transform;


        // TODO: put special resources logic into here

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

        public Unit GetTopUnit(Func<Unit, bool> pred = null)
        {
            var units = pred != null ? UnitsHere.Where(pred) : UnitsHere;
            return (Terrain.Type == TerrainType.Ocean
                    ? units.OrderByDescending(u => u.Domain == UnitGAS.Sea ? 1 : 0)
                    : units.OrderByDescending(u => u.Domain == UnitGAS.Sea ? 0 : 1))
                .ThenBy(u => u.AttackBase).First();
        }

        public void CompleteConstruction(OrderType order, Rules rules)
        {
            switch (order)
            {
                case OrderType.BuildFortress:
                    Fortress = true;
                    break;
                case OrderType.BuildRoad: 
                    if (Road)
                    {
                        Railroad = true;
                    }
                    else
                    {
                        Road = true;
                    }
                    break;
                case OrderType.BuildIrrigation:
                    if (Terrain.CanIrrigate >= 0)
                    {
                        Terrain = rules.Terrains[0][Terrain.CanIrrigate];
                        ClearImprovements();
                    }
                    else
                    {
                        if (Irrigation)
                        {
                            Farmland = true;
                        }
                        else
                        {
                            Irrigation = true;
                        }
                    }

                    break;
                case OrderType.BuildMine:
                    if (Terrain.CanMine >= 0)
                    {
                        Terrain = rules.Terrains[0][Terrain.CanMine];
                        ClearImprovements();
                    }
                    else
                    {
                        Mining = true;
                    }
                    break;
                case OrderType.Transform:
                    Terrain = rules.Terrains[0][Terrain.Transform];   
                    ClearImprovements();  
                    break;
                case OrderType.CleanPollution:
                    Pollution = false;
                    break;
                case OrderType.BuildAirbase:
                    Airbase = true;
                    break;
                case OrderType.BuildTransport1:
                    break;
                case OrderType.BuildTransport2:
                    break;
                case OrderType.BuildTransport3:
                    break;
            }
        }

        private void ClearImprovements()
        {
            Irrigation = false;
            Mining = false;
            Farmland = false;
        }
    }
}