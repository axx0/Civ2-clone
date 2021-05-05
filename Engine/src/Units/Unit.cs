using System;
using System.Linq;
using Civ2engine.Enums;

namespace Civ2engine.Units
{
    internal class Unit : BaseInstance, IUnit
    {
        // From RULES.TXT
        public string Name => Game.Rules.UnitName[(int)Type];
        public bool Dead { get; set; }
        public AdvanceType? UntilTech
        {
            get
            {
                if (Game.Rules.UnitUntil[(int)Type] == "nil")
                    return null;
                else
                    return (AdvanceType)Array.IndexOf(Game.Rules.AdvanceShortName, Game.Rules.UnitUntil[(int)Type]);
            }
        }
        public UnitGAS Domain => (UnitGAS)Game.Rules.UnitDomain[(int)Type];
        public int MaxMovePoints => Game.Rules.UnitMove[(int)Type];
        public int FuelRange => Game.Rules.UnitRange[(int)Type];
        public int AttackBase => Game.Rules.UnitAttack[(int)Type];
        public int DefenseBase => Game.Rules.UnitDefense[(int)Type];

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

        public int DefenseFactor(IUnit attackingUnit)
        {
            // Base defense factor from RULES
            double DF = (double)DefenseBase;

            // Bonus for veteran units
            if (Veteran) DF *= 1.5;

            // City walls bonus (applies only to land units)
            if (IsInCity && CityWithThisUnit.ImprovementExists(ImprovementType.CityWalls) && Domain == UnitGAS.Ground && !NegatesCityWalls) DF *= 3;
            // Fortress bonus (Applies only to land units. Unit doesn't have to be fortified. Doesn't count if air unit is attacking.)
            else if (Map.TileC2(X, Y).Fortress && Domain == UnitGAS.Ground && attackingUnit.Domain != UnitGAS.Air) DF *= 2;
            // Fortified bonus
            else if (Order == OrderType.Fortified && Domain == UnitGAS.Ground) DF *= 1.5;

            // Effect of SAM batteries (only when attacked from air)
            if (IsInCity && CityWithThisUnit.ImprovementExists(ImprovementType.SAMbattery) && attackingUnit.Domain == UnitGAS.Air) DF *= 2;

            // Effect of SDI
            if (IsInCity && CityWithThisUnit.ImprovementExists(ImprovementType.SDIdefense) && attackingUnit.Type == UnitType.CruiseMsl) DF *= 2;

            // Effect of coastal fortress
            if (IsInCity && CityWithThisUnit.ImprovementExists(ImprovementType.CoastalFort) && attackingUnit.Domain == UnitGAS.Sea) DF *= 2;

            // Effect of terrain
            if ((Map.TileC2(X, Y).Type == TerrainType.Forest) || (Map.TileC2(X, Y).Type == TerrainType.Jungle) || (Map.TileC2(X, Y).Type == TerrainType.Swamp))
            {
                if (Map.TileC2(X, Y).River) DF *= 2;
                else DF *= 1.5;
            }
            else if (Map.TileC2(X, Y).Type == TerrainType.Hills)
            {
                if (Map.TileC2(X, Y).River) DF *= 2.5;
                else DF *= 2;
            }
            else if (Map.TileC2(X, Y).Type == TerrainType.Mountains) 
            {
                if (Map.TileC2(X, Y).River) DF *= 3.5;
                else DF *= 3;
            }

            return (int)DF;
        }

        public int FirepowerBase => Game.Rules.UnitFirepwr[(int)Type];
        public int Firepower(bool isThisUnitAttacker, IUnit otherUnit)
        {
            // Base firepower from RULES
            double FP = (double)FirepowerBase;

            // When a sea unit attacks a land unit, both units have their firepower reduced to 1
            if (isThisUnitAttacker && Domain == UnitGAS.Sea && otherUnit.Domain == UnitGAS.Ground && Map.TileC2(otherUnit.X, otherUnit.Y).Type != TerrainType.Ocean) FP = 1;    // attacker (sea)
            else if (!isThisUnitAttacker && Domain == UnitGAS.Ground && otherUnit.Domain == UnitGAS.Sea && Map.TileC2(otherUnit.X, otherUnit.Y).Type == TerrainType.Ocean) FP = 1;  // defender (land)

            // Cought in port (A sea unit’s firepower is reduced to 1 when it is caught in port (or on a land square) by a land or air unit; The attacking air or land unit’s firepower is doubled)
            if (!isThisUnitAttacker && Domain == UnitGAS.Sea && IsInCity && otherUnit.Domain == UnitGAS.Ground) FP = 1; // defender (sea unit in city)
            else if (isThisUnitAttacker && Domain == UnitGAS.Ground && otherUnit.Domain == UnitGAS.Sea && otherUnit.IsInCity) FP *= 2;  // attacker (land)

            return (int)FP;
        }
        public int Cost => Game.Rules.UnitCost[(int)Type];
        public int ShipHold => Game.Rules.UnitHold[(int)Type];
        public AIroleType AIrole => (AIroleType)Game.Rules.UnitAIrole[(int)Type];
        public AdvanceType? PrereqAdvance
        {
            get
            {
                if (Game.Rules.UnitPrereq[(int)Type] == "nil" || Game.Rules.UnitPrereq[(int)Type] == "no")
                    return null;
                else
                    return (AdvanceType)Array.IndexOf(Game.Rules.AdvanceShortName, Game.Rules.UnitPrereq[(int)Type]);
            }
        }
        public bool TwoSpaceVisibility => Game.Rules.UnitFlags[(int)Type][14] == '1';
        public bool IgnoreZonesOfControl => Game.Rules.UnitFlags[(int)Type][13] == '1';
        public bool CanMakeAmphibiousAssaults => Game.Rules.UnitFlags[(int)Type][12] == '1';
        public bool SubmarineAdvantagesDisadvantages => Game.Rules.UnitFlags[(int)Type][11] == '1';
        public bool CanAttackAirUnits => Game.Rules.UnitFlags[(int)Type][10] == '1';    // fighter
        public bool ShipMustStayNearLand => Game.Rules.UnitFlags[(int)Type][9] == '1';  // trireme
        public bool NegatesCityWalls => Game.Rules.UnitFlags[(int)Type][8] == '1';  // howitzer
        public bool CanCarryAirUnits => Game.Rules.UnitFlags[(int)Type][7] == '1';  // carrier
        public bool CanMakeParadrops => Game.Rules.UnitFlags[(int)Type][6] == '1';
        public bool Alpine => Game.Rules.UnitFlags[(int)Type][5] == '1';    // treats all squares as road
        public bool X2onDefenseVersusHorse => Game.Rules.UnitFlags[(int)Type][4] == '1';    // pikemen
        public bool FreeSupportForFundamentalism => Game.Rules.UnitFlags[(int)Type][3] == '1';    // fanatics
        public bool DestroyedAfterAttacking => Game.Rules.UnitFlags[(int)Type][2] == '1';    // missiles
        public bool X2onDefenseVersusAir => Game.Rules.UnitFlags[(int)Type][1] == '1';    // AEGIS
        public bool UnitCanSpotSubmarines => Game.Rules.UnitFlags[(int)Type][0] == '1';


