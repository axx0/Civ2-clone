using System;
using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.Terrains;

namespace Civ2engine.Units
{
    public class Unit : BaseInstance, IUnit
    {
        private Tile _currentLocation;
        private bool _dead;

        // From RULES.TXT
        public string Name => TypeDefinition.Name;

        public bool Dead
        {
            get => _dead;
            set
            {
                if (value)
                {
                    CurrentLocation = null;
                }
                _dead = value;
            }
        }

        public int UntilTech => TypeDefinition.Until;
        public UnitGAS Domain => TypeDefinition.Domain;
        public int MaxMovePoints => TypeDefinition.Move;
        public int FuelRange => TypeDefinition.Range;
        public int AttackBase => TypeDefinition.Attack;
        public int DefenseBase => TypeDefinition.Defense;
        
        public UnitDefinition TypeDefinition { get; set; }

        public int AttackFactor(IUnit defendingUnit)
        {
            // Base attack factor from RULES
            double AF = (double)AttackBase;

            // Bonus for veteran units
            if (Veteran) AF *= 1.5;

            // Partisan bonus agains non-combat units
            if (Type == UnitType.Partisans && defendingUnit.AttackBase == 0) AF *= 8;

            return (int)AF;
        }

        public int DefenseFactor(IUnit attackingUnit, City cityHere)
        {
            //Carried units cannot be the defender
            if (InShip != null) return 0;
            
            // Base defense factor from RULES
            double DF = (double)DefenseBase;

            // Bonus for veteran units
            if (Veteran) DF *= 1.5;

            // City walls bonus (applies only to land units)
            if (cityHere != null && cityHere.ImprovementExists(ImprovementType.CityWalls) && Domain == UnitGAS.Ground && !attackingUnit.NegatesCityWalls) DF *= 3;
            // Fortress bonus (Applies only to land units. Unit doesn't have to be fortified. Doesn't count if air unit is attacking.)
            else if (Map.TileC2(X, Y).Fortress && Domain == UnitGAS.Ground && attackingUnit.Domain != UnitGAS.Air) DF *= 2;
            // Fortified bonus
            else if (Order == OrderType.Fortified && Domain == UnitGAS.Ground) DF *= 1.5;

            // Helicopters are vulnerable to anti air
            if (Domain == UnitGAS.Air && FuelRange == 0 && attackingUnit.CanAttackAirUnits)
            {
                DF /= 2;
            }

            if (cityHere != null)
            {
                if (Domain == UnitGAS.Air && FuelRange == 1 && attackingUnit.Domain == UnitGAS.Air)
                {
                    // TODO: Message box about fighters scrambling for defence
                    if (attackingUnit.FuelRange != 1)
                    {
                        DF *= 4;
                    }
                    else
                    {
                        DF *= 2;
                    }
                }
                else
                {
                    // Effect of SAM batteries (only when attacked from air)
                    if (cityHere.ImprovementExists(ImprovementType.SAMbattery) &&
                        attackingUnit.Domain == UnitGAS.Air) DF *= 2;
                }

                // Effect of SDI
                if (cityHere.ImprovementExists(ImprovementType.SDIdefense) &&
                    attackingUnit.Type == UnitType.CruiseMsl) DF *= 2;

                // Effect of coastal fortress
                if (cityHere.ImprovementExists(ImprovementType.CoastalFort) &&
                    attackingUnit.Domain == UnitGAS.Sea) DF *= 2;
            }

            // Effect of terrain
            DF *= Map.TileC2(X, Y).Defense;

            return (int)DF;
        }

        public int FirepowerBase => TypeDefinition.Firepwr;

        public int Cost => TypeDefinition.Cost;
        public int ShipHold => TypeDefinition.Hold;
        public AIroleType AIrole => (AIroleType)TypeDefinition.AIrole;
        public bool TwoSpaceVisibility => TypeDefinition.Flags[14] == '1';
        public bool IgnoreZonesOfControl => TypeDefinition.Flags[13] == '1';
        public bool CanMakeAmphibiousAssaults => TypeDefinition.Flags[12] == '1';
        public bool SubmarineAdvantagesDisadvantages => TypeDefinition.Flags[11] == '1';
        public bool CanAttackAirUnits => TypeDefinition.Flags[10] == '1';    // fighter
        public bool ShipMustStayNearLand => TypeDefinition.Flags[9] == '1';  // trireme
        public bool NegatesCityWalls => TypeDefinition.Flags[8] == '1';  // howitzer
        public bool CanCarryAirUnits => TypeDefinition.Flags[7] == '1';  // carrier
        public bool CanMakeParadrops => TypeDefinition.Flags[6] == '1';
        public bool Alpine => TypeDefinition.Flags[5] == '1';    // treats all squares as road
        public bool X2onDefenseVersusHorse => TypeDefinition.Flags[4] == '1';    // pikemen
        public bool FreeSupportForFundamentalism => TypeDefinition.Flags[3] == '1';    // fanatics
        public bool DestroyedAfterAttacking => TypeDefinition.Flags[2] == '1';    // missiles
        public bool X2onDefenseVersusAir => TypeDefinition.Flags[1] == '1';    // AEGIS
        public bool UnitCanSpotSubmarines => TypeDefinition.Flags[0] == '1';


