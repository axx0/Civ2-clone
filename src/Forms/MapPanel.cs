using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Timers;
using ExtensionMethods;
using civ2.Events;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Enums;

namespace civ2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        Game Game => Game.Instance;
        Map Map => Map.Instance;

        private static List<Bitmap> AnimationBitmap;
        private int MapGridVar { get; set; }        // Style of map grid presentation
        private System.Windows.Forms.Timer Timer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        private AnimationType AnimType;
        private int TimerCounter { get; set; }

        private int[] PanelOffsetXY, MapOffsetXY, CentralXY;
        private Rectangle mapRect1, mapRect2;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        public MapPanel(int _width, int _height) : base(_width, _height, "", false)
        {
            this.Paint += new PaintEventHandler(MapPanel_Paint);

            Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnUnitEvent += UnitEventHappened;
            Game.OnPlayerEvent += PlayerEventHappened;
            MinimapPanel.OnMapEvent += MapEventHappened;
            StatusPanel.OnMapEvent += MapEventHappened;
            MainWindow.OnMapEvent += MapEventHappened;
            MainWindow.OnCheckIfCityCanBeViewed += CheckIfCityCanBeViewed;

            NoSelectButton ZoomINButton = new NoSelectButton
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                //BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 23, 23)
                BackgroundImage = Images.ZoomIN
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            NoSelectButton ZoomOUTButton = new NoSelectButton
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                //BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 23, 23)
                BackgroundImage = Images.ZoomOUT
            };
            ZoomOUTButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomOUTButton);
            ZoomOUTButton.Click += ZoomOUTclicked;

            // Add DrawPanel from base control
            Controls.Add(DrawPanel);
            DrawPanel.BackgroundImage = null;
            DrawPanel.BackColor = Color.Black;
            DrawPanel.Paint += DrawPanel_Paint;
            DrawPanel.MouseClick += DrawPanel_MouseClick;

            // Center the map view and draw map
            MapGridVar = 0;
            MapViewChange(Game.ClickedXY);
            StartAnimation(AnimationType.UpdateMap);

            ViewPiecesMode = Game.ActiveUnit == null;  // If no unit is active at start (all units ended turn or no exist) go to View pieces mode
            if (ViewPiecesMode)
            {
                //ActiveXY = Game.ActiveCursorXY; // If NOT in ViewPieceMode, then ActiveXY will be set equal to currently active unit coords.
                AnimType = AnimationType.ViewPieces;
            }
            else
            {
                AnimType = AnimationType.UnitWaiting;
            }

            // Timer for waiting unit/ viewing piece
            Timer = new System.Windows.Forms.Timer();
            Timer.Tick += new EventHandler(Timer_Tick);
            StartAnimation(AnimType);
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            StringFormat sf = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.Black), new Point(this.Width / 2 + 1, 20 + 1), sf);
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(this.Width / 2, 20), sf);
            e.Dispose();
            sf.Dispose();
        }

        // Draw map here
        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Calculate these once just for less computations
            //int[] startingSqXY = StartingSqXY;
            //int[] startingSqXYpx = StartingSqXYpx;
            int[] startingSqXYpx = new int[] { 0, 0 };
            //int[] activeXY = ActiveXY;
            int[] activeXYpx = ActiveXYpx;

            // Draw animation
            switch (AnimType)
            {
                case AnimationType.UpdateMap:
                    {
                        // Draw map (for round world draw it from 2 parts)
                        e.Graphics.DrawImage(Map.ActiveCivMap, PanelOffsetXYpx[0], PanelOffsetXYpx[1], mapRect1, GraphicsUnit.Pixel);
                        e.Graphics.DrawImage(Map.ActiveCivMap, PanelOffsetXYpx[0] + mapRect1.Width, PanelOffsetXYpx[1], mapRect2, GraphicsUnit.Pixel);

                        // For each map tile on screen draw cities & units


                        break;
                    }
                case AnimationType.UnitWaiting:
                    {
                        e.Graphics.DrawImage(AnimationBitmap[TimerCounter % 2], ActiveXYpx[0], ActiveXYpx[1]);
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        IUnit unit = Game.ActiveUnit;
                        e.Graphics.DrawImage(AnimationBitmap[Game.ActiveUnit.MovementCounter], unit.LastXYpx[0] - startingSqXYpx[0] - 8 * (Game.Zoom + 8), unit.LastXYpx[1] - startingSqXYpx[1] - 4 * (Game.Zoom + 8));
                        break;
                    }
                case AnimationType.ViewPieces:
                    {
                        if (TimerCounter % 2 == 0) e.Graphics.DrawImage(Images.ViewPiece, activeXYpx[0] - startingSqXYpx[0], activeXYpx[1] - startingSqXYpx[1]);
                        break;
                    }
            }

            e.Dispose();
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            int[] coords = Ext.PxToCoords(e.Location.X, e.Location.Y, Game.Zoom);
            Game.ClickedXY = new int[] { (MapOffsetXY[0] + coords[0]) % (2 * Map.Xdim), MapOffsetXY[1] + coords[1] };  // Coordinates of clicked square

            if (e.Button == MouseButtons.Left)
            {
                if (Game.GetCities.Any(city => city.X == Game.ClickedXY[0] && city.Y == Game.ClickedXY[1]))    // City clicked
                {
                    if (ViewPiecesMode) Game.ActiveCursorXY = Game.ClickedXY;
                    //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]));
                    //cityForm.Show();
                }
                else if (Game.GetUnits.Any(unit => unit.X == Game.ClickedXY[0] && unit.Y == Game.ClickedXY[1]))    // Unit clicked
                {
                    int clickedUnitIndex = Game.GetUnits.FindIndex(a => a.X == Game.ClickedXY[0] && a.Y == Game.ClickedXY[1]);
                    if (!Game.GetUnits[clickedUnitIndex].TurnEnded)
                    {
                        Game.ActiveUnit = Game.GetUnits[clickedUnitIndex];
                        ViewPiecesMode = false;
                        OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
                        StartAnimation(AnimationType.UnitWaiting);
                    }
                    else
                    {
                        //TODO: determine what happens if unit has ended turn...
                    }
                    MapViewChange(Game.ClickedXY);
                }
                else    // Something else clicked
                {
                    if (ViewPiecesMode) Game.ActiveCursorXY = Game.ClickedXY;
                    MapViewChange(Game.ClickedXY);
                }
            }
            else    // Right click
            {
                ViewPiecesMode = true;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePieces));
                Game.ActiveCursorXY = Game.ClickedXY;
                MapViewChange(Game.ClickedXY);
                StartAnimation(AnimationType.ViewPieces);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            //CenterSqXY = newCenterCoords;
            ReturnCoordsAtMapViewChange(newCenterCoords);

            //OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, StartingSqXY, DrawingSqXY));

            DrawPanel.Refresh();
        }

        #region Calculation of coordinates
        // Coordinates in DrawPanel upper left corner where map drawing begins
        private int[] _startingSqXY;
        public int[] StartingSqXY
        {
            get
            {
                int[] centerDistanceXY = Ext.PxToCoords(DrawPanel.Width / 2, DrawPanel.Height / 2, Game.Zoom); // Offset of central tile from panel NW corner
                int[] _centerSqXY = CenterSqXY;
                _startingSqXY = new int[] { _centerSqXY[0] - centerDistanceXY[0], _centerSqXY[1] - centerDistanceXY[1] };

                //if (!Game.Options.FlatEarth)    // Round world --> make sure starting X is never < 0
                //{
                //    if (_startingSqXY[0] < 0)
                //    {
                //        _startingSqXY[0] = _startingSqXY[0] % 
                //    }
                //}

                return _startingSqXY; ;
            }
        }



        //// Determines offset to StartingSqXY for drawing of squares on panel edge { left, up, right, down }
        //private int[] _edgePxDrawOffsetXY;
        //private int[] EdgePxDrawOffsetXY
        //{
        //    get
        //    {
        //        int[] startingSqXY = StartingSqXY;
        //        int[] drawingSqXY = DrawingSqXY;
        //        int[] _edgeDrawOffsetXY = new int[] { -2, -2, 2, 2 };     // By default draw 2 squares more in each direction
        //        if (startingSqXY[0] == 0 || startingSqXY[1] == 0)   // Starting on edge
        //        {
        //            _edgeDrawOffsetXY[0] = -Math.Max(Math.Min(startingSqXY[0], 2), 0);
        //            _edgeDrawOffsetXY[1] = -Math.Max(Math.Min(startingSqXY[1], 2), 0);
        //        }
        //        if (startingSqXY[0] == 1 || startingSqXY[1] == 1)  // Starting in 1st row/column
        //        {
        //            _edgeDrawOffsetXY[0] = -1;
        //            _edgeDrawOffsetXY[1] = -1;
        //        }
        //        if (startingSqXY[0] + drawingSqXY[0] == 2 * Map.Xdim)  // On right edge
        //        {
        //            _edgeDrawOffsetXY[2] = 0;
        //            _edgeDrawOffsetXY[3] = Math.Min(Map.Ydim - drawingSqXY[1] - startingSqXY[1], 2);
        //        }
        //        if (startingSqXY[1] + drawingSqXY[1] == Map.Ydim)  // On bottom edge
        //        {
        //            _edgeDrawOffsetXY[2] = Math.Min(2 * Map.Xdim - drawingSqXY[0] - startingSqXY[0], 2);
        //            _edgeDrawOffsetXY[3] = 0;
        //        }
        //        if (startingSqXY[0] + drawingSqXY[0] == 2 * Map.Xdim - 1)  // 1 column left of right edge
        //        {
        //            _edgeDrawOffsetXY[2] = 1;
        //            _edgeDrawOffsetXY[3] = Math.Min(Map.Ydim - drawingSqXY[1] - startingSqXY[1], 2);
        //        }
        //        if (startingSqXY[1] + drawingSqXY[1] == Map.Ydim - 1)  // 1 column up of bottom edge
        //        {
        //            _edgeDrawOffsetXY[2] = Math.Min(2 * Map.Xdim - drawingSqXY[0] - startingSqXY[0], 2);
        //            _edgeDrawOffsetXY[3] = 1;
        //        }

        //        // Now convert it to pixels
        //        _edgePxDrawOffsetXY = new int[] { 32 * _edgeDrawOffsetXY[0], 16 * _edgeDrawOffsetXY[1] };
        //        if (startingSqXY[0] + drawingSqXY[0] == 2 * Map.Xdim)
        //            _edgePxDrawOffsetXY[0] = DrawPanel.Width - (32 + 32 * drawingSqXY[0] - 32 * _edgeDrawOffsetXY[0]);
        //        if (startingSqXY[1] + drawingSqXY[1] == Map.Ydim)
        //            _edgePxDrawOffsetXY[1] = DrawPanel.Height - (16 + 16 * drawingSqXY[1] - 16 * _edgeDrawOffsetXY[1]);

        //        return _edgePxDrawOffsetXY;
        //    }
        //}

        // Squares to be drawn on the panel
        private int[] DrawingSqXY => new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (8 * (8 + Game.Zoom))), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (4 * (8 + Game.Zoom))) };

        // Center square on the map
        private int[] _centerSqXY;
        private int[] CenterSqXY
        {
            get { return _centerSqXY; }
            set
            {
                int[] centerDistanceXY = Ext.PxToCoords(DrawPanel.Width / 2, DrawPanel.Height / 2, Game.Zoom); // Offset of central tile from panel NW corner
                int[] _startingSqXY = new int[] { value[0] - centerDistanceXY[0], value[1] - centerDistanceXY[1] };
                int[] drawingSqXY = DrawingSqXY;



                // Limit movement so that map limits are not exceeded
                if (Game.Options.FlatEarth)
                {
                    if (_startingSqXY[0] < 0 && _startingSqXY[1] < 0)    // Movement beyond upper & left edge
                        _startingSqXY = new int[] { 0, 0 };
                    else if ((_startingSqXY[0] + drawingSqXY[0] >= 2 * Map.Xdim) && _startingSqXY[1] < 0)    // Movement beyond upper & right edge
                        _startingSqXY = new int[] { 2 * Map.Xdim - drawingSqXY[0], 0 };
                    else if (_startingSqXY[0] < 0 && (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim))    // Movement beyond lower & left edge
                        _startingSqXY = new int[] { 0, Map.Ydim - drawingSqXY[1] };
                    else if ((_startingSqXY[0] + drawingSqXY[0] >= 2 * Map.Xdim) && (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim))    // Movement beyond lower & right edge
                        _startingSqXY = new int[] { 2 * Map.Xdim - drawingSqXY[0], Map.Ydim - drawingSqXY[1] };
                    else if (_startingSqXY[0] < 0)     // Movement beyond left edge
                        _startingSqXY = new int[] { _startingSqXY[1] % 2, _startingSqXY[1] };
                    else if (_startingSqXY[1] < 0)     // Movement beyond upper edge
                        _startingSqXY = new int[] { _startingSqXY[0], _startingSqXY[0] % 2 };
                    else if (_startingSqXY[0] + drawingSqXY[0] >= 2 * Map.Xdim)     // Movement beyond right edge
                        _startingSqXY = new int[] { 2 * Map.Xdim - drawingSqXY[0] - _startingSqXY[1] % 2, _startingSqXY[1] };
                    else if (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim)     // Movement beyond bottom edge
                        _startingSqXY = new int[] { _startingSqXY[0], Map.Ydim - drawingSqXY[1] - _startingSqXY[0] % 2 };
                }
                else    // ROUND EARTH
                {
                    // Check only movement beyond upper and lower edge
                    if (_startingSqXY[1] < 0)    // Upper edge
                        _startingSqXY[1] = 0;
                    else if (_startingSqXY[1] + drawingSqXY[1] >= Map.Ydim)    // Lower edge
                        _startingSqXY[1] = Map.Ydim - drawingSqXY[1];
                }

                _centerSqXY = new int[] { centerDistanceXY[0] + _startingSqXY[0], centerDistanceXY[1] + _startingSqXY[1] };
            }
        }

        // Currently active box (active unit or viewing piece), civ2 coords
        //private int[] _activeXY;
        //public int[] ActiveXY
        //{
        //    get
        //    {
        //        if (Game.ActiveUnit != null)
        //            return new int[] { Game.ActiveUnit.X, Game.ActiveUnit.Y };
        //        else
        //            return _activeXY;
        //    }
        //    set { _activeXY = value; }
        //}

        // Representation in pixels for drawing
        private int[] StartingSqXYpx => new int[] { StartingSqXY[0] * 4 * (8 + Game.Zoom), StartingSqXY[1] * 2 * (8 + Game.Zoom) };
        private int MapXdimPx => 2 * Map.Xdim * 4 * (8 + Game.Zoom);
        private int MapYdimPx => Map.Ydim * 2 * (8 + Game.Zoom);
        private int[] ActiveXYpx => new int[] { 4 * (Game.Zoom + 8) * (Game.ActiveCursorXY[0] - MapOffsetXYpx[0]), 2 * (Game.Zoom + 8) * (Game.ActiveCursorXY[1] - MapOffsetXYpx[1]) };
        #endregion

        public static bool ViewPiecesMode { get; set; }

        public int ToggleMapGrid()
        {
            MapGridVar++;
            if (MapGridVar > 3) MapGridVar = 0;
            //Options.Grid = (MapGridVar != 0) ? true : false;
            Refresh();
            return MapGridVar;
        }

        #region ZoomInOut events
        public void ZoomOUTclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom--;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh(); 
        }
        public void ZoomINclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom++;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh(); 
        }
        public void MaxZoomINclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom = 16;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh(); 
        }
        public void MaxZoomOUTclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom = 1;
            Map.SetNewActiveMapPic(); 
            DrawPanel.Refresh(); 
        }
        public void StandardZOOMclicked(Object sender, EventArgs e) 
        { 
            Game.Zoom = 8;
            Map.SetNewActiveMapPic(); 
            DrawPanel.Refresh(); 
        }
        public void MediumZoomOUTclicked(Object sender, EventArgs e) 
        {
            Game.Zoom = 5;
            Map.SetNewActiveMapPic();
            DrawPanel.Refresh();
        }
        #endregion

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
                        if (Game.ActiveUnit != null) ViewPiecesMode = false;
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
                // Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                    {
                        TimerCounter = Game.ActiveUnit.MovementCounter;
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
                        Game.ActiveCursorXY = new int[] { Game.ActiveUnit.X, Game.ActiveUnit.Y };
                        CenterSqXY = Game.ActiveCursorXY;
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

        // If ENTER pressed when view piece above city --> enter city view
        private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        {
            if (ViewPiecesMode && Game.GetCities.Any(city => city.X == Game.ActiveCursorXY[0] && city.Y == Game.ActiveCursorXY[1]))
            {
                //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
                //cityForm.Show();
            }
        }

        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.UpdateMap:
                    Timer.Stop();
                    TimerCounter = 0;
                    DrawPanel.Invalidate();
                    break;
                case AnimationType.UnitWaiting:
                    //AnimType = AnimationType.UnitWaiting;
                    Timer.Stop();
                    AnimationBitmap = GetAnimationFrames.UnitWaiting();
                    TimerCounter = 0;
                    Timer.Interval = 200;    // ms                    
                    Timer.Start();
                    break;
                case AnimationType.UnitMoving:
                    //AnimType = AnimationType.UnitMoving;
                    AnimationBitmap = GetAnimationFrames.UnitMoving();
                    break;
                case AnimationType.ViewPieces:
                    //AnimType = AnimationType.ViewPieces;
                    Timer.Stop();
                    TimerCounter = 0;
                    Timer.Interval = 200;    // ms                    
                    Timer.Start();
                    break;
            }
        }

        private void DrawAnimation()
        {
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            //DrawPanel.Invalidate(new Rectangle(ActiveXYpx[0], ActiveXYpx[1] - 2 * (Game.Zoom + 8), 8 * (Game.Zoom + 8), 6 * (Game.Zoom + 8)));
                            DrawPanel.Invalidate(new Rectangle(0, 0, 64, 48));
                        Update();
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        DrawPanel.Invalidate(new Rectangle((Game.ActiveCursorXY[0] - MapOffsetXY[0]) * 32 - 64, (Game.ActiveCursorXY[1] - MapOffsetXY[1]) * 16 - 48, 3 * 64, 3 * 32 + 16));
                        Update();
                        if (TimerCounter == 7)  // Unit has completed movement
                        {
                            // First update world map with new visible tiles
                            Game.UpdateWorldMapAfterUnitHasMoved();

                            // Update the original world map image with image of new location of unit & redraw whole map
                            IUnit unit = Game.Instance.ActiveUnit;
                            // Game.CivsMap[Game.Instance.ActiveCiv.Id] = ModifyImage.MergedBitmaps(Game.CivsMap[Game.Instance.ActiveCiv.Id], AnimationBitmap[TimerCounter], 32 * unit.LastXY[0] - 64, 16 * unit.LastXY[1] - 48);
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                            Update();

                            // Then stop animation
                            StartAnimation(AnimationType.UpdateMap);

                            // Check if unit moved outside map view -> map view needs to be updated
                            if (UnitMovedOutsideMapView)
                            {
                                CenterSqXY = Game.ActiveCursorXY;
                                DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                                Update();
                            }
                        }
                        break;
                    }
                case AnimationType.ViewPieces:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (TimerCounter == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle((Game.ActiveCursorXY[0] - MapOffsetXY[0]) * 32, (Game.ActiveCursorXY[1] - MapOffsetXY[1]) * 16, 64, 32));
                        Update();
                        break;
                    }
            }
        }

        private bool UnitMovedOutsideMapView
        {
            get
            {
                if (Game.ActiveCursorXY[0] >= StartingSqXY[0] + DrawingSqXY[0] ||
                Game.ActiveCursorXY[0] <= StartingSqXY[0] ||
                Game.ActiveCursorXY[1] >= StartingSqXY[1] + DrawingSqXY[1] ||
                Game.ActiveCursorXY[1] <= StartingSqXY[1])
                    return true;
                else
                    return false;
            }
        }

        // Function which sets various variables for drawing map on grid
        private void ReturnCoordsAtMapViewChange(int[] proposedCentralCoords)
        {
            CentralXY = proposedCentralCoords;    // Central point of Draw Panel (in map's coordinates)
            PanelOffsetXY = new int[] { 0, 0 }; // Offset of NW point of panel from maps NW point (=0 if map is larger than panel)
            MapOffsetXY = new int[] { 0, 0 }; // Offset of map NW point from panel NW point, in squares (=0 if panel is larger than map in any direction)
            mapRect1 = new Rectangle(0, 0, 0, 0);  // Rectangle for drawing 1st part of map
            mapRect2 = new Rectangle(0, 0, 0, 0);  // Rectangle for drawing 2nd part of map

            int mapWidth = 4 * (8 + Game.Zoom) * (2 * Map.Xdim + 1);
            int mapHeight = 2 * (8 + Game.Zoom) * (Map.Ydim + 1);

            // No of squares of panel and map
            int[] PanelSq = new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (8 * (8 + Game.Zoom))), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (4 * (8 + Game.Zoom))) };
            int[] MapSq = new int[] { 2 * Map.Xdim + 1, Map.Ydim + 1 };

            // First determine the Y-central coordinate
            if (PanelSq[1] > MapSq[1])    // Panel is larger than map in Y, center the map in panel center
            {
                PanelOffsetXY[1] = (PanelSq[1] - MapSq[1]) / 2;
                CentralXY[1] = MapSq[1] / 2;
                mapRect1.Height = mapHeight;
            }
            else    // Map higher than panel
            {
                if (CentralXY[1] < PanelSq[1] / 2)    // Limit Drawing Panel so it's not going beyond the map in north
                {
                    CentralXY[1] = PanelSq[1] / 2;
                }
                else if (CentralXY[1] > MapSq[1] - PanelSq[1] / 2)  // Limit Drawing Panel so it's not going below the map in south
                {
                    MapOffsetXY[1] = MapSq[1] - PanelSq[1];
                    CentralXY[1] = MapOffsetXY[1] + PanelSq[1] / 2;
                }
                else    // Drawing panel within map (Y-axis)
                {
                    MapOffsetXY[1] = CentralXY[1] - PanelSq[1] / 2;
                }
                mapRect1.Height = DrawPanel.Height;
            }
            mapRect1.Y = 2 * (8 + Game.Zoom) * MapOffsetXY[1];

            // Then determine X-coordinate
            if (PanelSq[0] > MapSq[0])    // Panel is larger than map in X, center the map in panel center
            {
                PanelOffsetXY[0] = (PanelSq[0] - MapSq[0]) / 2;
                CentralXY[0] = MapSq[0] / 2;
                mapRect1.X = 0;
                mapRect1.Width = mapWidth;

                // If the tile at these coords doesn't exist, shift X to the right
                if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;
            }
            else    // Map wider than panel
            {
                if (Game.Options.FlatEarth)
                {
                    if (CentralXY[0] < PanelSq[1] / 2)  // Limit Drawing Panel so it's not going beyond the map in west
                    {
                        CentralXY[0] = PanelSq[0] / 2;
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                        if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;                        
                    }
                    else if (CentralXY[0] > MapSq[0] - PanelSq[0] / 2)  // Limit Drawing Panel so it's not going below the map in east
                    {
                        CentralXY[0] = MapSq[0] - PanelSq[0] / 2;
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                        if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;

                        MapOffsetXY[0] = CentralXY[0] - PanelSq[0] / 2;
                    }
                    else    // Drawing panel within map (X-axis)
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                        if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;

                        MapOffsetXY[0] = CentralXY[0] - PanelSq[0] / 2;
                    }
                    mapRect1.Width = DrawPanel.Width;
                    mapRect1.X = 4 * (8 + Game.Zoom) * MapOffsetXY[0];
                }
                else    // Round world
                {
                    if (CentralXY[0] < PanelSq[1] / 2)  // Map reaches west of X=0 axis
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                        if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;

                        MapOffsetXY[0] = MapSq[0] + (CentralXY[0] - PanelSq[0] / 2);
                        mapRect1.X = 4 * (8 + Game.Zoom) * MapOffsetXY[0];
                        mapRect1.Width = mapWidth - mapRect1.X;
                        mapRect2.X = 4 * (8 + Game.Zoom);
                        mapRect2.Width = DrawPanel.Width - mapRect1.Width;

                    }
                    else if (CentralXY[0] + PanelSq[0] / 2 > MapSq[0])  // Panel beyond map eastern edge
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                        if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;

                        MapOffsetXY[0] = CentralXY[0] - PanelSq[0] / 2;
                        mapRect1.X = 4 * (8 + Game.Zoom) * MapOffsetXY[0];
                        mapRect1.Width = mapWidth - mapRect1.X;
                        mapRect2.X = 4 * (8 + Game.Zoom);
                        mapRect2.Width = DrawPanel.Width - mapRect1.Width;
                    }
                    else
                    {
                        // If the tile at these coords doesn't exist, shift X to the right
                        if (CentralXY[0] % 2 != 0 && CentralXY[1] % 2 == 0) CentralXY[0]++;
                        if (CentralXY[0] % 2 == 0 && CentralXY[1] % 2 != 0) CentralXY[0]++;

                        MapOffsetXY[0] = CentralXY[0] - PanelSq[0] / 2;
                        mapRect1.Width = DrawPanel.Width;
                        mapRect1.X = 4 * (8 + Game.Zoom) * MapOffsetXY[0];
                    }
                    mapRect2.Y = mapRect1.Y;
                    mapRect2.Height = mapRect1.Height;
                }
            }
        }

        private int[] PanelOffsetXYpx => new int[] { 4 * (8 + Game.Zoom) * PanelOffsetXY[0], 2 * (8 + Game.Zoom) * PanelOffsetXY[1] };
        private int[] MapOffsetXYpx => new int[] { 4 * (8 + Game.Zoom) * MapOffsetXY[0], 2 * (8 + Game.Zoom) * MapOffsetXY[1] };
    }
}
