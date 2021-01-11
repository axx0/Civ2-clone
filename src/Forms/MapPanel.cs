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
    public class MapPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private Map Map => Map.Instance;

        private Main _main;
        private static List<Bitmap> AnimationFrames;
        private int MapGridVar { get; set; }        // Style of map grid presentation
        private System.Windows.Forms.Timer animationTimer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        private AnimationType AnimType { get; set; }
        private int AnimationCount { get; set; }

        private int[] CentrXY, centrOffset;
        private Rectangle mapSrc1, mapSrc2;
        private int[] mapStartXY, activeOffsetXY, mapDrawSq, clickedXY;
        private Point mapDest;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        private Bitmap map;

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
                BackgroundImage = ModifyImage.Resize(Images.ZoomIN, 4)
            };
            ZoomINButton.FlatAppearance.BorderSize = 0;
            Controls.Add(ZoomINButton);
            ZoomINButton.Click += ZoomINclicked;

            var ZoomOUTButton = new NoSelectButton
            {
                Location = new Point(36, 9),
                Size = new Size(23, 23),
                FlatStyle = FlatStyle.Flat,
                BackgroundImage = ModifyImage.Resize(Images.ZoomOUT, 4),
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
            activeOffsetXY = new int[] { 0, 0 };    // Just initialize
            MapViewChange(Game.StartingClickedXY);

            if (_main.ViewPieceMode) AnimType = AnimationType.ViewPiece;
            else AnimType = AnimationType.UnitWaiting;

            // Timer for waiting unit/ viewing piece
            animationTimer = new System.Windows.Forms.Timer();
            animationTimer.Tick += Animation_Tick;
            StartAnimation(AnimType);
        }

        private void MapPanel_Paint(object sender, PaintEventArgs e)
        {
            // Title
            using var _font = new Font("Times New Roman", 17, FontStyle.Bold);
            Draw.Text(e.Graphics, $"{Game.PlayerCiv.Adjective} Map", _font, StringAlignment.Center, StringAlignment.Center, Color.FromArgb(135, 135, 135), new Point(Width / 2, 20), Color.Black, 1, 1);
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
            switch (AnimType)
            {
                case AnimationType.UnitWaiting:
                    {
                        e.Graphics.DrawImage(AnimationFrames[AnimationCount % 2], ActiveOffsetPx.X, ActiveOffsetPx.Y - Game.Ypx);
                        break;
                    }
                case AnimationType.ViewPiece:
                    {
                        if (AnimationCount % 2 == 0) Draw.ViewPiece(e.Graphics, Game.Zoom, ActiveOffsetPx);
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        IUnit unit = Game.ActiveUnit;
                        e.Graphics.DrawImage(AnimationFrames[Game.ActiveUnit.MovementCounter], unit.LastXYpx[0] - startingSqXYpx[0] - (2 * Game.Xpx), unit.LastXYpx[1] - startingSqXYpx[1] - (2 * Game.Ypx));
                        break;
                    }
            }
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // If clicked location is beyond map limits --> exit method
            if (e.Location.X < mapDest.X || e.Location.X > mapDest.X + mapSrc1.Width || e.Location.Y < mapDest.Y || e.Location.Y > mapDest.Y + mapSrc1.Height) return;
            // Else you clicked within the map
            int clickedX = e.Location.X - mapDest.X;
            int clickedY = e.Location.Y - mapDest.Y;
            clickedXY = Ext.PxToCoords(clickedX, clickedY, Game.Zoom);
            clickedXY[0] += mapStartXY[0];
            clickedXY[1] += mapStartXY[1];
                
            Debug.WriteLine($"clickedXY={clickedXY[0]},{clickedXY[1]}");

            // TODO: Make sure that edge black tiles are also ignored!

            //clickedXY = new int[] { (MapPanel_offset[0] + coords[0]) % (2 * Map.Xdim), MapPanel_offset[1] + coords[1] };  // Coordinates of clicked square

            if (e.Button == MouseButtons.Left)
            {
                // City clicked
                if (Game.GetCities.Any(city => city.X == clickedXY[0] && city.Y == clickedXY[1]))
                {
                    if (_main.ViewPieceMode) Game.ActiveXY = clickedXY;
                    var cityPanel = new CityPanel(_main, Game.GetCities.Find(city => city.X == clickedXY[0] && city.Y == clickedXY[1]), 658, 459); // For normal zoom
                    _main.Controls.Add(cityPanel);
                    cityPanel.Location = new Point((ClientSize.Width / 2) - (cityPanel.Size.Width / 2), (ClientSize.Height / 2) - (cityPanel.Size.Height / 2));
                    cityPanel.Show();
                    cityPanel.BringToFront();
                }
                else if (Game.GetUnits.Any(unit => unit.X == clickedXY[0] && unit.Y == clickedXY[1]))    // Unit clicked
                {
                    int clickedUnitIndex = Game.GetUnits.FindIndex(a => a.X == clickedXY[0] && a.Y == clickedXY[1]);
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
                    MapViewChange(clickedXY);
                }
                else    // Something else clicked
                {
                    if (_main.ViewPieceMode) Game.ActiveXY = clickedXY;
                    MapViewChange(clickedXY);
                }
            }
            else    // Right click
            {
                _main.ViewPieceMode = true;
                OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.SwitchViewMovePiece));
                Game.ActiveXY = clickedXY;
                MapViewChange(clickedXY);
                StartAnimation(AnimationType.ViewPiece);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            ReturnCoordsAtMapViewChange(newCenterCoords);
            map = Draw.MapPart(Game.ActiveCiv.Id, mapStartXY[0], mapStartXY[1], mapDrawSq[0], mapDrawSq[1], Game.Zoom, Game.Options.FlatEarth, true);
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

        #region Zoom events
        public void ZoomINclicked(Object sender, EventArgs e)
        {
            if (Game.Zoom != 8)
            {
                Game.Zoom++;
                MapViewChange(CentrXY);
                StartAnimation(AnimType);
                DrawPanel.Invalidate();
            }
        }

        public void ZoomOUTclicked(Object sender, EventArgs e)
        {
            if (Game.Zoom != 8)
            {
                Game.Zoom--;
                MapViewChange(CentrXY);
                StartAnimation(AnimType);
                DrawPanel.Invalidate();
            }
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
                case MapEventType.ZoomChanged:
                    {
                        MapViewChange(CentrXY);
                        StartAnimation(AnimType);
                        DrawPanel.Invalidate();
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
                        animationTimer.Stop();
                        AnimationCount = 0;
                        animationTimer.Start();
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
            animationTimer.Stop();
            AnimationCount = 0;
            animationTimer.Start();
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
                    animationTimer.Stop();
                    AnimationCount = 0;
                    DrawPanel.Invalidate();
                    break;
                case AnimationType.UnitWaiting:
                    animationTimer.Stop();
                    AnimationFrames = GetAnimationFrames.UnitWaiting(_main.ViewPieceMode);
                    AnimationCount = 0;
                    animationTimer.Interval = 200;    // ms                    
                    animationTimer.Start();
                    break;
                case AnimationType.UnitMoving:
                    //AnimType = AnimationType.UnitMoving;
                    AnimationFrames = GetAnimationFrames.UnitMoving(_main.ViewPieceMode);
                    break;
                case AnimationType.ViewPiece:
                    animationTimer.Stop();
                    AnimationCount = 0;
                    animationTimer.Interval = 200;    // ms                    
                    animationTimer.Start();
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
                            DrawPanel.Invalidate(new Rectangle(ActiveOffsetPx.X, ActiveOffsetPx.Y - Game.Ypx, 2 * Game.Xpx, 3 * Game.Ypx));
                        break;
                    }
                case AnimationType.ViewPiece:
                    {
                        // At new unit turn initially re-draw the whole map
                        if (AnimationCount == 0)
                            DrawPanel.Invalidate(new Rectangle(0, 0, DrawPanel.Width, DrawPanel.Height));
                        else if (ActiveOffsetPx.X >= 0 && ActiveOffsetPx.X <= DrawPanel.Width && ActiveOffsetPx.Y >= 0 && ActiveOffsetPx.Y <= DrawPanel.Height) // Draw only if active piece is within the panel
                            DrawPanel.Invalidate(new Rectangle(ActiveOffsetPx.X, ActiveOffsetPx.Y, 2 * Game.Xpx, 2 * Game.Ypx));
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        //DrawPanel.Invalidate(new Rectangle((Game.ActiveXY[0] - MapPanel_offset[0]) * 32 - 64, (Game.ActiveXY[1] - MapPanel_offset[1]) * 16 - 48, 3 * 64, (3 * 32) + 16));
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
                if (Game.ActiveXY[0] >= 2 * Map.Xdim || Game.ActiveXY[0] < 0 || Game.ActiveXY[1] >= Map.Ydim || Game.ActiveXY[1] < 0)
                    return true;
                else
                    return false;
            }
        }

        // Function which sets various variables for drawing map on grid
        private void ReturnCoordsAtMapViewChange(int[] proposedCentralCoords)
        {
            CentrXY = proposedCentralCoords;

            // For making an image of map part
            mapStartXY = new int[] { 0, 0 };   // Starting square for drawn map pic
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

            // Initial calculation of map starting coords
            mapStartXY[0] = proposedCentralCoords[0] - centrOffset[0];
            mapStartXY[1] = proposedCentralCoords[1] - centrOffset[1];

            // Correct starting coords if necessary
            if (mapStartXY[0] <= 0)
            {
                mapStartXY[0] = 0;
                if (mapStartXY[1] <= 0)
                {
                    mapStartXY[1] = 0;
                }
                else if (mapStartXY[1] + mapDrawSq[1] >= Map.Ydim)
                {
                    mapStartXY[1] = Map.Ydim - mapDrawSq[1];
                }
                else
                {
                    if (mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                }
            }
            else if (mapStartXY[0] + mapDrawSq[0] >= 2 * Map.Xdim)
            {
                mapStartXY[0] = 2 * Map.Xdim - mapDrawSq[0];
                if (mapStartXY[1] <= 0)
                {
                    mapStartXY[1] = 0;
                }
                else if (mapStartXY[1] + mapDrawSq[1] >= Map.Ydim)
                {
                    mapStartXY[1] = Map.Ydim - mapDrawSq[1];
                }
                else
                {
                    if (mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                }
            }
            else
            {
                if (mapStartXY[1] <= 0)
                {
                    mapStartXY[1] = 0;
                    if (mapStartXY[0] % 2 != 0) mapStartXY[0]--;
                }
                else if (mapStartXY[1] + mapDrawSq[1] >= Map.Ydim)
                {
                    mapStartXY[1] = Map.Ydim - mapDrawSq[1];
                    if (mapStartXY[0] % 2 != 0) mapStartXY[0]--;
                }
                else
                {
                    if (mapStartXY[0] % 2 == 0 && mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                    if (mapStartXY[0] % 2 != 0 && mapStartXY[1] % 2 == 0) mapStartXY[0]--;
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

            // Set the new offset of the active piece
            activeOffsetXY[0] = Game.ActiveXY[0] - mapStartXY[0];
            activeOffsetXY[1] = Game.ActiveXY[1] - mapStartXY[1];

            Debug.WriteLine($"panelSq {panelSq[0]},{panelSq[1]}");
            Debug.WriteLine($"mapStartXY {mapStartXY}");
            Debug.WriteLine($"mapDrawSq X={mapDrawSq[0]} Y={mapDrawSq[1]}");
            Debug.WriteLine($"mapSrc1 {mapSrc1}");
            Debug.WriteLine($"mapDest {mapDest}");
            Debug.WriteLine("");

            //OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, CentrXY, CentrOffset, ActiveOffset, PanelMap_offset, MapPanel_offset, mapRect1, mapRect2));
            //DrawPanel.Invalidate();
        }

        //private int[] PanelMap_offsetpx => new int[] { Game.Xpx * PanelMap_offset[0], Game.Ypx * PanelMap_offset[1] };
        //private int[] MapPanel_offsetpx => new int[] { Game.Xpx * MapPanel_offset[0], Game.Ypx * MapPanel_offset[1] };
        private Point ActiveOffsetPx => new Point(Game.Xpx * activeOffsetXY[0], Game.Ypx * activeOffsetXY[1]);
    }
}