        public int Id { get; set; }

        public int MovePoints => MaxMovePoints - MovePointsLost;

        public int MovePointsLost { get; set; }
        public int HitpointsBase => Game.Rules.UnitHitp[(int)Type];
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

            bool unitMoved = false;
            switch (Domain)
            {
                case UnitGAS.Ground:
                    {
                        // Cannot move to ocean tile
                        if (Map.TileC2(Xto, Yto).Type == TerrainType.Ocean) break;

                        // Cannot move beyond map edge
                        if (Xto < 0 || Xto >= 2 * Map.Xdim || Yto < 0 || Yto >= Map.Ydim)
                        {
                            //TODO: display a message that a unit cannot move beyond map edges
                            break;
                        }

                        // Movement possible, reduce movement points
                        if ((Map.TileC2(X, Y).Road || Map.TileC2(X, Y).IsCityPresent) && (Map.TileC2(Xto, Yto).Road || Map.TileC2(Xto, Yto).IsCityPresent) ||   //From & To must be cities, road
                            (Map.TileC2(X, Y).River && Map.TileC2(Xto, Yto).River && (movementDirection == OrderType.MoveSW || movementDirection == OrderType.MoveSE || movementDirection == OrderType.MoveNE || movementDirection == OrderType.MoveNW)))    //For rivers only for diagonal movement
                        {
                            MovePointsLost += 1;
                        }
                        else
                        {
                            MovePointsLost += 3;
                        }

                        unitMoved = true;
                        break;
                    }
                case UnitGAS.Sea:
                    {
                        if (Map.TileC2(Xto, Yto).Type != TerrainType.Ocean) break;

                        // Cannot move beyond map edge
                        if (Xto < 0 || Xto >= 2 * Map.Xdim || Yto < 0 || Yto >= Map.Ydim)
                        {
                            //TODO: display a message that a unit cannot move beyond map edges
                            break;
                        }

                        MovePointsLost += 3;

                        unitMoved = true;
                        break;
                    }
                case UnitGAS.Air:
                    {
                        // Cannot move beyond map edge
                        if (Xto < 0 || Xto >= 2 * Map.Xdim || Yto < 0 || Yto >= Map.Ydim) break;

                        MovePointsLost += 3;

                        unitMoved = true;
                        break;
                    }
            }

            // If unit moved, update its X-Y coords
            if (unitMoved)
            {
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
            MovePointsLost = MovePoints;
            PrevXY = new int[] { X, Y };
        }

        public void Fortify()
        {
            Order = OrderType.Fortify;
        }

        public void BuildIrrigation()
        {
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Map.Tile[X, Y].Irrigation == false) || (Map.Tile[X, Y].Farmland == false)))
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
            if ((Type == UnitType.Settlers || Type == UnitType.Engineers) && Map.Tile[X, Y].Mining == false && (Map.Tile[X, Y].Type == TerrainType.Mountains || Map.Tile[X, Y].Type == TerrainType.Hills))
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
            if (Type == UnitType.Engineers)
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
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Map.Tile[X, Y].Road == false) || (Map.Tile[X, Y].Railroad == false)))
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
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && (Map.Tile[X, Y].Type != TerrainType.Ocean))
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
        public bool IsInStack => Game.GetUnits.Where(u => u.X == X && u.Y == Y).Count() > 1;
        public bool IsLastInStack => Game.GetUnits.Where(u => u.X == X && u.Y == Y).Last() == this;
        public City CityWithThisUnit => Game.GetCities.Where(c => c.X == X && c.Y == Y).First();
    }
}
