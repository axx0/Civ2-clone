using System;
using System.Linq;
using Civ2engine.Enums;

namespace Civ2engine.Units
{
    public class Unit : BaseInstance, IUnit
    {
        // From RULES.TXT
        public string Name => TypeDefinition.Name;
        public bool Dead { get; set; }
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
        public int Xpx => X * Map.Xpx;
        public int Ypx => Y * Map.Ypx;
        public int[] PrevXY { get; set; }   // XY position of unit before it moved
        public int[] PrevXYpx => new int[] { PrevXY[0] * Map.Xpx, PrevXY[1] * Map.Ypx };

        public bool Move(OrderType movementDirection)
        {
            // Determine coordinates of movement
            int Xto = 0;
            int Yto = 0;
            switch (movementDirection)
            {
                case OrderType.MoveSW:
                    Xto = X - 1;
                    Yto = Y + 1;
                    break;
                case OrderType.MoveS:
                    Xto = X + 0;
                    Yto = Y + 2;
                    break;
                case OrderType.MoveSE:
                    Xto = X + 1;
                    Yto = Y + 1;
                    break;
                case OrderType.MoveE:
                    Xto = X + 2;
                    Yto = Y + 0;
                    break;
                case OrderType.MoveNE:
                    Xto = X + 1;
                    Yto = Y - 1;
                    break;
                case OrderType.MoveN:
                    Xto = X + 0;
                    Yto = Y - 2;
                    break;
                case OrderType.MoveNW:
                    Xto = X - 1;
                    Yto = Y - 1;
                    break;
                case OrderType.MoveW:
                    Xto = X - 2;
                    Yto = Y + 0;
                    break;
            }
            
            // Cannot move beyond map edge
            if (Xto < 0 || Xto >= 2 * Map.XDim || Yto < 0 || Yto >= Map.YDim)
            {
                //TODO: display a message that a unit cannot move beyond map edges
                return false;
            }

            var tileTo = Map.TileC2(Xto, Yto);
            var isCity = tileTo.IsCityPresent;
            bool unitMoved = isCity;
            var moveCost = Game.Rules.Cosmic.MovementMultiplier;
            switch (Domain)
            {
                case UnitGAS.Ground:
                {
                        if (tileTo.Type == TerrainType.Ocean) break;


                        var tileFrom = Map.TileC2(X, Y);
                        if (isCity || tileTo.Railroad)
                        {
                            if (tileFrom.Railroad)
                            {
                                moveCost = Game.Rules.Cosmic.RailroadMovement;
                            }else if (tileFrom.Road)
                            {
                                moveCost = Game.Rules.Cosmic.RoadMovement;
                            }
                        }else if (tileTo.Road && (tileFrom.Road || tileFrom.IsCityPresent || tileFrom.Railroad))
                        {
                            moveCost = Game.Rules.Cosmic.RoadMovement;
                        }
                        else
                        {
                            moveCost *= tileTo.MoveCost;
                        }
                        
                        // If alpine movement could be less use that
                        if (Game.Rules.Cosmic.AlpineMovement < moveCost && Alpine)
                        {
                            moveCost = Game.Rules.Cosmic.AlpineMovement;
                        }
                        
                        if (Game.Rules.Cosmic.RiverMovement < moveCost && tileFrom.River && tileTo.River && (movementDirection is OrderType.MoveSW or OrderType.MoveSE or OrderType.MoveNE or OrderType.MoveNW))    //For rivers only for diagonal movement
                        {
                            moveCost = Game.Rules.Cosmic.RiverMovement;
                        }

                        unitMoved = true;
                        break;
                    }
                case UnitGAS.Sea:
                    {
                        if (tileTo.Type != TerrainType.Ocean) break;
                        unitMoved = true;
                        break;
                    }
                case UnitGAS.Air:
                    {
                        unitMoved = true;
                        break;
                    }
            }

            // If unit moved, update its X-Y coords
            if (unitMoved)
            {
                MovePointsLost += moveCost;
                // Set previous coords
                PrevXY = new int[] { X, Y };

                // Set new coords
                X = Xto;
                Y = Yto;
            }

            return unitMoved;
        }


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

        public bool IsInCity => Game.GetCities.Any(city => city.X == X && city.Y == Y);
        public bool IsInStack => Game.AllUnits.Where(u => u.X == X && u.Y == Y).Count() > 1;
        public bool IsLastInStack => Game.AllUnits.Where(u => u.X == X && u.Y == Y).Last() == this;
        public bool IsInShip
        {
            get
            {
                var ship = Game.UnitsHere(X, Y).Where(u => u.Domain == UnitGAS.Sea).First();
                var unitsOnShip = Game.UnitsOnShip(ship);
                return unitsOnShip.Contains(this);
            }
        }

        public string AttackSound => TypeDefinition.AttackSound;
        public City CityWithThisUnit => Game.GetCities.FirstOrDefault(c => c.X == X && c.Y == Y);
    }
}
