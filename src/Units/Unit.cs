using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using civ2.Enums;
using civ2.Bitmaps;

namespace civ2.Units
{
    internal class Unit : BaseInstance, IUnit
    {
        // From RULES.TXT
        public string Name => Game.Rules.UnitName[(int)Type];
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
        public int AttackFactor => Game.Rules.UnitAttack[(int)Type];
        public int DefenseFactor => Game.Rules.UnitDefense[(int)Type];
        public int MaxHitpoints => Game.Rules.UnitHitp[(int)Type];
        public int Firepower => Game.Rules.UnitFirepwr[(int)Type];
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
        public int MovePoints
        {
            get { return MaxMovePoints - MovePointsLost; }
        }
        public int MovePointsLost { get; set; }
        public int HitPoints
        {
            get { return MaxHitpoints - HitPointsLost; }
        }
        public int HitPointsLost { get; set; }
        public UnitType Type { get; set; }
        public OrderType Order { get; set; }
        public bool FirstMove { get; set; }
        public bool GreyStarShield { get; set; }
        public bool Veteran { get; set; }
        public Civilization Owner { get; set; }
        public int LastMove { get; set; }
        public CommodityType CaravanCommodity { get; set; }
        public City HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MovementCounter { get; set; }
        public int Xpx => 4 * X * (Game.Zoom + 8);
        public int Ypx => 2 * (Y - 1) * (Game.Zoom + 8);

        public bool Move(OrderType movementDirection)
        {
            //Determine coordinates of movement
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

            //Convert civ2 coords to real coords (for existing & destination square)
            int X_ = (X - Y % 2) / 2;
            int Xto_ = (Xto - Yto % 2) / 2;

            bool unitMoved = false;

            switch (Domain)
            {
                case UnitGAS.Ground:
                    {
                        //Cannot move to ocean tile
                        if (Map.Tile[Xto_, Yto].Type == TerrainType.Ocean)
                        {
                            break;
                        }

                        //Cannot move beyond map edge
                        if (Xto_ < 0 || Xto_ >= Map.Xdim || Yto < 0 || Yto >= Map.Ydim)
                        {
                            //TODO: display a message that a unit cannot move beyond map edges
                            break;
                        }

                        //Movement possible, reduce movement points
                        if ((Map.Tile[X_, Y].Road || Map.Tile[X_, Y].CityPresent) && (Map.Tile[Xto_, Yto].Road || Map.Tile[Xto_, Yto].CityPresent) ||   //From & To must be cities, road
                            (Map.Tile[X_, Y].River && Map.Tile[Xto_, Yto].River && (movementDirection == OrderType.MoveSW || movementDirection == OrderType.MoveSE || movementDirection == OrderType.MoveNE || movementDirection == OrderType.MoveNW)))    //For rivers only for diagonal movement
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
                        if (Map.Tile[Xto_, Yto].Type != TerrainType.Ocean)
                        {
                            break;
                        }

                        //Cannot move beyond map edge
                        if (Xto_ < 0 || Xto_ >= Map.Xdim || Yto < 0 || Yto >= Map.Ydim)
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
                        //Cannot move beyond map edge
                        if (Xto_ < 0 || Xto_ >= Map.Xdim || Yto < 0 || Yto >= Map.Ydim)
                        {
                            break;
                        }

                        MovePointsLost += 3;

                        unitMoved = true;
                        break;
                    }
            }

            //If unit moved, update its X-Y coords
            if (unitMoved)
            {
                //set previous coords
                LastXY = new int[] { X, Y };

                //set last move for unit
                if (movementDirection == OrderType.MoveNE) LastMove = 0;
                else if (movementDirection == OrderType.MoveE) LastMove = 1;
                else if (movementDirection == OrderType.MoveSE) LastMove = 2;
                else if (movementDirection == OrderType.MoveS) LastMove = 3;
                else if (movementDirection == OrderType.MoveSW) LastMove = 4;
                else if (movementDirection == OrderType.MoveW) LastMove = 5;
                else if (movementDirection == OrderType.MoveNW) LastMove = 6;
                else if (movementDirection == OrderType.MoveN) LastMove = 7;

                //set new coords
                X = Xto;
                Y = Yto;
            }

            return unitMoved;
        }

        public int[] LastXY { get; set; }   //XY position of unit before it moved
        public int[] LastXYpx => new int[] { 4 * LastXY[0] * (Game.Zoom + 8), 2 * (LastXY[1] - 1) * (Game.Zoom + 8) };

        private bool _turnEnded;
        public bool TurnEnded
        {
            get
            {
                if (MovePoints <= 0) _turnEnded = true;
                if (Order == OrderType.Fortified || Order == OrderType.Transform || Order == OrderType.Fortify || Order == OrderType.BuildIrrigation ||
                    Order == OrderType.BuildRoad || Order == OrderType.BuildAirbase || Order == OrderType.BuildFortress || Order == OrderType.BuildMine) _turnEnded = true;
                return _turnEnded;
            }
            set { _turnEnded = value; }
        }

        private bool _awaitingOrders;
        public bool AwaitingOrders
        {
            get
            {
                _awaitingOrders = Order == OrderType.NoOrders || Order == OrderType.GoTo;
                if (TurnEnded) _awaitingOrders = false;
                return _awaitingOrders;
            }
            set { _awaitingOrders = value; }
        }

        public void SkipTurn()
        {
            TurnEnded = true;
            LastMove = 255; //FF hex
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

        public Bitmap Graphic(bool isInStack, int zoom) => Draw.Unit(this, isInStack, zoom);

    }
}
