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
using RTciv2.GameActions;

namespace RTciv2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        private static DoubleBufferedPanel DrawPanel;
        StringFormat sf = new StringFormat();

        private static List<Bitmap> AnimationBitmap;        
        private int MapGridVar { get; set; }    //style of map grid presentation
        private System.Windows.Forms.Timer Timer;    //timer for blinking (unit or viewing piece), moving unit, etc.
        private static AnimationType AnimType;
        private int CivIdWhoseMapIsDisplayed { get; set; }
        int TimerCounter { get; set; }
        Label HelpLabel;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public MapPanel(int width, int height)
        {
            Size = new Size(width, height);
            this.Paint += new PaintEventHandler(MapPanel_Paint);

            Actions.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Actions.OnUnitEvent += UnitEventHappened;
            Actions.OnPlayerEvent += PlayerEventHappened;
            MinimapPanel.OnMapEvent += MapEventHappened;
            StatusPanel.OnMapEvent += MapEventHappened;
            MainCiv2Window.OnMapEvent += MapEventHappened;
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
            ViewPiecesMode = (Game.Instance.ActiveUnit == null) ? true : false;  //if no unit is active at start --> all units ended turn
            ClickedXY = Data.ClickedXY;
            ActiveXY = Data.ActiveXY;
            CenterSqXY = ActiveXY;
            CivIdWhoseMapIsDisplayed = Game.Instance.ActiveCiv.Id;  //when game starts reveal map for current player's civ view
            //TODO: when game starts make sure revealed map is either for current player's civ view or whole map is revealed
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

            //Timer for waiting unit/viewing piece
            Timer = new System.Windows.Forms.Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            StartAnimation(AnimationType.UnitWaiting);
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
            int[] drawingSqXY = DrawingSqXY;
            int[] edgeDrawOffsetXY = EdgeDrawOffsetXY;
            int[] startingSqXY = StartingSqXY;
            int[] drawingPxOffsetXY = DrawingPxOffsetXY;
            int[] activeXY = ActiveXY;
            int[] centerSqXY = CenterSqXY;
            int zoomLvl = ZoomLvl;

            Rectangle rect = new Rectangle(startingSqXY[0] * 32, startingSqXY[1] * 16, DrawPanel.Width, DrawPanel.Height);
            e.Graphics.DrawImage(Game.CivsMap[CivIdWhoseMapIsDisplayed], 0, 0, rect, GraphicsUnit.Pixel);

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
                        e.Graphics.DrawImage(AnimationBitmap[Game.Instance.ActiveUnit.MovementCounter], (unit.LastXY[0] - startingSqXY[0]) * 32 - 64, (unit.LastXY[1] - startingSqXY[1]) * 16 - 48);
                        break;
                    }
                case AnimationType.ViewPieces:
                    {
                        if (TimerCounter % 2 == 0) e.Graphics.DrawImage(Images.ViewPiece, 32 * (ActiveXY[0] - startingSqXY[0]), 16 * (ActiveXY[1] - startingSqXY[1]), 64, 32);
                        break;
                    }
            }

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
                    if (ViewPiecesMode) ActiveXY = ClickedXY;
                    CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]));
                    cityForm.Show();
                }
                else if (Game.Units.Any(unit => unit.X == ClickedXY[0] && unit.Y == ClickedXY[1]))    //unit clicked
                {
                    int clickedUnitIndex = Game.Units.FindIndex(a => a.X == ClickedXY[0] && a.Y == ClickedXY[1]);
                    if (!Game.Units[clickedUnitIndex].TurnEnded)
                    {
                        Game.Instance.ActiveUnit = Game.Units[clickedUnitIndex];
                        ViewPiecesMode = false;
                        OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
                        StartAnimation(AnimationType.UnitWaiting);
                    }
                    else
                    {
                        //TODO: determine what happens if unit has ended turn...
                    }
                    MapViewChange(ClickedXY);
                }
                else    //something else clicked
                {
                    if (ViewPiecesMode) ActiveXY = ClickedXY;
                    MapViewChange(ClickedXY);
                }
            }
            else    //right click
            {
                ViewPiecesMode = true;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
                ActiveXY = ClickedXY;
                MapViewChange(ClickedXY);
                StartAnimation(AnimationType.ViewPieces);                
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            CenterSqXY = newCenterCoords;

            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged));    

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
                if (!ViewPiecesMode && Game.Instance.ActiveUnit != null)
                    _activeXY = new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y };
                return _activeXY;
            }
            set { _activeXY = value; }
        }

        private int[] ClickedXY { get; set; } //tile currently clicked

        public static bool ViewPiecesMode { get; set; }

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
            DrawAnimation();
            TimerCounter++;
        }

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        MapViewChange(e.CenterSqXY);
                        break;
                    }
                case MapEventType.SwitchViewMovePieces:
                    {
                        if (ViewPiecesMode) StartAnimation(AnimationType.ViewPieces);
                        else StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case MapEventType.ViewPieceMoved:
                    {
                        StartAnimation(AnimationType.ViewPieces);
                        break;
                    }
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        if (CivIdWhoseMapIsDisplayed == Game.Instance.ActiveCiv.Id) CivIdWhoseMapIsDisplayed = 8;   //show entire map
                        else if (CivIdWhoseMapIsDisplayed == 8) CivIdWhoseMapIsDisplayed = Game.Instance.ActiveCiv.Id;   //show current civ's map view
                        DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        Update();
                        break;
                    }
                default: break;
            }
        }

        private void PlayerEventHappened(object sender, PlayerEventArgs e)
        {
            switch (e.EventType)
            {
                case PlayerEventType.NewTurn:
                    {
                        if (Game.Instance.ActiveUnit != null) ViewPiecesMode = false;
                        Timer.Stop();
                        TimerCounter = 0;
                        Timer.Start();
                        break;
                    }
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            switch (e.EventType)
            {
                //Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        TimerCounter = Game.Instance.ActiveUnit.MovementCounter;
                        if (TimerCounter == 0) StartAnimation(AnimationType.UnitMoving);
                        DrawAnimation();
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        ActiveXY = new int[] { Game.Instance.ActiveUnit.X, Game.Instance.ActiveUnit.Y };
                        CenterSqXY = ActiveXY;
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
            }
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            ViewPiecesMode = true;
            Timer.Stop();
            TimerCounter = 0;
            Timer.Start();
        }

        private void CheckIfUnitMovedOutsideMapView()
        {
            if (ActiveXY[0] >= StartingSqXY[0] + DrawingSqXY[0] ||
                ActiveXY[0] <= StartingSqXY[0] ||
                ActiveXY[1] >= StartingSqXY[1] + DrawingSqXY[1] ||
                ActiveXY[1] <= StartingSqXY[1])
            {
                CenterSqXY = ActiveXY;
                DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                Update();
            }
        }

        //If ENTER pressed when view piece above city --> enter city view
        private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        {
            if (ViewPiecesMode && Game.Cities.Any(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]))
            {
                CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
                cityForm.Show();
            }
        }

        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.None:
                    Timer.Stop();
                    TimerCounter = 0;
                    break; 
                case AnimationType.UnitWaiting:
                    AnimType = AnimationType.UnitWaiting;
                    Timer.Stop();
                    AnimationBitmap = GetAnimationFrames.UnitWaiting();
                    TimerCounter = 0;
                    Timer.Interval = 200;    //ms                    
                    Timer.Start();
                    break;
                case AnimationType.UnitMoving:
                    AnimType = AnimationType.UnitMoving;
                    AnimationBitmap = GetAnimationFrames.UnitMoving();
                    break;
                case AnimationType.ViewPieces:
                    AnimType = AnimationType.ViewPieces;
                    Timer.Stop();
                    TimerCounter = 0;
                    Timer.Interval = 200;    //ms                    
                    Timer.Start();
                    break;
            }            
        }

        private void DrawAnimation()
        {
            int[] startingSqXY = StartingSqXY;
            int[] activeXY = ActiveXY;

            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        //At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32, (activeXY[1] - startingSqXY[1]) * 16 - 16, 64, 48));
                        Update();
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32 - 64, (activeXY[1] - startingSqXY[1]) * 16 - 48, 3 * 64, 3 * 32 + 16));
                        Update();
                        if (TimerCounter == 7)  //Unit has completed movement
                        {
                            //Update the original world map image with image of new location of unit & redraw whole map
                            IUnit unit = Game.Instance.ActiveUnit;
                            Game.CivsMap[Game.Instance.ActiveCiv.Id] = ModifyImage.MergedBitmaps(Game.CivsMap[Game.Instance.ActiveCiv.Id], AnimationBitmap[TimerCounter], 32 * unit.LastXY[0] - 64, 16 * unit.LastXY[1] - 48);
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                            Update();

                            //Then stop animation
                            StartAnimation(AnimationType.None);

                            //Check if unit moved outside map view -> map view needs to be updated
                            CheckIfUnitMovedOutsideMapView();
                        }
                        break;
                    }
                case AnimationType.ViewPieces:
                    {
                        //At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle((activeXY[0] - startingSqXY[0]) * 32, (activeXY[1] - startingSqXY[1]) * 16, 64, 32));
                        Update();
                        break;
                    }
            }
        }
    }
}
