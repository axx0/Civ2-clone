using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PoskusCiv2.Enums;
using PoskusCiv2.Sounds;
using PoskusCiv2.Forms;

namespace PoskusCiv2.Units
{
    internal class Unit : IUnit
    {
        //From RULES.TXT
        public string Name { get; set; }
        public TechType UntilTech { get; set; }
        public int MoveRate { get; set; }
        public int Range { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public int MaxHitPoints { get; set; }
        public int HitPoints { get; set; }
        public int Firepower { get; set; }
        public int Cost { get; set; }
        public int ShipHold { get; set; }
        public int AIrole { get; set; }
        public TechType PrereqTech { get; set; }
        public string Flags { get; set; }

        public UnitType Type { get; set; }
        public UnitGAS GAS { get; set; }
        public OrderType Action { get; set; }

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

        public int X { get; set; }
        public int Y { get; set; }

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
                if ((Game.Terrain[X, Y].Road || Game.Terrain[X, Y].CityPresent) && (Game.Terrain[Xto, Yto].Road || Game.Terrain[Xto, Yto].CityPresent) ||   //From & To must be cities, road
                    (Game.Terrain[X, Y].River && Game.Terrain[Xto, Yto].River && moveX < 2 && moveY < 2)    //For rivers only for diagonal movement
                    )
                {
                    MovePointsLost += 1;
                }
                else
                {
                    MovePointsLost += 3;
                }
                X = Xto;
                Y = Yto;

                //for animation of movement
                if (!Game.Options.FastPieceSlide) Application.OpenForms.OfType<MapForm>().First().AnimateUnit(this, X2 - moveX, Y2 - moveY);    //send coords of unit starting loc
    
                Sound.MoveSound.Play();
            }

            if (MovePointsLost >= 3 * MoveRate)
            {
                TurnEnded = true;
                MovePointsLost = 3 * MoveRate;
            }            
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
                if (Action == OrderType.Fortified || Action == OrderType.Sleep || Action == OrderType.Transform || Action == OrderType.Fortify || Action == OrderType.BuildIrrigation || Action == OrderType.BuildRoad || Action == OrderType.BuildAirbase || Action == OrderType.BuildFortress || Action == OrderType.BuildMine) _turnEnded = true;
                
                return _turnEnded;
            }
            set { _turnEnded = value; }
        }

        public void SkipTurn()
        {
            TurnEnded = true;
        }

        public void Fortify()
        {
            Action = OrderType.Fortify;
        }

        public void BuildIrrigation()
        {
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Game.Terrain[X, Y].Irrigation == false) || (Game.Terrain[X, Y].Farmland == false)))
            {
                Action = OrderType.BuildIrrigation;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
        }

        public void BuildMines()
        {
            if ((Type == UnitType.Settlers || Type == UnitType.Engineers) && Game.Terrain[X, Y].Mining == false && (Game.Terrain[X, Y].Type == TerrainType.Mountains || Game.Terrain[X, Y].Type == TerrainType.Hills))
            {
                Action = OrderType.BuildMine;
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
                Action = OrderType.Transform;
            }
        }

        public void Sleep()
        {
            Action = OrderType.Sleep;
        }

        public void BuildRoad()
        {
            if (((Type == UnitType.Settlers) || (Type == UnitType.Engineers)) && ((Game.Terrain[X, Y].Road == false) || (Game.Terrain[X, Y].Railroad == false)))
            {
                Action = OrderType.BuildRoad;
                Counter = 0;    //reset counter
            }
            else
            {
                //Warning!
            }
        }

        //When making a new unit, read stats from RULES.TXT
        public Unit(UnitType type)
        {
            Name = ReadFiles.UnitName[(int)type];
            //UntilTech = TO-DO
            if (ReadFiles.UnitDomain[(int)type] == 0) GAS = UnitGAS.Ground;
            else if (ReadFiles.UnitDomain[(int)type] == 1) GAS = UnitGAS.Air;
            else GAS = UnitGAS.Sea;
            MoveRate = ReadFiles.UnitMove[(int)type];
            Range = ReadFiles.UnitRange[(int)type];
            Attack = ReadFiles.UnitAttack[(int)type];
            Defense = ReadFiles.UnitDefense[(int)type];
            MaxHitPoints = 10 * ReadFiles.UnitHitp[(int)type];
            HitPoints = MaxHitPoints;
            Firepower = ReadFiles.UnitFirepwr[(int)type];
            Cost = ReadFiles.UnitCost[(int)type];
            ShipHold = ReadFiles.UnitHold[(int)type];
            AIrole = ReadFiles.UnitAIrole[(int)type];
            //PrereqTech = TO-DO
            Flags = ReadFiles.UnitFlags[(int)type];
        }

    }
}
