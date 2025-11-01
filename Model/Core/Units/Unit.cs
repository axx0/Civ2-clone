using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using Model.Constants;

namespace Model.Core.Units
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

        public int FirepowerBase => TypeDefinition.Firepwr;

        public int Cost => TypeDefinition.Cost;
        public int ShipHold => TypeDefinition.Hold;
        public AiRoleType AiRole => TypeDefinition.AIrole;
        public bool TwoSpaceVisibility => TypeDefinition.Flags[0] ;
        public bool IgnoreZonesOfControl => TypeDefinition.Flags[1]  || Domain == UnitGas.Air || Domain == UnitGas.Sea;
        public bool CanMakeAmphibiousAssaults => TypeDefinition.Flags[2] ;
        public bool SubmarineAdvantagesDisadvantages => TypeDefinition.Flags[3] ;
        public bool CanAttackAirUnits => TypeDefinition.Flags[4] ;    // fighter
        public bool ShipMustStayNearLand => TypeDefinition.Flags[5] ;  // trireme
        public bool NegatesCityWalls => TypeDefinition.Flags[6] ;  // howitzer
        public bool CanCarryAirUnits => TypeDefinition.Flags[7] ;  // carrier
        public bool CanMakeParadrops => TypeDefinition.Flags[8];
        public bool Alpine => TypeDefinition.Flags[9] ;    // treats all squares as road
        public bool X2OnDefenseVersusHorse => TypeDefinition.Flags[10] ;    // pikemen
        public bool FreeSupportForFundamentalism => TypeDefinition.Flags[11] ;    // fanatics
        public bool DestroyedAfterAttacking => TypeDefinition.Flags[12] ;    // missiles
        public bool X2OnDefenseVersusAir => TypeDefinition.Flags[13] ;    // AEGIS
        public bool UnitCanSpotSubmarines => TypeDefinition.Flags[4] ;


        public int Id { get; set; }

        public int MovePoints => MaxMovePoints - MovePointsLost;

        public int MovePointsLost { get; set; }
        public int HitpointsBase => TypeDefinition.Hitp;
        public int RemainingHitpoints => HitpointsBase - HitPointsLost;
        public int HitPointsLost { get; set; }

        public int Type => TypeDefinition.Type;

        public int Order { get; set; }
        public bool MadeFirstMove { get; set; }
        public bool Veteran { get; set; }
        public Civilization Owner { get; set; }
        public int CaravanCommodity { get; set; }
        public City? HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MapIndex { get; set; }

        public int[] PrevXy { get; set; }   // XY position of unit before it moved

        public bool TurnEnded => MovePoints <= 0 ||
                                 Order is (int)OrderType.Fortified or (int)OrderType.Transform or (int)OrderType.Fortify or
                                     (int)OrderType.BuildIrrigation or (int)OrderType.BuildRoad or (int)OrderType.BuildAirbase or
                                     (int)OrderType
                                         .BuildFortress or (int)OrderType.BuildMine;
    

        public bool AwaitingOrders => !TurnEnded && !Dead && (Order is (int)OrderType.NoOrders);

        public void SkipTurn()
        {
            MovePointsLost = MaxMovePoints;
            PrevXy = [X, Y];
        }

        public void Sleep()
        {
            Order = (int)OrderType.Sleep;
        }

        public bool IsInStack => CurrentLocation is { UnitsHere.Count: > 1 };

        public Unit? InShip { get; set; }

        public string AttackSound => TypeDefinition.AttackSound;
        public List<Unit> CarriedUnits { get; } = new();

        public Tile CurrentLocation
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

        public bool FreeSupport(int[] typesWithFreeSupport)
        {
            return AiRole is AiRoleType.Diplomacy or AiRoleType.Trade || (typesWithFreeSupport.Contains(Type));
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
            Order = improvement.Id;
        }

        public int Building { get; set; }
        public int AttacksSpent { get; set; }

        public Dictionary<string, string> ExtendedData { get;} = new();
    }
}
