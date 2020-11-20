using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;
using PoskusCiv2.Techs;

namespace PoskusCiv2.Units
{
    internal class BaseUnit : IUnit
    {
        //Original data (should be read from txt files)
        public int Cost { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int HitPoints { get; }
        public int Firepower { get; }
        public int MoveRate { get; }

        public UnitType Type { get; set; }
        public UnitGAS GAS { get; set; }
        public UnitAction Action { get; set; }
        public string Name { get; set; }

        private int _x;
        public int X
        {
            get { return _x; }
            set { _x = value; }
        }

        private int _y;
        public int Y
        {
            get { return _y; }
            set { _y = value; }
        }

        public bool FirstMove { get; set; }
        public bool GreyStarShield { get; set; }
        public bool Veteran { get; set; }
        public int Civ { get; set; }
        public int HitpointsLost { get; set; }
        public int LastMove { get; set; }
        public int CaravanCommodity { get; set; }
        public int Orders { get; set; }
        public int HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }
        public int Counter { get; set; }

        public int X2   //Civ2 style
        {
            get { return 2 * X + (Y % 2); }
        }

        public int Y2   //Civ2 style
        {
            get { return Y; }
        }

        public int GoToX2   //Civ2 style
        {
            get { return 2 * GoToX + (GoToY % 2); }
        }

        public int GoToY2   //Civ2 style
        {
            get { return GoToY; }
        }

        private int _movePointsLost;
        public int MovePointsLost
        {
            get { return _movePointsLost; }
            set { _movePointsLost = value; }
        }
        
        public void Move(int moveX, int moveY)
        {
            int xTo = X2 + moveX;    //Civ2-style
            int yTo = Y2 + moveY;
            int Xto = (xTo - yTo % 2) / 2;  //from civ2-style to real coords
            int Yto = yTo;

            if (Game.Terrain[Xto, Yto].Type != TerrainType.Ocean)
            {
                if ((Game.Terrain[X, Y].Road || Game.Terrain[X, Y].CityPresent) && (Game.Terrain[Xto, Yto].Road || Game.Terrain[Xto, Yto].CityPresent)) //From & To must be cities or road (movement reduced)
                {
                    MovePointsLost = MovePointsLost + 1;
                }
                else
                {
                    MovePointsLost = MovePointsLost + 3;
                }
                X = Xto;
                Y = Yto;                
            }

            if (MovePointsLost >= 3 * MoveRate)
            {
                TurnEnded = true;
                MovePointsLost = 3 * MoveRate;
            }

            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        private bool _turnEnded;
        public bool TurnEnded
        {
            get
            {
                if (MovePointsLost >= 3 * MoveRate)
                {
                    MovePointsLost = 3 * MoveRate;
                    _turnEnded = true;
                }
                else if (Action == UnitAction.Fortified || Action == UnitAction.Sentry || Action == UnitAction.TransformTerr || Action == UnitAction.Fortify || Action == UnitAction.BuildIrrigation || Action == UnitAction.BuildRoadRR || Action == UnitAction.BuildAirbase || Action == UnitAction.BuildFortress || Action == UnitAction.BuildMine) { _turnEnded = true; }
                else { _turnEnded = false; }

                return _turnEnded;
            }
            set { _turnEnded = value; }
        }

        public void SkipTurn()
        {
            TurnEnded = true;
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        public void Fortify()
        {
            Action = UnitAction.Fortify;
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        public void Irrigate()
        {
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Game.Terrain[X, Y].Irrigation == false) || (Game.Terrain[X, Y].Farmland == false)))
            {
                Action = UnitAction.BuildIrrigation;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        public void BuildMines()
        {
            if ((Type == UnitType.Settlers || Type == UnitType.Engineers) && Game.Terrain[X, Y].Mining == false && (Game.Terrain[X, Y].Type == TerrainType.Mountains || Game.Terrain[X, Y].Type == TerrainType.Hills))
            {
                Action = UnitAction.BuildMine;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        public void Terraform()
        {
            if (Type == UnitType.Engineers)
            {
                Action = UnitAction.TransformTerr;
            }
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        public void Sentry()
        {
            Action = UnitAction.Sentry;
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        public void BuildRoad()
        {
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Game.Terrain[X, Y].Road == false) || (Game.Terrain[X, Y].Railroad == false)))
            {
                Action = UnitAction.BuildRoadRR;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
            Actions.UpdateUnit(Game.Instance.ActiveUnit);
        }

        protected BaseUnit(int cost, int attackFactor, int defenseFactor, int hitPoints, int firepwr, int moveRate)
        {
            X = -1;
            Y = -1;
            GreyStarShield = false;
            Veteran = false;
            Civ = -1;
            HitpointsLost = 0;
            CaravanCommodity = 0;
            Orders = 0;
            HomeCity = 0;
            GoToX = -1;
            GoToY = -1;
            LinkOtherUnitsOnTop = 0;
            LinkOtherUnitsUnder = 0;

            Cost = cost;
            Attack = attackFactor;
            Defense = defenseFactor;
            HitPoints = hitPoints;
            Firepower = firepwr;
            MoveRate = moveRate;
        }
    }
}
