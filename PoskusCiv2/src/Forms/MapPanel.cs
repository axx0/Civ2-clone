using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Timers;
using ExtensionMethods;
using RTciv2.Events;
using RTciv2.Imagery;
using RTciv2.Units;
using RTciv2.Enums;

namespace RTciv2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        private static DoubleBufferedPanel DrawPanel;
        StringFormat sf = new StringFormat();

        private List<Bitmap> AnimationBitmap;        
        private int MapGridVar { get; set; }    //style of map grid presentation
        public static bool UnitMoved { get; set; }
        private System.Windows.Forms.Timer Timer;    //timer for blinking (unit or viewing piece), moving unit, etc.
        //private System.Threading.Timer Timer2;
        //private System.Timers.Timer Timer3;
        private AnimationType AnimType;
        int TimerCounter;
        Label HelpLabel;

        Stopwatch sw1 = new Stopwatch();
        Stopwatch sw2 = new Stopwatch();
        //Stopwatch sw3 = new Stopwatch();
        long t_prev;
        long t_now;

        public delegate void MapViewChanged();
        public static event MapViewChanged MapViewChangedEvent;

        public MapPanel(int width, int height)
        {
            Size = new Size(width, height);
            this.Paint += new PaintEventHandler(MapPanel_Paint);

            Actions.OnMoveUnitCommand += MoveUnitCommand;
            Actions.OnNewPlayerTurn += NewPlayerTurn;
            Actions.OnNewUnitChosen += NewUnitWasChosen;
            Actions.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            MainCiv2Window.OnCheckIfCityCanBeViewed += CheckIfCityCanBeViewed;

            DrawPanel = new DoubleBufferedPanel() 
            {
                Location = new Point(11, 38),
                Size = new Size(Width - 22, Height - 49),
                BackColor = Color.Black 
            };
            Controls.Add(DrawPanel);
            DrawPanel.Paint += DrawPanel_Paint;
            DrawPanel.MouseClick += DrawPanel_MouseClick;

            NoSelectButton ZoomINButton = new NoSelectButton
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 23, 23)
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            NoSelectButton ZoomOUTButton = new NoSelectButton
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 23, 23)
            };
            ZoomOUTButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomOUTButton);
            ZoomOUTButton.Click += ZoomOUTclicked;

            //Initialize variables
            ZoomLvl = 8;  // TODO: zoom needs to be read from SAV
            MapGridVar = 0;
            ViewingPiecesMode = (Game.Instance.ActiveUnit == null) ? true : false;  //if no unit is active at start --> all units ended turn
            ClickedXY = Data.ClickedXY;
            ActiveXY = Data.ActiveXY;
            CenterSqXY = ActiveXY;
            UnitMoved = false;
            //TODO: Implement zoom

            //Uncomment this for help in drawing-logic
            //HelpLabel = new Label
            //{
            //    Location = new Point(1000, 50),
            //    AutoSize = true,
            //    BackColor = Color.White,
            //    Text = "OK"
            //};
            //DrawPanel.Controls.Add(HelpLabel);

            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;

            //Timer for blinking of unit/viewing piece
            Timer = new System.Windows.Forms.Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            StartAnimation(AnimationType.UnitWaiting);
            t_prev = 0;
            t_now = 0;

            //Timer3 = new System.Timers.Timer(200);
            //Timer3.Elapsed += OnTimedEvent;
            //Timer3.Enabled = true;

            //sw2.Start();
            //sw3.Start();
            //cas = 0;
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            //Title
            StringFormat sf = new StringFormat();
            sf.LineAlignment = StringAlignment.Center;
            sf.Alignment = StringAlignment.Center;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString($"{Game.Civs[Data.HumanPlayer].Adjective} Map", new Font("Times New Roman", 15, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString($"{Game.Civs[Data.HumanPlayer].Adjective} Map", new Font("Times New Roman", 15, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            sf.Dispose();
            //Draw line borders of panel
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 9 + (Width - 18 - 1), 36);   //1st layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 9, 36, 9, Height - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 9 - 1, 36, Width - 9 - 1, Height - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 9, Height - 9 - 1, Width - 9 - 1, Height - 9 - 1);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 37, 9 + (Width - 18 - 2), 37);   //2nd layer of border
            e.Graphics.DrawLine(new Pen(Color.FromArgb(67, 67, 67)), 10, 37, 10, Height - 9 - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), Width - 9 - 2, 37, Width - 9 - 2, Height - 9 - 2);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(223, 223, 223)), 10, Height - 9 - 2, Width - 9 - 2, Height - 9 - 2);
            e.Dispose();
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)   //DRAW MAP
        {
            Console.WriteLine("DRAWPANEL PAINT!!!");
            sw2 = Stopwatch.StartNew();

            int[] drawingSqXY = DrawingSqXY;
            int[] edgeDrawOffsetXY = EdgeDrawOffsetXY;
            int[] startingSqXY = StartingSqXY;
            int[] drawingPxOffsetXY = DrawingPxOffsetXY;
            int[] activeXY = ActiveXY;
            int[] centerSqXY = CenterSqXY;
            int zoomLvl = ZoomLvl;

            Rectangle rect = new Rectangle(startingSqXY[0] * 32, startingSqXY[1] * 16, DrawPanel.Width, DrawPanel.Height);
            e.Graphics.DrawImage(Game.WholeMap, 0, 0, rect, GraphicsUnit.Pixel);

            //Unit/viewing piece static
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        IUnit unit = Game.Instance.ActiveUnit;
                        e.Graphics.DrawImage(AnimationBitmap[TimerCounter % 2], (unit.X - startingSqXY[0]) * 32, (unit.Y - startingSqXY[1]) * 16 - 16);
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        IUnit unit = Game.Instance.ActiveUnit;
                        e.Graphics.DrawImage(AnimationBitmap[TimerCounter], (unit.LastXY[0] - startingSqXY[0]) * 32 - 64, (unit.LastXY[1] - startingSqXY[1]) * 16 - 48);
                        break;
                    }
            }

            Console.WriteLine($"for drawing elapsed={sw2.ElapsedMilliseconds} ms"); ;
            sw2.Stop();

            #region working
            ////Draw for each visible square
            //for (int row = 0; row < drawingSqXY[1] - edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3]; row++)
            //    for (int col = 0; col < drawingSqXY[0] - edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]; col++)
            //        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
            //        {
            //            //TILES
            //            int[] coords = Ext.Civ2xy(new int[] { col + startingSqXY[0] + edgeDrawOffsetXY[0], row + startingSqXY[1] + edgeDrawOffsetXY[1] });
            //            e.Graphics.DrawImage(ModifyImage.ResizeImage(Game.Map[coords[0], coords[1]].Graphic, zoomLvl * 8, zoomLvl * 4),
            //                                 drawingPxOffsetXY[0] + 32 * col,
            //                                 drawingPxOffsetXY[1] + 16 * row);

            //            //UNITS
            //            List<IUnit> unitsHere = new List<IUnit>();    //list of all units on this tile
            //            foreach (IUnit unit in Game.Units.Where(u => u.X == col + startingSqXY[0] + edgeDrawOffsetXY[0] && u.Y == row + startingSqXY[1] + edgeDrawOffsetXY[1])) unitsHere.Add(unit);
            //            if (unitsHere.Any())
            //            {
            //                IUnit unit = unitsHere.Last();
            //                if ((unit != Game.Instance.ActiveUnit || (unit == Game.Instance.ActiveUnit && ViewingPiecesMode)) &&
            //                    UnitIsInView(unit) && !unit.IsInCity && unit.IsLastInStack)
            //                    e.Graphics.DrawImage(unit.GraphicMapPanel,
            //                                         drawingPxOffsetXY[0] + 4 * zoomLvl * (unit.X - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                                         drawingPxOffsetXY[1] + 2 * zoomLvl * (unit.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl);

            //            }

            //            //CITIES
            //            City city = Game.Cities.Find(c => c.X == col + startingSqXY[0] + edgeDrawOffsetXY[0] && c.Y == row + startingSqXY[1] + edgeDrawOffsetXY[1]);    //find if city is here
            //            if (city != null && CityIsInView(city))
            //            {
            //                e.Graphics.DrawImage(city.Graphic,
            //                                     drawingPxOffsetXY[0] + 4 * zoomLvl * (city.X - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                                     drawingPxOffsetXY[1] + 2 * zoomLvl * (city.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl);
            //                e.Graphics.DrawImage(city.TextGraphic,
            //                                     drawingPxOffsetXY[0] + 4 * zoomLvl * (city.X - startingSqXY[0] - edgeDrawOffsetXY[0]) + 4 * zoomLvl - city.TextGraphic.Width / 2,
            //                                     drawingPxOffsetXY[1] + 2 * zoomLvl * (city.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl + zoomLvl * 5);
            //            }

            //            //ACTIVE UNIT
            //            if (unitsHere.Contains(Game.Instance.ActiveUnit))
            //                if (!ViewingPiecesMode && (Game.Instance.ActiveUnit != null) && UnitIsInView(Game.Instance.ActiveUnit))
            //                {
            //                    if (AnimType == AnimationType.None && (TimerCounter % 2 == 0))
            //                    {
            //                        e.Graphics.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel,
            //                                            drawingPxOffsetXY[0] + 4 * zoomLvl * (Game.Instance.ActiveUnit.X - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                                            drawingPxOffsetXY[1] + 2 * zoomLvl * (Game.Instance.ActiveUnit.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl);
            //                    }
            //                    else if (AnimType == AnimationType.UnitMoving)
            //                    {
            //                        int moveToX = 8 * (Game.Instance.ActiveUnit.X - Game.Instance.ActiveUnit.LastXY[0]) * TimerCounter;
            //                        int moveToY = 4 * (Game.Instance.ActiveUnit.Y - Game.Instance.ActiveUnit.LastXY[1]) * TimerCounter;
            //                        e.Graphics.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel,
            //                                            drawingPxOffsetXY[0] + 4 * zoomLvl * (Game.Instance.ActiveUnit.LastXY[0] - startingSqXY[0] - edgeDrawOffsetXY[0]) + moveToX,
            //                                            drawingPxOffsetXY[1] + 2 * zoomLvl * (Game.Instance.ActiveUnit.LastXY[1] - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl + moveToY);

            //                        if (TimerCounter == 4)  //unit finished movement
            //                        {
            //                            ChangeAnimation(AnimationType.None); //reset after unit has moved
            //                            Actions.UpdateUnit(Game.Instance.ActiveUnit);
            //                        }
            //                    }
            //                }

            //            //GRID
            //            if (Options.Grid)
            //            {
            //                Color brushColor = ((col + startingSqXY[0] + edgeDrawOffsetXY[0] == centerSqXY[0]) && (row + startingSqXY[1] + edgeDrawOffsetXY[1] == centerSqXY[1])) ? Color.Red : Color.Yellow; //color central tile red;
            //                if (MapGridVar > 0)
            //                    e.Graphics.DrawImage(Images.GridLines,
            //                                         drawingPxOffsetXY[0] + 32 * col,
            //                                         drawingPxOffsetXY[1] + 16 * row);
            //                if (MapGridVar == 2)     //Map coords from SAVfile logic
            //                {
            //                    int[] realCoords = Ext.Civ2xy(new int[] { col + startingSqXY[0] + edgeDrawOffsetXY[0], row + startingSqXY[1] + edgeDrawOffsetXY[1] });
            //                    e.Graphics.DrawString(String.Format($"({realCoords[0]},{realCoords[1]})"),
            //                                          new Font("Arial", 8),
            //                                          new SolidBrush(brushColor),
            //                                          drawingPxOffsetXY[0] + 32 * col + 32,
            //                                          drawingPxOffsetXY[1] + 16 * row + 16, sf);
            //                }
            //                if (MapGridVar == 3)    //Civ2-coords
            //                    e.Graphics.DrawString(String.Format($"({col + startingSqXY[0] + edgeDrawOffsetXY[0]},{row + startingSqXY[1] + edgeDrawOffsetXY[1]})"),
            //                                          new Font("Arial", 8), new SolidBrush(brushColor),
            //                                          drawingPxOffsetXY[0] + 32 * col + 32,
            //                                          drawingPxOffsetXY[1] + 16 * row + 16, sf);
            //            }
            //        }

            ////VIEWING PIECE
            //if (ViewingPiecesMode && (TimerCounter % 2 == 0))
            //    e.Graphics.DrawImage(Images.ViewingPieces,
            //                         drawingPxOffsetXY[0] + 4 * zoomLvl * (activeXY[0] - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                         drawingPxOffsetXY[1] + 2 * zoomLvl * (activeXY[1] - startingSqXY[1] - edgeDrawOffsetXY[1]));
            #endregion

            #region Calculate time between drawings
            //long t1 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"precal elapsed={t1}");

            ////TILES
            //for (int row = 0; row < drawingSqXY[1] - edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3]; row++)
            //{
            //    for (int col = 0; col < drawingSqXY[0] - edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]; col++)
            //        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
            //        {
            //            int[] coords = Ext.Civ2xy(new int[] { col + startingSqXY[0] + edgeDrawOffsetXY[0], row + startingSqXY[1] + edgeDrawOffsetXY[1] });
            //            //Console.Write($"({coords[0]},{coords[1]}) "); //check which tiles are drawn
            //            //e.Graphics.DrawImage(ModifyImage.ResizeImage(Game.Map[coords[0], coords[1]].Graphic, zoomLvl * 8, zoomLvl * 4),
            //            //                     drawingPxOffsetXY[0] + 32 * col,
            //            //                     drawingPxOffsetXY[1] + 16 * row);
            //            e.Graphics.DrawImage(Game.Map[coords[0], coords[1]].Graphic[zoomLvl],
            //                                 drawingPxOffsetXY[0] + 32 * col,
            //                                 drawingPxOffsetXY[1] + 16 * row);
            //            //e.Graphics.DrawImage(Images.Desert[0],
            //            //                     drawingPxOffsetXY[0] + 32 * col,
            //            //                     drawingPxOffsetXY[1] + 16 * row);
            //        }
            //    //Console.Write("\n");
            //}

            //long t2 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"tiles elapsed={t2 - t1}");

            ////UNITS
            //for (int row = 0; row < drawingSqXY[1] - edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3]; row++)
            //    for (int col = 0; col < drawingSqXY[0] - edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]; col++)
            //        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
            //        {
            //            List<IUnit> unitsHere = new List<IUnit>();    //list of all units on this tile
            //            foreach (IUnit unit in Game.Units.Where(u => u.X == col + startingSqXY[0] + edgeDrawOffsetXY[0] && u.Y == row + startingSqXY[1] + edgeDrawOffsetXY[1])) unitsHere.Add(unit);
            //            if (unitsHere.Any())
            //            {
            //                IUnit unit = unitsHere.Last();
            //                if ((unit != Game.Instance.ActiveUnit || (unit == Game.Instance.ActiveUnit && ViewingPiecesMode)) &&
            //                    UnitIsInView(unit) && !unit.IsInCity && unit.IsLastInStack)
            //                    e.Graphics.DrawImage(unit.GraphicMapPanel,
            //                                         drawingPxOffsetXY[0] + 4 * zoomLvl * (unit.X - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                                         drawingPxOffsetXY[1] + 2 * zoomLvl * (unit.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl);
            //            }
            //        }

            //long t3 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"units elapsed={t3 - t2}");

            ////CITIES
            //for (int row = 0; row < drawingSqXY[1] - edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3]; row++)
            //    for (int col = 0; col < drawingSqXY[0] - edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]; col++)
            //        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
            //        {
            //            City city = Game.Cities.Find(c => c.X == col + startingSqXY[0] + edgeDrawOffsetXY[0] && c.Y == row + startingSqXY[1] + edgeDrawOffsetXY[1]);    //find if city is here
            //            if (city != null && CityIsInView(city))
            //            {
            //                e.Graphics.DrawImage(city.Graphic,
            //                                     drawingPxOffsetXY[0] + 4 * zoomLvl * (city.X - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                                     drawingPxOffsetXY[1] + 2 * zoomLvl * (city.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl);
            //                e.Graphics.DrawImage(city.TextGraphic,
            //                                     drawingPxOffsetXY[0] + 4 * zoomLvl * (city.X - startingSqXY[0] - edgeDrawOffsetXY[0]) + 4 * zoomLvl - city.TextGraphic.Width / 2,
            //                                     drawingPxOffsetXY[1] + 2 * zoomLvl * (city.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl + zoomLvl * 5);
            //            }
            //        }

            //long t4 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"cities elapsed={t4 - t3}");

            ////ACTIVE UNIT
            //for (int row = 0; row < drawingSqXY[1] - edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3]; row++)
            //    for (int col = 0; col < drawingSqXY[0] - edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]; col++)
            //        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
            //        {
            //            List<IUnit> unitsHere = new List<IUnit>();    //list of all units on this tile
            //            foreach (IUnit unit in Game.Units.Where(u => u.X == col + startingSqXY[0] + edgeDrawOffsetXY[0] && u.Y == row + startingSqXY[1] + edgeDrawOffsetXY[1])) unitsHere.Add(unit);
            //            if (unitsHere.Contains(Game.Instance.ActiveUnit))
            //            {
            //                if (!ViewingPiecesMode && (Game.Instance.ActiveUnit != null) && UnitIsInView(Game.Instance.ActiveUnit))
            //                {
            //                    if (AnimType == AnimationType.None && (TimerCounter % 2 == 0))
            //                    {
            //                        e.Graphics.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel,
            //                                            drawingPxOffsetXY[0] + 4 * zoomLvl * (Game.Instance.ActiveUnit.X - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                                            drawingPxOffsetXY[1] + 2 * zoomLvl * (Game.Instance.ActiveUnit.Y - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl);
            //                    }
            //                    else if (AnimType == AnimationType.UnitMoving)
            //                    {
            //                        int moveToX = 8 * (Game.Instance.ActiveUnit.X - Game.Instance.ActiveUnit.LastXY[0]) * TimerCounter;
            //                        int moveToY = 4 * (Game.Instance.ActiveUnit.Y - Game.Instance.ActiveUnit.LastXY[1]) * TimerCounter;
            //                        e.Graphics.DrawImage(Game.Instance.ActiveUnit.GraphicMapPanel,
            //                                            drawingPxOffsetXY[0] + 4 * zoomLvl * (Game.Instance.ActiveUnit.LastXY[0] - startingSqXY[0] - edgeDrawOffsetXY[0]) + moveToX,
            //                                            drawingPxOffsetXY[1] + 2 * zoomLvl * (Game.Instance.ActiveUnit.LastXY[1] - startingSqXY[1] - edgeDrawOffsetXY[1]) - 2 * zoomLvl + moveToY);

            //                        if (TimerCounter == 4)  //unit finished movement
            //                        {
            //                            ChangeAnimation(AnimationType.None); //reset after unit has moved
            //                            Actions.UpdateUnit(Game.Instance.ActiveUnit);
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //long t5 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"active unit elapsed={t5 - t4}");

            ////GRID
            //for (int row = 0; row < drawingSqXY[1] - edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3]; row++)
            //    for (int col = 0; col < drawingSqXY[0] - edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]; col++)
            //        if (Math.Abs(row - col) % 2 == 0)   //choose which squares
            //        {
            //            if (Options.Grid)
            //            {
            //                Color brushColor = ((col + startingSqXY[0] + edgeDrawOffsetXY[0] == centerSqXY[0]) && (row + startingSqXY[1] + edgeDrawOffsetXY[1] == centerSqXY[1])) ? Color.Red : Color.Yellow; //color central tile red;
            //                if (MapGridVar > 0)
            //                    e.Graphics.DrawImage(Images.GridLines,
            //                                         drawingPxOffsetXY[0] + 32 * col,
            //                                         drawingPxOffsetXY[1] + 16 * row);
            //                if (MapGridVar == 2)     //Map coords from SAVfile logic
            //                {
            //                    int[] realCoords = Ext.Civ2xy(new int[] { col + startingSqXY[0] + edgeDrawOffsetXY[0], row + startingSqXY[1] + edgeDrawOffsetXY[1] });
            //                    e.Graphics.DrawString(String.Format($"({realCoords[0]},{realCoords[1]})"),
            //                                          new Font("Arial", 8),
            //                                          new SolidBrush(brushColor),
            //                                          drawingPxOffsetXY[0] + 32 * col + 32,
            //                                          drawingPxOffsetXY[1] + 16 * row + 16, sf);
            //                }
            //                if (MapGridVar == 3)    //Civ2-coords
            //                    e.Graphics.DrawString(String.Format($"({col + startingSqXY[0] + edgeDrawOffsetXY[0]},{row + startingSqXY[1] + edgeDrawOffsetXY[1]})"),
            //                                          new Font("Arial", 8), new SolidBrush(brushColor),
            //                                          drawingPxOffsetXY[0] + 32 * col + 32,
            //                                          drawingPxOffsetXY[1] + 16 * row + 16, sf);
            //            }
            //        }

            //long t6 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"grid elapsed={t6 - t5}");

            ////VIEWING PIECE
            //if (ViewingPiecesMode && (TimerCounter % 2 == 0))
            //    e.Graphics.DrawImage(Images.ViewingPieces,
            //                         drawingPxOffsetXY[0] + 4 * zoomLvl * (activeXY[0] - startingSqXY[0] - edgeDrawOffsetXY[0]),
            //                         drawingPxOffsetXY[1] + 2 * zoomLvl * (activeXY[1] - startingSqXY[1] - edgeDrawOffsetXY[1]));

            //long t7 = sw1.ElapsedMilliseconds;
            //Console.WriteLine($"viewing pieces elapsed={t7 - t6}");
            #endregion

            e.Dispose();
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int[] coords = Ext.PxToCoords(e.Location.X, e.Location.Y, ZoomLvl);
            ClickedXY = new int[] { StartingSqXY[0] + coords[0], StartingSqXY[1] + coords[1] };  // coordinates of clicked square

            if (e.Button == MouseButtons.Left)
            {
                if (Game.Cities.Any(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]))    //city clicked
                {
                    if (ViewingPiecesMode) ActiveXY = ClickedXY;
                    CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]));
                    cityForm.Show();
                }
                else if (Game.Units.Any(unit => unit.X == ClickedXY[0] && unit.Y == ClickedXY[1]))    //unit clicked
                {
                    int clickedUnitIndex = Game.Units.FindIndex(a => a.X == ClickedXY[0] && a.Y == ClickedXY[1]);
                    if (!Game.Units[clickedUnitIndex].TurnEnded)
                    {
                        Game.Instance.ActiveUnit = Game.Units[clickedUnitIndex];
                        ViewingPiecesMode = false;
                    }
                    else
                    {
                        //TODO: determine what happens if unit has ended turn...
                    }
                    MapViewChange(ClickedXY);
                }
                else    //something else clicked
                {
                    if (ViewingPiecesMode) ActiveXY = ClickedXY;
                    MapViewChange(ClickedXY);
                }
            }
            else    //right click
            {
                ViewingPiecesMode = true;
                ActiveXY = ClickedXY;
                MapViewChange(ClickedXY);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            CenterSqXY = newCenterCoords;

            MapViewChangedEvent?.Invoke();  //send dimensions of current view

            DrawPanel.Refresh();
        }

        private static int[] _startingSqXY;
        public static int[] StartingSqXY
        {
            get
            {
                if (_startingSqXY == null) _startingSqXY = new int[] { 0, 0 };
                return _startingSqXY;
            }
            set
            {
                int[] drawingSqXY = DrawingSqXY;
                //limit movement so that map limits are not exceeded
                if (value[0] < 0 && value[1] < 0)    //movement beyond upper & left edge
                    _startingSqXY = new int[] { 0, 0 };
                else if ((value[0] + drawingSqXY[0] >= 2 * Data.MapXdim) && value[1] < 0)    //movement beyond upper & right edge
                    _startingSqXY = new int[] { 2 * Data.MapXdim - drawingSqXY[0], 0 };
                else if (value[0] < 0 && (value[1] + drawingSqXY[1] >= Data.MapYdim))    //movement beyond lower & left edge
                    _startingSqXY = new int[] { 0, Data.MapYdim - drawingSqXY[1] };
                else if ((value[0] + drawingSqXY[0] >= 2 * Data.MapXdim) && (value[1] + drawingSqXY[1] >= Data.MapYdim))    //movement beyond lower & right edge
                    _startingSqXY = new int[] { 2 * Data.MapXdim - drawingSqXY[0], Data.MapYdim - drawingSqXY[1] };
                else if (value[0] < 0)     //movement beyond left edge
                    _startingSqXY = new int[] { value[1] % 2, value[1] };
                else if (value[1] < 0)     //movement beyond upper edge
                    _startingSqXY = new int[] { value[0], value[0] % 2 };
                else if (value[0] + drawingSqXY[0] >= 2 * Data.MapXdim)     //movement beyond right edge
                    _startingSqXY = new int[] { 2 * Data.MapXdim - drawingSqXY[0] - value[1] % 2, value[1] };
                else if (value[1] + drawingSqXY[1] >= Data.MapYdim)     //movement beyond bottom edge
                    _startingSqXY = new int[] { value[0], Data.MapYdim - drawingSqXY[1] - value[0] % 2 };
                else
                    _startingSqXY = value;
            }
        }

        private int[] _edgeDrawOffsetXY;
        private int[] EdgeDrawOffsetXY  //determines offset to StartingSqXY for drawing of squares on panel edge { left, up, right, down }
        {
            get
            {
                int[] startingSqXY = StartingSqXY;
                int[] drawingSqXY = DrawingSqXY;
                _edgeDrawOffsetXY = new int[] { -2, -2, 2, 2 }; //by default draw 2 squares more in each direction
                if (startingSqXY[0] == 0 || startingSqXY[1] == 0)   //starting on edge
                {
                    _edgeDrawOffsetXY[0] = -Math.Max(Math.Min(startingSqXY[0], 2), 0);
                    _edgeDrawOffsetXY[1] = -Math.Max(Math.Min(startingSqXY[1], 2), 0);
                }
                if (startingSqXY[0] == 1 || startingSqXY[1] == 1)  //starting in 1st row/column
                {
                    _edgeDrawOffsetXY[0] = -1;
                    _edgeDrawOffsetXY[1] = -1;
                }
                if (startingSqXY[0] + drawingSqXY[0] == 2 * Data.MapXdim)  //on right edge
                {
                    _edgeDrawOffsetXY[2] = 0;
                    _edgeDrawOffsetXY[3] = Math.Min(Data.MapYdim - drawingSqXY[1] - startingSqXY[1], 2);
                }
                if (startingSqXY[1] + drawingSqXY[1] == Data.MapYdim)  //on bottom edge
                {
                    _edgeDrawOffsetXY[2] = Math.Min(2 * Data.MapXdim - drawingSqXY[0] - startingSqXY[0], 2);
                    _edgeDrawOffsetXY[3] = 0;
                }
                if (startingSqXY[0] + drawingSqXY[0] == 2 * Data.MapXdim - 1)  //1 column left of right edge
                {
                    _edgeDrawOffsetXY[2] = 1;
                    _edgeDrawOffsetXY[3] = Math.Min(Data.MapYdim - drawingSqXY[1] - startingSqXY[1], 2);
                }
                if (startingSqXY[1] + drawingSqXY[1] == Data.MapYdim - 1)  //1 column up of bottom edge
                {
                    _edgeDrawOffsetXY[2] = Math.Min(2 * Data.MapXdim - drawingSqXY[0] - startingSqXY[0], 2);
                    _edgeDrawOffsetXY[3] = 1;
                }
                return _edgeDrawOffsetXY;
            }
        }

        private int[] _drawingPxOffsetXY;
        private int[] DrawingPxOffsetXY   //in px
        {
            get
            {
                int[] startingSqXY = StartingSqXY;
                int[] edgeDrawOffsetXY = EdgeDrawOffsetXY;
                int[] drawingSqXY = DrawingSqXY;
                _drawingPxOffsetXY = new int[] { 32 * edgeDrawOffsetXY[0], 16 * edgeDrawOffsetXY[1] };
                if (startingSqXY[0] + drawingSqXY[0] == 2 * Data.MapXdim) _drawingPxOffsetXY[0] = DrawPanel.Width - (32 + 32 * drawingSqXY[0] - 32 * edgeDrawOffsetXY[0]);
                if (startingSqXY[1] + drawingSqXY[1] == Data.MapYdim) _drawingPxOffsetXY[1] = DrawPanel.Height - (16 + 16 * drawingSqXY[1] - 16 * edgeDrawOffsetXY[1]);
                return _drawingPxOffsetXY;
            }
        }

        private static int[] _drawingSqXY;
        public static int[] DrawingSqXY  //Squares to be drawn on the panel
        {
            //get { return new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (8 * ZoomLvl)), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (4 * ZoomLvl)) }; }
            get
            {
                _drawingSqXY = new int[] { (int)Math.Floor(((double)DrawPanel.Width - 32) / 32), (int)Math.Floor(((double)DrawPanel.Height - 16) / 16) };
                return _drawingSqXY;
            }
        }

        private int[] _centerSqXY;
        private int[] CenterSqXY
        {
            get { return _centerSqXY; }
            set
            {
                int[] centerDistanceXY = Ext.PxToCoords(DrawPanel.Width / 2, DrawPanel.Height / 2, ZoomLvl); //offset of central tile from panel NW corner
                StartingSqXY = new int[] { value[0] - centerDistanceXY[0], value[1] - centerDistanceXY[1] };
                int[] startingSqXY = StartingSqXY;
                _centerSqXY = new int[] { centerDistanceXY[0] + startingSqXY[0], centerDistanceXY[1] + startingSqXY[1] };
            }
        }

        private static int[] _activeXY;
        public static int[] ActiveXY  //Currently active box (active unit or viewing piece), civ2 coords
        {
            get
            {
                if (!ViewingPiecesMode && Game.Instance.ActiveUnit != null)
                    _activeXY = new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y };
                return _activeXY;
            }
            set { _activeXY = value; }
        }

        private int[] ClickedXY { get; set; } //tile currently clicked

        public static bool ViewingPiecesMode { get; set; }

        public int ToggleMapGrid()
        {
            MapGridVar++;
            if (MapGridVar > 3) MapGridVar = 0;
            Options.Grid = (MapGridVar != 0) ? true : false;
            Refresh();
            return MapGridVar;
        }

        private static int _zoomLvl;
        public static int ZoomLvl 
        {
            get { return _zoomLvl; }
            set 
            { 
                _zoomLvl = Math.Max(Math.Min(value, 16), 1);
                DrawPanel.Refresh();
            }
        }

        public void ZoomOUTclicked(Object sender, EventArgs e) { ZoomLvl--; }
        public void ZoomINclicked(Object sender, EventArgs e) { ZoomLvl++; }
        public void MaxZoomINclicked(Object sender, EventArgs e) { ZoomLvl = 16; }
        public void MaxZoomOUTclicked(Object sender, EventArgs e) { ZoomLvl = 1; }
        public void StandardZOOMclicked(Object sender, EventArgs e) { ZoomLvl = 8; }
        public void MediumZoomOUTclicked(Object sender, EventArgs e) { ZoomLvl = 5; }

        private void Timer_Tick(object sender, EventArgs e)
        {
            //Measure elapsed time between ticks
            t_now = sw1.ElapsedMilliseconds;
            Console.WriteLine($"elapsed between ticks={t_now - t_prev} ms, count={TimerCounter}");
            t_prev = t_now;

            int[] startingSqXY = StartingSqXY;
            int[] activeXY = ActiveXY;
            
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        //At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                        {
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        }
                        DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32, (activeXY[1] - startingSqXY[1]) * 16 - 16, 64, 48));
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32 - 64, (activeXY[1] - startingSqXY[1]) * 16 - 48, 3 * 64, 3 * 32 + 16));
                        if (TimerCounter == 7)  //Once the unit has moved, update the original world map image with image of new location of unit
                        {
                            IUnit unit = Game.Instance.ActiveUnit;
                            Game.WholeMap = ModifyImage.MergedBitmaps(Game.WholeMap, AnimationBitmap[TimerCounter], 32 * unit.LastXY[0] - 64, 16 * unit.LastXY[1] - 48);
                            StartAnimation(AnimationType.UnitWaiting);
                        }
                        break;
                    }
            }
            
            TimerCounter++;
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("The Elapsed event was raised at {0:HH:mm:ss.fff}", e.SignalTime);
            DrawPanel.Refresh();
        }

        private void MoveUnitCommand(object sender, MoveUnitCommandEventArgs e)
        {
            if (e.MoveUnit) StartAnimation(AnimationType.UnitMoving);
        }

        private void NewPlayerTurn(object sender, NewPlayerTurnEventArgs e)
        {
            if (Game.Instance.ActiveUnit != null) ViewingPiecesMode = false;
            Timer.Stop();
            TimerCounter = 0;
            Timer.Start();
        }

        private void NewUnitWasChosen(object sender, NewUnitChosenEventArgs e)
        {
            ActiveXY = new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y };
            CenterSqXY = ActiveXY;
            StartAnimation(AnimationType.UnitWaiting);
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            ViewingPiecesMode = true;
            Timer.Stop();
            TimerCounter = 0;
            Timer.Start();
        }

        //If ENTER pressed when view piece above city --> enter city view
        private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        {
            if (ViewingPiecesMode && Game.Cities.Any(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]))
            {
                CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
                cityForm.Show();
            }
        }

        //private bool UnitIsInView(IUnit unit)   //Determine if unit can be seen in current map view
        //{
        //    int[] startingSqXY = StartingSqXY;
        //    int[] edgeDrawOffsetXY = EdgeDrawOffsetXY;
        //    int[] drawingSqXY = DrawingSqXY;
        //    bool isInView;
        //    if ((unit.X >= startingSqXY[0] + edgeDrawOffsetXY[0]) && 
        //        (unit.X <= startingSqXY[0] + drawingSqXY[0] + edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]) &&
        //        (unit.Y >= startingSqXY[1] + edgeDrawOffsetXY[1]) &&
        //        (unit.Y <= startingSqXY[1] + drawingSqXY[1] + edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3])) isInView = true;
        //    else isInView = false;
        //    return isInView;
        //}

        //private bool CityIsInView(City city)   //Determine if city can be seen in current map view
        //{
        //    int[] startingSqXY = StartingSqXY;
        //    int[] edgeDrawOffsetXY = EdgeDrawOffsetXY;
        //    int[] drawingSqXY = DrawingSqXY;
        //    bool isInView;
        //    if ((city.X >= startingSqXY[0] + edgeDrawOffsetXY[0]) &&
        //        (city.X <= startingSqXY[0] + drawingSqXY[0] + edgeDrawOffsetXY[0] + edgeDrawOffsetXY[2]) &&
        //        (city.Y >= startingSqXY[1] + edgeDrawOffsetXY[1]) &&
        //        (city.Y <= startingSqXY[1] + drawingSqXY[1] + edgeDrawOffsetXY[1] + edgeDrawOffsetXY[3])) isInView = true;
        //    else isInView = false;
        //    return isInView;
        //}

        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.UnitWaiting:
                    AnimType = AnimationType.UnitWaiting;
                    Timer.Stop();
                    //AnimationBitmap = Images.GetAnimationFrames(anim);
                    AnimationBitmap = Images.GetAnimationFrames_UnitWaiting();
                    TimerCounter = 0;
                    Timer.Interval = 200;    //ms                    
                    Timer.Start();
                    break;
                case AnimationType.UnitMoving:
                    AnimType = AnimationType.UnitMoving;
                    Timer.Stop();
                    //AnimationBitmap = Images.GetAnimationFrames(anim);
                    AnimationBitmap = Images.GetAnimationFrames_UnitMoving();
                    TimerCounter = 0;
                    Timer.Interval = 25;    //ms
                    Timer.Start();
                    break;
            }            
        }
    }
}
