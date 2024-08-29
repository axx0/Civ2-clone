using System.Collections.Generic;
using System.Linq;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Production;
using Civ2engine.Terrains;

namespace Civ2engine.Units
{
    public class Unit: IUnit
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
                    _currentLocation?.UnitsHere.Remove(this);
                }
                _dead = value;
            }
        }

        public int UntilTech => TypeDefinition.Until;
        public UnitGas Domain => TypeDefinition.Domain;
        public int MaxMovePoints => TypeDefinition.Move;
        public int FuelRange => TypeDefinition.Range;
        public int AttackBase => TypeDefinition.Attack;
        public int DefenseBase => TypeDefinition.Defense;
        
        public UnitDefinition TypeDefinition { get; set; }

        public double AttackFactor(Unit defendingUnit)
        {
            // Base attack factor from RULES
            double af = AttackBase;

            // Bonus for veteran units
            if (Veteran) af *= 1.5;

            // Partisan bonus agains non-combat units
            if (Type == UnitType.Partisans && defendingUnit.AttackBase == 0) af *= 8;

            return af;
        }

        public int DefenseFactor(Unit attackingUnit, Tile tile, int groundDefMultiplier)
        {
            //Carried units cannot be the defender
            if (InShip != null) return 0;
            
            // Base defense factor from RULES
            decimal df = DefenseBase;

            // Bonus for veteran units
            if (Veteran) df *= 1.5m;

            // City walls bonus (applies only to land units)
            if (tile.CityHere != null && tile.CityHere.ImprovementExists(ImprovementType.CityWalls) && Domain == UnitGas.Ground && !attackingUnit.NegatesCityWalls) df *= 3;
            // Fortress bonus (Applies only to land units. Unit doesn't have to be fortified. Doesn't count if air unit is attacking.)
            else if (groundDefMultiplier != 0 && Domain == UnitGas.Ground && attackingUnit.Domain != UnitGas.Air) df += df * groundDefMultiplier / 100;
            // Fortified bonus
            else if (Order == OrderType.Fortified && Domain == UnitGas.Ground) df *= 1.5m;

            // Helicopters are vulnerable to anti air
            if (Domain == UnitGas.Air && FuelRange == 0 && attackingUnit.CanAttackAirUnits)
            {
                df /= 2;
            }

            if (tile.CityHere != null)
            {
                if (Domain == UnitGas.Air && FuelRange == 1 && attackingUnit.Domain == UnitGas.Air)
                {
                    // TODO: Message box about fighters scrambling for defence
                    if (attackingUnit.FuelRange != 1)
                    {
                        df *= 4;
                    }
                    else
                    {
                        df *= 2;
                    }
                }
                else
                {
                    // Effect of SAM batteries (only when attacked from air)
                    if (tile.CityHere.ImprovementExists(ImprovementType.SaMbattery) &&
                        attackingUnit.Domain == UnitGas.Air) df *= 2;
                }

                // Effect of SDI
                if (tile.CityHere.ImprovementExists(ImprovementType.SdIdefense) &&
                    attackingUnit.Type == UnitType.CruiseMsl) df *= 2;

                // Effect of coastal fortress
                if (tile.CityHere.ImprovementExists(ImprovementType.CoastalFort) &&
                    attackingUnit.Domain == UnitGas.Sea) df *= 2;
            }

            // Effect of terrain
            df *= tile.Defense;

            return (int)df;
        }

        public int FirepowerBase => TypeDefinition.Firepwr;

        public int Cost => TypeDefinition.Cost;
        public int ShipHold => TypeDefinition.Hold;
        public AIroleType AIrole => (AIroleType)TypeDefinition.AIrole;
        public bool TwoSpaceVisibility => TypeDefinition.Flags[14] == '1';
        public bool IgnoreZonesOfControl => TypeDefinition.Flags[13] == '1' || Domain == UnitGas.Air || Domain == UnitGas.Sea;
        public bool CanMakeAmphibiousAssaults => TypeDefinition.Flags[12] == '1';
        public bool SubmarineAdvantagesDisadvantages => TypeDefinition.Flags[11] == '1';
        public bool CanAttackAirUnits => TypeDefinition.Flags[10] == '1';    // fighter
        public bool ShipMustStayNearLand => TypeDefinition.Flags[9] == '1';  // trireme
        public bool NegatesCityWalls => TypeDefinition.Flags[8] == '1';  // howitzer
        public bool CanCarryAirUnits => TypeDefinition.Flags[7] == '1';  // carrier
        public bool CanMakeParadrops => TypeDefinition.Flags[6] == '1';
        public bool Alpine => TypeDefinition.Flags[5] == '1';    // treats all squares as road
        public bool X2OnDefenseVersusHorse => TypeDefinition.Flags[4] == '1';    // pikemen
        public bool FreeSupportForFundamentalism => TypeDefinition.Flags[3] == '1';    // fanatics
        public bool DestroyedAfterAttacking => TypeDefinition.Flags[2] == '1';    // missiles
        public bool X2OnDefenseVersusAir => TypeDefinition.Flags[1] == '1';    // AEGIS
        public bool UnitCanSpotSubmarines => TypeDefinition.Flags[0] == '1';


        public int Id { get; set; }

        public int MovePoints => MaxMovePoints - MovePointsLost;

        public int MovePointsLost { get; set; }
        public int HitpointsBase => TypeDefinition.Hitp;
        public int RemainingHitpoints => HitpointsBase - HitPointsLost;
        public int HitPointsLost { get; set; }

        public UnitType Type => TypeDefinition.Type;

        public OrderType Order { get; set; }
        public bool MadeFirstMove { get; set; }
        public bool Veteran { get; set; }
        public Civilization Owner { get; set; }
        public CommodityType CaravanCommodity { get; set; }
        public City? HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }
        public int X { get; set; }
        public int Xreal => (X - Y % 2) / 2;
        public int Y { get; set; }
        public int[] Xy => new int[] { X, Y };
        public int MapIndex { get; set; }
        
        public int MovementCounter { get; set; }

        public int[] PrevXy { get; set; }   // XY position of unit before it moved
        public int[] PrevXYpx => new int[] { PrevXy[0] * CurrentLocation.Map.Xpx, PrevXy[1] * CurrentLocation.Map.Ypx };


        public bool TurnEnded => MovePoints <= 0 ||
                                 Order is OrderType.Fortified or OrderType.Transform or OrderType.Fortify or
                                     OrderType.BuildIrrigation or OrderType.BuildRoad or OrderType.BuildAirbase or
                                     OrderType
                                         .BuildFortress or OrderType.BuildMine;
    

        public bool AwaitingOrders => !TurnEnded && !Dead && (Order is OrderType.NoOrders);

        public void SkipTurn()
        {
            MovePointsLost = MaxMovePoints;
            PrevXy = new[] { X, Y };
        }

        public void Fortify()
        {
            Order = OrderType.Fortify;
        }

        public void Sleep()
        {
            Order = OrderType.Sleep;
        }

        public bool IsInCity => CurrentLocation is {CityHere: { }};
        public bool IsInStack => CurrentLocation != null && CurrentLocation.UnitsHere.Count > 1;
        public bool IsLastInStack => CurrentLocation != null && CurrentLocation.UnitsHere.Last() == this;
        
        public Unit? InShip { get; set; }

        public string AttackSound => TypeDefinition.AttackSound;
        public City CityWithThisUnit => CurrentLocation != null ? CurrentLocation.CityHere: null;
        public List<Unit> CarriedUnits { get; } = new();

        public Tile? CurrentLocation
        {
            get => _currentLocation;
            set
            {
                if(_dead) return; //dead units can't move
                if(_currentLocation == value) return;
                _currentLocation?.UnitsHere.RemoveAll(u=> u== this);
                if (value != null && !value.UnitsHere.Contains(this))
                {
                    value.UnitsHere.Add(this);
                }
                _currentLocation = value;
            }
        }

        public bool FreeSupport(bool isFundamental)
        {
            return AIrole is AIroleType.Diplomacy or AIroleType.Trade || (isFundamental && FreeSupportForFundamentalism);
        }

        public bool NeedsSupport { get; set; } = true;

        public void ProcessOrder()
        {
            Counter += TypeDefinition.WorkRate;
            MovePointsLost = MovePoints;
        }

        public void Build(TerrainImprovement improvement)
        {
            Building = improvement.Id;
            ProcessOrder();
            // This is a cludge but it will work for now
            Order = (OrderType)improvement.Id;
        }

        public int Building { get; set; }
    }
}
