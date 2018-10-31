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
        public UnitType Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool FirstMove { get; set; }
        public bool GreyStarShield { get; set; }
        public bool Veteran { get; set; }        
        public int Civ { get; set; }
        public int MovesMade { get; set; }
        public int HitpointsLost { get; set; }
        public int CaravanCommodity { get; set; }
        public int Orders { get; set; }
        public int HomeCity { get; set; }
        public int GoToX { get; set; }
        public int GoToY { get; set; }
        public int LinkOtherUnitsOnTop { get; set; }
        public int LinkOtherUnitsUnder { get; set; }

        public int Cost { get; }
        public int Attack { get; }
        public int Defense { get; }
        public int HitPoints { get; }
        public int Firepower { get; }
        public int MovementRate { get; }
        public int LandSeaAirUnit { get; }

        private int movesLeft;
        public int MovesLeft
        {
            get { return movesLeft; }
            set { movesLeft = 3 * value; }
        }
        

        public string Name { get; set; }

        protected BaseUnit(int cost = 1, int attack = 1, int defense = 1, int hitpoints = 1, int firepower = 1, int move = 1)
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
            MovementRate = move;
            LandSeaAirUnit = 1;
            MovesLeft = move;
        }
    }
}
