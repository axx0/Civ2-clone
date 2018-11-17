using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

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
        public int StartingMoves { get; }

        public UnitType Type { get; set; }
        public UnitLSA LSA { get; set; }
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
        public bool TurnEnded { get; set; }
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

        private int _movesMade;
        public int MovesMade
        {
            get { return _movesMade; }
            set { _movesMade = value; }
        }
        
        public void Move(int moveX, int moveY)
        {
            int xTo = 2 * X + Y % 2 + moveX;    //new coordinates in Civ2-style
            int yTo = Y + moveY;
            int Xto = (xTo - yTo % 2) / 2;  //from civ2-style to real coords
            int Yto = yTo;

            if (Game.Terrain[Xto, Yto].Type != TerrainType.Ocean)
            {
                if ((Game.Terrain[X, Y].Road || Game.Terrain[X, Y].CityPresent) && (Game.Terrain[Xto, Yto].Road || Game.Terrain[Xto, Yto].CityPresent)) //From & To must be cities or road (movement reduced)
                {
                    MovesMade = MovesMade + 1;
                }
                else
                {
                    MovesMade = MovesMade + 3;
                }
                X = Xto;
                Y = Yto;                
            }

            if (MovesMade >= 3 * StartingMoves)
            {
                TurnEnded = true;
                MovesMade = 3 * StartingMoves;
            }

            Actions.UpdateUnit();
        }

        public void SkipTurn()
        {
            TurnEnded = true;
            Actions.UpdateUnit();
        }

        public void Fortify()
        {
            Action = UnitAction.Fortify;
            TurnEnded = true;
            Actions.UpdateUnit();
        }

        public void Irrigate()
        {
            if ((Type == UnitType.Settlers) || (Type == UnitType.Engineers))
            {
                Action = UnitAction.BuildIrrigation;
                TurnEnded = true;
            }
            else
            {
                Action = UnitAction.Wait;
            }

            Actions.UpdateUnit();
        }

        public void Terraform()
        {
            if (Type == UnitType.Engineers)
            {
                Action = UnitAction.TransformTerr;
                TurnEnded = true;
            }
            else
            {
                Action = UnitAction.Wait;
            }

            Actions.UpdateUnit();
        }

        public void Sentry()
        {
            Action = UnitAction.Sentry;
            TurnEnded = true;
            Actions.UpdateUnit();
        }

        public void BuildRoad()
        {
            Action = UnitAction.BuildRoadRR;

        }

        protected BaseUnit(int cost = 1, int attack = 1, int defense = 1, int hitpoints = 1, int firepower = 1, int moves = 1)
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
            Attack = attack;
            Defense = defense;
            HitPoints = hitpoints;
            Firepower = firepower;
            StartingMoves = moves;
        }
    }
}