        public int Id { get; set; }

        public int MovePoints => MaxMovePoints - MovePointsLost;

        public int MovePointsLost { get; set; }
        public int HitpointsBase => TypeDefinition.Hitp;
        public int HitPoints => HitpointsBase - HitPointsLost;
        public int HitPointsLost { get; set; }
        public UnitType Type { get; set; }
        public OrderType Order { get; set; }
        public bool FirstMove { get; set; }
        public bool GreyStarShield { get; set; }
        public bool Veteran { get; set; }
        public Civilization Owner { get; set; }
        public CommodityType CaravanCommodity { get; set; }
        public City HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }
        public int X { get; set; }
        public int Xreal => (X - Y % 2) / 2;
        public int Y { get; set; }
        public int[] XY => new int[] { X, Y };
        
        public int MovementCounter { get; set; }

        public int[] PrevXY { get; set; }   // XY position of unit before it moved
        public int[] PrevXYpx => new int[] { PrevXY[0] * Map.Xpx, PrevXY[1] * Map.Ypx };


        public bool TurnEnded => MovePoints <= 0 ||
                                 Order is OrderType.Fortified or OrderType.Transform or OrderType.Fortify or
                                     OrderType.BuildIrrigation or OrderType.BuildRoad or OrderType.BuildAirbase or
                                     OrderType
                                         .BuildFortress or OrderType.BuildMine;
    

        public bool AwaitingOrders => !TurnEnded && (Order is OrderType.NoOrders or OrderType.GoTo);

        public void SkipTurn()
        {
            MovePointsLost = MaxMovePoints;
            PrevXY = new[] { X, Y };
        }

        public void Fortify()
        {
            Order = OrderType.Fortify;
        }

        public void BuildIrrigation()
        {
            if (TypeDefinition.IsSettler && Map.Tile[X, Y].CanBeIrrigated)
            {
                Order = OrderType.BuildIrrigation;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
        }

        public void BuildMines()
        {
            if (TypeDefinition.IsSettler && Map.Tile[X,Y].CanBeMined)
            {
                Order = OrderType.BuildMine;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
        }

        public void Transform()
        {
            if (TypeDefinition.IsEngineer)
            {
                Order = OrderType.Transform;
            }
        }

        public void Sleep()
        {
            Order = OrderType.Sleep;
        }

        public void BuildRoad()
        {
            if (TypeDefinition.IsSettler && ((Map.Tile[X, Y].Road == false) || (Map.Tile[X, Y].Railroad == false)))
            {
                Order = OrderType.BuildRoad;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
        }

        public void BuildCity()
        {
            if (TypeDefinition.IsSettler && (Map.Tile[X, Y].Type != TerrainType.Ocean))
            {
                //First invoke city name panel. If cancel is pressed, do nothing.
                //Application.OpenForms.OfType<MapForm>().First().ShowCityNamePanel();
            }
            else
            {
                //Warning!
            }
        }

        public bool IsInCity => CurrentLocation is {CityHere: { }};
        public bool IsInStack => CurrentLocation != null && CurrentLocation.UnitsHere.Count > 1;
        public bool IsLastInStack => CurrentLocation != null && CurrentLocation.UnitsHere.Last() == this;
        
        public Unit InShip { get; set; }

        public string AttackSound => TypeDefinition.AttackSound;
        public City CityWithThisUnit => CurrentLocation != null ? CurrentLocation.CityHere: null;
        public List<Unit> CarriedUnits { get; } = new();

        public Tile CurrentLocation
        {
            get => _currentLocation;
            set
            {
                if(_currentLocation == value) return;
                _currentLocation?.UnitsHere.RemoveAll(u=> u== this);
                if (value != null && !value.UnitsHere.Contains(this))
                {
                    value.UnitsHere.Add(this);
                }
                _currentLocation = value;
            }
        }
    }
}
