using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using RTciv2.Enums;
using RTciv2.Improvements;
using RTciv2.Imagery;
using RTciv2.Forms;

namespace RTciv2
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
        public int FoodInStorage { get; set; }
        public int ShieldsProgress { get; set; }
        public int NetTrade { get; set; }
        public string Name { get; set; }
        public int WorkersInnerCircle { get; set; }
        public int WorkersOn8 { get; set; }
        public int WorkersOn4 { get; set; }
        public int NoOfSpecialistsx4 { get; set; }
        public int ItemInProduction { get; set; }
        public int ActiveTradeRoutes { get; set; }
        public int NoOfTradeIcons { get; set; }
        public int FoodProduction { get; set; }
        public int ShieldProduction { get; set; }
        public int HappyCitizens { get; set; }
        public int UnhappyCitizens { get; set; }

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

        private List<IImprovement> _improvements = new List<IImprovement>();

        public void AddImprovement(IImprovement improvement) => _improvements.Add(improvement);
        
        
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
                    countFST[square] = Game.Map[x, y].Food + Game.Map[x, y].Shields + Game.Map[x, y].Trade;
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

        private int _foodtotal;
        public int FoodTotal
        {
            get
            {
                _foodtotal = 0;
                for (int i = 0; i <= Size; i++)
                {
                    int x2 = X2 + PriorityOffsets[i, 0];   //Civ2 format
                    int y2 = Y2 + PriorityOffsets[i, 1];
                    int x = (x2 - (y2 % 2)) / 2;    //Real format
                    int y = y2;
                    _foodtotal += Game.Map[x, y].Food;
                }
                return _foodtotal;
            }
        }

        private int _food;
        public int Food
        {
            get
            {
                _food = Size * 2;
                return _food;
            }
        }

        private int _surplus;
        public int Surplus
        {
            get
            {
                _surplus = FoodTotal - Food;
                return _surplus;
            }
        }

        private int _trade;
        public int Trade
        {
            get
            {
                _trade = 5;
                return _trade;
            }
        }

        private int _corruption;
        public int Corruption
        {
            get
            {
                _corruption = 3;
                return _corruption;
            }
        }

        private int _tax;
        public int Tax
        {
            get
            {
                return _tax;
            }
            set
            {
                _tax = value;
            }
        }

        private int _lux;
        public int Lux
        {
            get
            {
                _lux = 3;
                return _lux;
            }
        }

        private int _science;
        public int Science
        {
            get
            {
                return _science;
            }
            set
            {
                _science = value;
            }
        }

        private int _support;
        public int Support
        {
            get
            {
                _support = 5;
                return _support;
            }
        }

        private int _production;
        public int Production
        {
            get
            {
                _production = 3;
                return _production;
            }
        }

        private bool _isInView;
        public bool IsInView    //determine if city is visible in current map panel view
        {
            get
            {
                if ((X > MapPanel.MapOffsetXY[0]) && (X < MapPanel.MapOffsetXY[0] + MapPanel.MapVisSqXY[0]) && (Y > MapPanel.MapOffsetXY[1]) && (Y < MapPanel.MapOffsetXY[1] + MapPanel.MapVisSqXY[1])) _isInView = true;
                else _isInView = false;
                return _isInView;
            }
        }

        private Bitmap _graphic;
        public Bitmap Graphic
        {
            get
            {
                _graphic = Images.CreateCityBitmap(this, true, MapPanel.ZoomLvl);
                return _graphic;
            }
        }

        private Bitmap _textGraphic;
        public Bitmap TextGraphic
        {
            get
            {
                //Define text characteristics for zoom levels
                int shadowOffset, fontSize;
                switch (MapPanel.ZoomLvl)
                {
                    case 1: shadowOffset = 0; fontSize = 1; break;
                    case 2: shadowOffset = 0; fontSize = 3; break;
                    case 3: shadowOffset = 0; fontSize = 5; break;
                    case 4: shadowOffset = 1; fontSize = 7; break;
                    case 5: shadowOffset = 1; fontSize = 10; break;
                    case 6: shadowOffset = 1; fontSize = 11; break;
                    case 7: shadowOffset = 1; fontSize = 13; break;
                    case 8: shadowOffset = 2; fontSize = 14; break;
                    case 9: shadowOffset = 2; fontSize = 16; break;
                    case 10: shadowOffset = 2; fontSize = 17; break;
                    case 11: shadowOffset = 2; fontSize = 19; break;
                    case 12: shadowOffset = 2; fontSize = 21; break;
                    case 13: shadowOffset = 2; fontSize = 24; break;
                    case 14: shadowOffset = 2; fontSize = 25; break;
                    case 15: shadowOffset = 2; fontSize = 26; break;
                    case 16: shadowOffset = 2; fontSize = 28; break;
                    default: shadowOffset = 2; fontSize = 14; break;
                }
                //Draw
                Graphics gr = Graphics.FromImage(new Bitmap(1, 1));
                SizeF stringSize = gr.MeasureString(Name, new Font("Times New Roman", fontSize));
                int stringWidth = (int)stringSize.Width;
                int stringHeight = (int)stringSize.Height;
                _textGraphic = new Bitmap(stringWidth + 2, stringHeight + 2);
                Graphics g = Graphics.FromImage(_textGraphic);
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.DrawString(Name, new Font("Times New Roman", fontSize), Brushes.Black, new PointF(shadowOffset, 0));
                g.DrawString(Name, new Font("Times New Roman", fontSize), Brushes.Black, new PointF(0, shadowOffset));
                g.DrawString(Name, new Font("Times New Roman", fontSize), new SolidBrush(CivColors.CityTextColor[Owner]), new PointF(0, 0));
                return _textGraphic;
            }
        }
    }
}
