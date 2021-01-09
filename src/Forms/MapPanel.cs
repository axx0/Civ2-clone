using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ExtensionMethods;
using civ2.Events;
using civ2.Bitmaps;
using civ2.Units;
using civ2.Enums;
using System.Diagnostics;

namespace civ2.Forms
{
    public partial class MapPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private Map Map => Map.Instance;

        private Main _main;
        private static List<Bitmap> AnimationFrames;
        private int MapGridVar { get; set; }        // Style of map grid presentation
        private System.Windows.Forms.Timer AnimationTimer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        private AnimationType AnimType { get; set; }
        private int AnimationCount { get; set; }

        private int[] PanelMap_offset, MapPanel_offset, ActiveOffset, CentrXY, centrOffset, ClickedXY;
        private Rectangle mapRect1, mapRect2;
        private Rectangle mapSrc1, mapSrc2;
        private Point mapDest, mapStartSq;
        private int mapWidth, mapHeight;
        private int[] mapDrawSq;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        Bitmap map;
        int[] panelSq;

        public MapPanel(Main parent, int _width, int _height) : base(_width, _height, "", 38, 10)
        {
            _main = parent;

            Paint += MapPanel_Paint;

            Game.OnWaitAtTurnEnd += InitiateWaitAtTurnEnd;
            Game.OnUnitEvent += UnitEventHappened;
            Game.OnPlayerEvent += PlayerEventHappened;
            _MinimapPanel.OnMapEvent += MapEventHappened;
            StatusPanel.OnMapEvent += MapEventHappened;
            Main.OnMapEvent += MapEventHappened;
            Main.OnCheckIfCityCanBeViewed += CheckIfCityCanBeViewed;

            var ZoomINButton = new NoSelectButton
            {
                Location = new Point(11, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomIN, 4)
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            var ZoomOUTButton = new NoSelectButton
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.ResizeImage(Images.ZoomOUT, 4),
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
            AnimType = AnimationType.UpdateMap;
            MapViewChange(Game.StartingClickedXY);

            if (_main.ViewPieceMode)
                AnimType = AnimationType.ViewPiece;
            else
                AnimType = AnimationType.UnitWaiting;

            // Timer for waiting unit/ viewing piece
            AnimationTimer = new System.Windows.Forms.Timer();
            AnimationTimer.Tick += Animation_Tick;
            StartAnimation(AnimType);

            //map = Draw.DrawMapPart(Game.ActiveCiv.Id, 0, 0, 20, 20, 0, false);
            //panelSq = new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (2 * Game.Xpx)), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (2 * Game.Ypx)) };
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
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.Black), new Point((Width / 2) + 1, 20 + 1), sf);
            e.Graphics.DrawString($"{Game.PlayerCiv.Adjective} Map", new Font("Times New Roman", 17, FontStyle.Bold), new SolidBrush(Color.FromArgb(135, 135, 135)), new Point(Width / 2, 20), sf);
            e.Dispose();
            sf.Dispose();
        }

        // Draw map here
        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Calculate these once just for less computations
            int[] startingSqXYpx = new int[] { 0, 0 };

            // Draw map (for round world draw it from 2 parts)
            e.Graphics.DrawImage(map, mapDest.X, mapDest.Y, mapSrc1, GraphicsUnit.Pixel);
            //e.Graphics.DrawImage(Map.ActiveCivMap, PanelMap_offsetpx[0], PanelMap_offsetpx[1], mapRect1, GraphicsUnit.Pixel);
            //e.Graphics.DrawImage(Map.ActiveCivMap, PanelMap_offsetpx[0] + mapRect1.Width, PanelMap_offsetpx[1], mapRect2, GraphicsUnit.Pixel);

            // Draw animation
            //switch (AnimType)
            //{
            //    case AnimationType.UnitWaiting:
            //        {
            //            e.Graphics.DrawImage(AnimationFrames[AnimationCount % 2], ActiveOffsetPx[0], ActiveOffsetPx[1] - Game.Ypx);
            //            break;
            //        }
            //    case AnimationType.ViewPiece:
            //        {
            //            if (AnimationCount % 2 == 0)
            //                e.Graphics.DrawImage(Images.ViewPiece, ActiveOffsetPx[0], ActiveOffsetPx[1]);
            //            break;
            //        }
            //    case AnimationType.UnitMoving:
            //        {
            //            IUnit unit = Game.ActiveUnit;
            //            e.Graphics.DrawImage(AnimationFrames[Game.ActiveUnit.MovementCounter], unit.LastXYpx[0] - startingSqXYpx[0] - (2 * Game.Xpx), unit.LastXYpx[1] - startingSqXYpx[1] - (2 * Game.Ypx));
            //            break;
            //        }
            //}

            //e.Dispose();
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // If clicked location is beyond map limits --> exit method
            if (e.Location.X < mapDest.X || e.Location.X > mapDest.X + mapSrc1.Width || e.Location.Y < mapDest.Y || e.Location.Y > mapDest.Y + mapSrc1.Height) return;
            // Else you clicked within the map
            int clickedX = e.Location.X - mapDest.X;
            int clickedY = e.Location.Y - mapDest.Y;
            ClickedXY = Ext.PxToCoords(clickedX, clickedY, Game.Zoom);
            ClickedXY[0] += mapStartSq.X;
            ClickedXY[1] += mapStartSq.Y;
                
            Debug.WriteLine($"ClickedXY={ClickedXY[0]},{ClickedXY[1]}");

            // TODO: Make sure that edge black tiles are also ignored!

            //ClickedXY = new int[] { (MapPanel_offset[0] + coords[0]) % (2 * Map.Xdim), MapPanel_offset[1] + coords[1] };  // Coordinates of clicked square

            if (e.Button == MouseButtons.Left)
            {
                // City clicked
                if (Game.GetCities.Any(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]))
                {
                    if (_main.ViewPieceMode) Game.ActiveXY = ClickedXY;
                    var cityPanel = new CityPanel(_main, Game.GetCities.Find(city => city.X == ClickedXY[0] && city.Y == ClickedXY[1]), 658, 459); // For normal zoom
                    _main.Controls.Add(cityPanel);
                    cityPanel.Location = new Point((ClientSize.Width / 2) - (cityPanel.Size.Width / 2), (ClientSize.Height / 2) - (cityPanel.Size.Height / 2));
                    cityPanel.Show();
                    cityPanel.BringToFront();
                }
                else if (Game.GetUnits.Any(unit => unit.X == ClickedXY[0] && unit.Y == ClickedXY[1]))    // Unit clicked
                {
                    int clickedUnitIndex = Game.GetUnits.FindIndex(a => a.X == ClickedXY[0] && a.Y == ClickedXY[1]);
                    if (!Game.GetUnits[clickedUnitIndex].TurnEnded)
                    {
                        Game.ActiveUnit = Game.GetUnits[clickedUnitIndex];
                        _main.ViewPieceMode = false;
                        OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
                        StartAnimation(AnimationType.UnitWaiting);
                    }
                    else
                    {
                        //TODO: determine what happens if unit has ended turn...
                    }
                    MapViewChange(ClickedXY);
                }
                else    // Something else clicked
                {
                    if (_main.ViewPieceMode) Game.ActiveXY = ClickedXY;
                    MapViewChange(ClickedXY);
                }
            }
            else    // Right click
            {
                _main.ViewPieceMode = true;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
                Game.ActiveXY = ClickedXY;
                MapViewChange(ClickedXY);
                StartAnimation(AnimationType.ViewPiece);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            ReturnCoordsAtMapViewChange(newCenterCoords);
            panelSq = new int[] { 2 * (int)Math.Ceiling((double)DrawPanel.Width / (2 * Game.Xpx)), 2 * (int)Math.Ceiling((double)DrawPanel.Height / (2 * Game.Ypx)) };
            map = Draw.MapPart(Game.ActiveCiv.Id, mapStartSq.X, mapStartSq.Y, mapDrawSq[0], mapDrawSq[1], Game.Zoom, Game.Options.FlatEarth, true);
            DrawPanel.Invalidate();
        }

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
            //Map.SetNewActiveMapPic();
            DrawPanel.Invalidate();
        }
        public void ZoomINclicked(Object sender, EventArgs e)
        {
            Game.Zoom++;
            MapViewChange(CentrXY);
            //ReturnCoordsAtMapViewChange(CentrXY);
            //Map.SetNewActiveMapPic();
            //StartAnimation(AnimType);
            DrawPanel.Invalidate();
        }
        public void MaxZoomINclicked(Object sender, EventArgs e)
        {
            Game.Zoom = 16;
            //Map.SetNewActiveMapPic();
            DrawPanel.Invalidate();
        }
        public void MaxZoomOUTclicked(Object sender, EventArgs e)
        {
            Game.Zoom = 1;
            //Map.SetNewActiveMapPic();
            DrawPanel.Invalidate();
        }
        public void StandardZOOMclicked(Object sender, EventArgs e)
        {
            Game.Zoom = 8;
            //Map.SetNewActiveMapPic();
            DrawPanel.Invalidate();
        }
        public void MediumZoomOUTclicked(Object sender, EventArgs e)
        {
            Game.Zoom = 5;
            //Map.SetNewActiveMapPic();
            DrawPanel.Invalidate();
        }
        #endregion

        private void MapEventHappened(object sender, MapEventArgs e)
        {
            switch (e.EventType)
            {
                case MapEventType.MapViewChanged:
                    {
                        MapViewChange(e.CentrXY);
                        break;
                    }
                case MapEventType.SwitchViewMovePiece:
                    {
                        if (_main.ViewPieceMode) StartAnimation(AnimationType.ViewPiece);
                        else StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case MapEventType.ViewPieceMoved:
                    {
                        StartAnimation(AnimationType.ViewPiece);
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
                        if (Game.ActiveUnit != null) _main.ViewPieceMode = false;
                        AnimationTimer.Stop();
                        AnimationCount = 0;
                        AnimationTimer.Start();
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
                        AnimationCount = Game.ActiveUnit.MovementCounter;
                        if (AnimationCount == 0) StartAnimation(AnimationType.UnitMoving);
                        //DrawAnimation();
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
                case UnitEventType.NewUnitActivated:
                    {
                        ReturnCoordsAtMapViewChange(Game.ActiveXY);
                        StartAnimation(AnimationType.UnitWaiting);
                        break;
                    }
            }
        }

        private void InitiateWaitAtTurnEnd(object sender, WaitAtTurnEndEventArgs e)
        {
            _main.ViewPieceMode = true;
            AnimationTimer.Stop();
            AnimationCount = 0;
            AnimationTimer.Start();
        }

        // If ENTER pressed when view piece above city --> enter city view
        private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        {
            if (_main.ViewPieceMode && Game.GetCities.Any(city => city.X == Game.ActiveXY[0] && city.Y == Game.ActiveXY[1]))
            {
                //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
                //cityForm.Show();
            }
        }

        #region Animation
        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.UpdateMap:
                    AnimationTimer.Stop();
                    AnimationCount = 0;
                    DrawPanel.Invalidate();
                    break;
                case AnimationType.UnitWaiting:
                    //AnimType = AnimationType.UnitWaiting;
                    AnimationTimer.Stop();
                    AnimationFrames = GetAnimationFrames.UnitWaiting(_main.ViewPieceMode);
                    AnimationCount = 0;
                    AnimationTimer.Interval = 200;    // ms                    
                    AnimationTimer.Start();
                    break;
                case AnimationType.UnitMoving:
                    //AnimType = AnimationType.UnitMoving;
                    AnimationFrames = GetAnimationFrames.UnitMoving(_main.ViewPieceMode);
                    break;
                case AnimationType.ViewPiece:
                    //AnimType = AnimationType.ViewPieces;
                    AnimationTimer.Stop();
                    AnimationCount = 0;
                    AnimationTimer.Interval = 200;    // ms                    
                    AnimationTimer.Start();
                    break;
            }
        }

        private void Animation_Tick(object sender, EventArgs e)
        {
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (AnimationCount == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle(ActiveOffsetPx[0], ActiveOffsetPx[1] - Game.Ypx, 2 * Game.Xpx, 3 * Game.Ypx));
                        break;
                    }
                case AnimationType.ViewPiece:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (AnimationCount == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else
                            DrawPanel.Invalidate(new Rectangle(ActiveOffsetPx[0], ActiveOffsetPx[1], 2 * Game.Xpx, 2 * Game.Ypx));
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        DrawPanel.Invalidate(new Rectangle((Game.ActiveXY[0] - MapPanel_offset[0]) * 32 - 64, (Game.ActiveXY[1] - MapPanel_offset[1]) * 16 - 48, 3 * 64, (3 * 32) + 16));
                        Update();
                        if (AnimationCount == 7)  // Unit has completed movement
                        {
                            // First update world map with new visible tiles
                            Game.UpdateWorldMapAfterUnitHasMoved();

                            // Update the original world map image with image of new location of unit & redraw whole map
                            IUnit unit = Game.Instance.ActiveUnit;
                            // Game.CivsMap[Game.Instance.ActiveCiv.Id] = ModifyImage.MergedBitmaps(Game.CivsMap[Game.Instance.ActiveCiv.Id], AnimationFrames[TimerCounter], 32 * unit.LastXY[0] - 64, 16 * unit.LastXY[1] - 48);
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                            Update();

                            // Then stop animation
                            StartAnimation(AnimationType.UpdateMap);

                            // Check if unit moved outside map view -> map view needs to be updated
                            if (UnitMovedOutsideMapView)
                            {
                                ReturnCoordsAtMapViewChange(Game.ActiveXY);
                                DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                                Update();
                            }
                        }
                        break;
                    }
            }

            AnimationCount++;
        }
        #endregion

        private bool UnitMovedOutsideMapView
        {
            get
            {
                if (Game.ActiveXY[0] >= 2 * Map.Xdim ||
                Game.ActiveXY[0] < 0 ||
                Game.ActiveXY[1] >= Map.Ydim ||
                Game.ActiveXY[1] < 0)
                    return true;
                else
                    return false;
            }
        }

        // Function which sets various variables for drawing map on grid
        private void ReturnCoordsAtMapViewChange(int[] proposedCentralCoords)
        {
            CentrXY = proposedCentralCoords;
            ActiveOffset = new int[] { 0, 0 };

            // For making an image of map part
            mapStartSq = new Point(0, 0);   // Starting squares for drawn map pic
            mapDrawSq = new int[] { 0, 0 }; // Squares to be drawn on map pic
            // For drawing map on panel
            mapSrc1 = mapSrc2 = new Rectangle(0, 0, 0, 0);  // Rectangle part of map pic to be drawn
            mapDest = new Point(0, 0);  // XY coords of whre map should be drawn on panel (in px)

            int fullMapWidth = Game.Xpx * (2 * Map.Xdim + 1);
            int fullMapHeight = Game.Ypx * (Map.Ydim + 1);

            // No of squares of panel and map
            int[] panelSq = { (int)Math.Ceiling((double)DrawPanel.Width / Game.Xpx), (int)Math.Ceiling((double)DrawPanel.Height / Game.Ypx) };
            centrOffset = new int[] { panelSq[0] / 2, panelSq[1] / 2 };
            if (centrOffset[0] % 2 != 1 && centrOffset[1] % 2 == 1) centrOffset[1]--;
            if (centrOffset[0] % 2 == 1 && centrOffset[1] % 2 != 1) centrOffset[0]--;

            // Number of drawn squares in both directions (in line with how game works). It's always multiple of 2 squares.
            mapDrawSq[0] = 2 * (int)Math.Floor((double)Math.Min(fullMapWidth, DrawPanel.Width) / (2 * Game.Xpx));
            mapDrawSq[1] = 2 * (int)Math.Floor((double)Math.Min(fullMapHeight, DrawPanel.Height) / (2 * Game.Ypx));

            // Initial calculation of map starting sq
            mapStartSq.X = proposedCentralCoords[0] - centrOffset[0];
            mapStartSq.Y = proposedCentralCoords[1] - centrOffset[1];

            // Correct starting coords if necessary
            if (mapStartSq.X <= 0)
            {
                mapStartSq.X = 0;
                if (mapStartSq.Y <= 0)
                {
                    mapStartSq.Y = 0;
                }
                else if (mapStartSq.Y + mapDrawSq[1] >= Map.Ydim)
                {
                    mapStartSq.Y = Map.Ydim - mapDrawSq[1];
                }
                else
                {
                    if (mapStartSq.Y % 2 != 0) mapStartSq.Y--;
                }
            }
            else if (mapStartSq.X + mapDrawSq[0] >= 2 * Map.Xdim)
            {
                mapStartSq.X = 2 * Map.Xdim - mapDrawSq[0];
                if (mapStartSq.Y <= 0)
                {
                    mapStartSq.Y = 0;
                }
                else if (mapStartSq.Y + mapDrawSq[1] >= Map.Ydim)
                {
                    mapStartSq.Y = Map.Ydim - mapDrawSq[1];
                }
                else
                {
                    if (mapStartSq.Y % 2 != 0) mapStartSq.Y--;
                }
            }
            else
            {
                if (mapStartSq.Y <= 0)
                {
                    mapStartSq.Y = 0;
                    if (mapStartSq.X % 2 != 0) mapStartSq.X--;
                }
                else if (mapStartSq.Y + mapDrawSq[1] >= Map.Ydim)
                {
                    mapStartSq.Y = Map.Ydim - mapDrawSq[1];
                    if (mapStartSq.X % 2 != 0) mapStartSq.X--;
                }
                else
                {
                    if (mapStartSq.X % 2 == 0 && mapStartSq.Y % 2 != 0) mapStartSq.Y--;
                    if (mapStartSq.X % 2 != 0 && mapStartSq.Y % 2 == 0) mapStartSq.X--;
                }
            }

            // Determine drawing rectangles
            if (panelSq[0] > 2 * Map.Xdim + 1)
            {
                mapSrc1.Width = fullMapWidth;
                mapDest.X = (DrawPanel.Width - fullMapWidth) / 2;
            }
            else
            {
                mapSrc1.Width = DrawPanel.Width;
                mapDest.X = 0;
            }

            if (panelSq[1] > Map.Ydim + 1)
            {
                mapSrc1.Height = fullMapHeight;
                mapDest.Y = (DrawPanel.Height - fullMapHeight) / 2;
            }
            else
            {
                mapSrc1.Height = DrawPanel.Height;
                mapDest.Y = 0;
            }

            Debug.WriteLine($"panelSq {panelSq[0]},{panelSq[1]}");
            Debug.WriteLine($"mapStartSq {mapStartSq}");
            Debug.WriteLine($"mapDrawSq X={mapDrawSq[0]} Y={mapDrawSq[1]}");
            Debug.WriteLine($"mapSrc1 {mapSrc1}");
            Debug.WriteLine($"mapDest {mapDest}");
            Debug.WriteLine("");

            //OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, CentrXY, CentrOffset, ActiveOffset, PanelMap_offset, MapPanel_offset, mapRect1, mapRect2));
            //DrawPanel.Invalidate();
        }

        private int[] PanelMap_offsetpx => new int[] { Game.Xpx * PanelMap_offset[0], Game.Ypx * PanelMap_offset[1] };
        private int[] MapPanel_offsetpx => new int[] { Game.Xpx * MapPanel_offset[0], Game.Ypx * MapPanel_offset[1] };
        private int[] ActiveOffsetPx => new int[] { Game.Xpx * ActiveOffset[0], Game.Ypx * ActiveOffset[1] };
    }
}
