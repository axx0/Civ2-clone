using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;
using PoskusCiv2.Improvements;

namespace PoskusCiv2
{
    public class City
    {
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

        public bool CanBuildCoastal { get; set; }
        public bool AutobuildMilitaryRule { get; set; }
        public bool StolenTech { get; set; }
        public bool ImprovementSold { get; set; }
        public bool WeLoveKingDay { get; set; }
        public bool CivilDisorder { get; set; }
        public bool CanBuildShips { get; set; }
        public bool Objectivex3 { get; set; }
        public bool Objectivex1 { get; set; }
        public int Owner { get; set; }
        public int Size { get; set; }
        public int WhoBuiltIt { get; set; }
        public int FoodBox { get; set; }
        public int ShieldBox { get; set; }
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public int WorkersInnerCircle { get; set; }
        public int WorkersOn8 { get; set; }
        public int WorkersOn4 { get; set; }
        public int NoOfSpecialistsx4 { get; set; }

        private int _population;
        public int Population
        {
            get
            {
                 _population = 0;
                for (int i = 1; i <= Size; i++) { _population += i * 10000; }
                return _population;
            }
        }

        public IImprovement[] Improvements => _improvements.OrderBy(i => i.Id).ToArray();
        public IImprovement[] Wonders => _wonders.OrderBy(i => i.WId).ToArray();

        private List<IImprovement> _improvements = new List<IImprovement>();
        private List<IImprovement> _wonders = new List<IImprovement>();

        public void AddImprovement(IImprovement improvement) => _improvements.Add(improvement);
        public void AddWonder(IImprovement wonder) => _wonders.Add(wonder);
        
        
        //offsets of squares around the city (0,0) in civ2-format
        private int[,] offsets = new int[20, 2] { { -2, 0 }, { -1, -1 }, { 0, -2 }, { 1, -1 }, { 2, 0 }, { 1, 1 }, { 0, 2 }, { -1, 1 }, { -3, -1 }, { -2, -2 }, { -1, -3 }, { 1, -3 }, { 2, -2 }, { 3, -1 }, { 3, 1 }, { 2, 2 }, { 1, 3 }, { -1, 3 }, { -2, 2 }, { -3, 1 } };
        //Returns coordinates (offsets) of city-surrounding squares according to most FST they have
        private int[,] _priorityOffsets = new int[21, 2];
        public int[,] PriorityOffsets
        {
            get
            {
                //Distribute on those squares that have max(FST)
                int[] countFST = new int[20];
                int[] prioritySquareIndexes = new int[20] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };
                int x, y, x2, y2;
                for (int square = 0; square < 20; square++)
                {
                    x2 = X2 + offsets[square, 0];   //Civ2 format
                    y2 = Y2 + offsets[square, 1];
                    x = (x2 - (y2 % 2)) / 2;    //Real format
                    y = y2;
                    countFST[square] = Game.Terrain[x, y].Food + Game.Terrain[x, y].Shields + Game.Terrain[x, y].Trade;
                }
                Array.Sort(countFST, prioritySquareIndexes);  //this sorts countFST and indexes shows the correct index order
                Array.Reverse(prioritySquareIndexes);   //because it's sorted in wrong order
                //Now with sorted offset indexes make _priorityOffsets (element 0 is always city square itself with index (0,0))
                _priorityOffsets[0, 0] = 0;
                _priorityOffsets[0, 1] = 0;
                for (int i = 0; i < 20; i++)
                {
                    _priorityOffsets[i + 1, 0] = offsets[prioritySquareIndexes[i], 0];
                    _priorityOffsets[i + 1, 1] = offsets[prioritySquareIndexes[i], 1];
                }

                return _priorityOffsets;
            }
            set
            {
                _priorityOffsets = value;
            }
        }
}
}
