using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using civ2.Enums;
using civ2.Forms;
using civ2.Bitmaps;

namespace civ2.Units
{
    internal class Unit : IUnit
    {
        //From RULES.TXT
        public string Name 
        { 
            get { return Rules.UnitName[(int)Type]; } 
        }
        public TechType UntilTech { get; }         // TODO: implement unit UntilTech logic
        public int MaxMovePoints 
        {
            get { return 3 * Rules.UnitMove[(int)Type]; }
        }
        public int Range 
        {
            get { return Rules.UnitRange[(int)Type]; }
        }
        public int Attack 
        { 
            get { return Rules.UnitAttack[(int)Type]; }
        }
        public int Defense 
        {
            get { return Rules.UnitDefense[(int)Type]; }
        }
        public int MaxHitPoints 
        {
            get { return 10 * Rules.UnitHitp[(int)Type]; }
        }
        public int Firepower 
        {
            get { return Rules.UnitFirepwr[(int)Type]; }
        }
        public int Cost 
        {
            get { return Rules.UnitCost[(int)Type]; }
        }
        public int ShipHold
        {
            get { return Rules.UnitHold[(int)Type]; }
        }
        public int AIrole 
        {
            get { return Rules.UnitAIrole[(int)Type]; }
        }
        public TechType PrereqTech { get; }        // TODO: implement unit PrereqTech logic
        public string Flags 
        { 
            get { return Rules.UnitFlags[(int)Type]; }
        }

        public int Id { get; set; }
        public int MovePoints { get; set; }
        public int HitPoints { get; set; }
        public UnitType Type { get; set; }
        public UnitGAS GAS 
        {
            get
            {
                if (Rules.UnitDomain[(int)Type] == 0)
                    return UnitGAS.Ground;
                else if (Rules.UnitDomain[(int)Type] == 1)
                    return UnitGAS.Air;
                else
                    return UnitGAS.Sea;
            }
        }
        public OrderType Order { get; set; }
        public bool FirstMove { get; set; }
        public bool GreyStarShield { get; set; }
        public bool Veteran { get; set; }
        public int CivId { get; set; }
        public int LastMove { get; set; }
        public int CaravanCommodity { get; set; }
        public int HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int MovementCounter { get; set; }

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

            switch (GAS)
            {
                case UnitGAS.Ground:
                    {
                        //Cannot move to ocean tile
                        if (Game.TerrainTile[Xto_, Yto].Type == TerrainType.Ocean)
                        { 
                            break; 
                        }
                        
                        //Cannot move beyond map edge
                        if (Xto_ < 0 || Xto_ >= Data.MapXdim || Yto < 0 || Yto >= Data.MapYdim) 
                        { 
                            //TODO: display a message that a unit cannot move beyond map edges
                            break; 
                        }

                        //Movement possible, reduce movement points
                        if ((Game.TerrainTile[X_, Y].Road || Game.TerrainTile[X_, Y].CityPresent) && (Game.TerrainTile[Xto_, Yto].Road || Game.TerrainTile[Xto_, Yto].CityPresent) ||   //From & To must be cities, road
                            (Game.TerrainTile[X_, Y].River && Game.TerrainTile[Xto_, Yto].River && (movementDirection == OrderType.MoveSW || movementDirection == OrderType.MoveSE || movementDirection == OrderType.MoveNE || movementDirection == OrderType.MoveNW)))    //For rivers only for diagonal movement
                        {
                            MovePoints -= 1;
                        }
                        else
                        {
                            MovePoints -= 3;
                        }

                        unitMoved = true;
                        break;
                    }
                case UnitGAS.Sea:
                    {
                        if (Game.TerrainTile[Xto_, Yto].Type != TerrainType.Ocean)
                        { 
                            break; 
                        }

                        //Cannot move beyond map edge
                        if (Xto_ < 0 || Xto_ >= Data.MapXdim || Yto < 0 || Yto >= Data.MapYdim) 
                        {
                            //TODO: display a message that a unit cannot move beyond map edges
                            break;
                        }

                        MovePoints -= 3;

                        unitMoved = true;
                        break;
                    }
                case UnitGAS.Air:
                    {
                        //Cannot move beyond map edge
                        if (Xto_ < 0 || Xto_ >= Data.MapXdim || Yto < 0 || Yto >= Data.MapYdim)
                        {
                            break;
                        }

                        MovePoints -= 3;

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
                _awaitingOrders = (Order == OrderType.NoOrders || Order == OrderType.GoTo) ? true : false;
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
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Game.TerrainTile[X, Y].Irrigation == false) || (Game.TerrainTile[X, Y].Farmland == false)))
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
            if ((Type == UnitType.Settlers || Type == UnitType.Engineers) && Game.TerrainTile[X, Y].Mining == false && (Game.TerrainTile[X, Y].Type == TerrainType.Mountains || Game.TerrainTile[X, Y].Type == TerrainType.Hills))
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
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Game.TerrainTile[X, Y].Road == false) || (Game.TerrainTile[X, Y].Railroad == false)))
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
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && (Game.TerrainTile[X, Y].Type != TerrainType.Ocean))
            {
                //First invoke city name panel. If cancel is pressed, do nothing.
                //Application.OpenForms.OfType<MapForm>().First().ShowCityNamePanel();
            }
            else
            {
                //Warning!
            }
        }

        public bool IsInCity
        {
            get
            {
                return Game.Cities.Any(city => city.X == X && city.Y == Y); ;
            }
        }

        private bool _isInStack;
        public bool IsInStack
        {
            get 
            {
                List<IUnit> unitsInStack = new List<IUnit>();
                foreach (IUnit unit in Game.Units) 
                    if (unit.X == X && unit.Y == Y) unitsInStack.Add(unit);
                _isInStack = (unitsInStack.Count > 1) ? true : false;
                return _isInStack;
            }
        }

        private bool _isLastInStack;
        public bool IsLastInStack   //determine if unit is last in stack list (return TRUE if it is not in stack)
        {
            get
            {
                List<IUnit> unitsInStack = new List<IUnit>();
                foreach (IUnit unit in Game.Units) 
                    if (unit.X == X && unit.Y == Y) unitsInStack.Add(unit);
                _isLastInStack = (unitsInStack.Last() == this) ? true : false;
                return _isLastInStack;
            }
        }

        private Bitmap _graphicMapPanel;
        public Bitmap GraphicMapPanel
        {
            get
            {
                _graphicMapPanel = Images.CreateUnitBitmap(this, IsInStack, MapPanel.ZoomLvl);
                return _graphicMapPanel;
            }
        }
    }
}
