using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.Units;

namespace EtoFormsUI
{
    public class MapPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        private Map Map => Game.CurrentMap;
        
        private Main main;
        private List<Bitmap> animationFrames;
        private readonly UITimer animationTimer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        private AnimationType animType;
        private int animationCount;
        public Drawable drawPanel;

        private int[] CentrXY, centrOffset;
        private Rectangle mapSrc1, mapSrc2;
        private int[] mapStartXY, activeOffsetXY, mapDrawSq, clickedXY;
        private Point mapDest;
        private bool updateMap;
        private CityWindow cityWindow;
        public Point CityWindowLocation;
        public int CityWindowZoom;

        private int _topOffset = 0;

        public static event EventHandler<MapEventArgs> OnMapEvent;

        private Bitmap map;

        public MapPanel(Main parent, int width, int height) : base(width, height, 38, 10)
        {
            main = parent;
            
            drawPanel = new Drawable() { Size = new Size(MainPanel.Width - 2 * 11, MainPanel.Height - 38 - 10), BackgroundColor = Colors.Black };
            drawPanel.Paint += DrawPanel_Paint;
            drawPanel.MouseUp += DrawPanel_MouseClick;
            MainPanelLayout.Add(drawPanel, 11, 38);
            MainPanel.Content = MainPanelLayout;

            MainPanel.Paint += (sender, e) => Draw.Text(e.Graphics, $"{Game.GetActiveCiv.Adjective} Map", new Font("Times new roman", 17, FontStyle.Bold), Color.FromArgb(135, 135, 135), new Point(MainPanel.Width / 2, 38 / 2), true, true, Colors.Black, 1, 1);

            Game.OnUnitEvent += UnitEventHappened;
            Game.OnPlayerEvent += PlayerEventHappened;
            MinimapPanel.OnMapEvent += MapEventHappened;
            StatusPanel.OnMapEvent += MapEventHappened;
            Game.OnMapEvent += MapEventHappened;
            Main.OnMapEvent += MapEventHappened;

            //var ZoomINButton = new NoSelectButton
            //{
            //    Location = new Point(11, 9),
            //    Size = new Size(23, 23),
            //    FlatStyle = FlatStyle.Flat,
            //    BackgroundImage = Images.ZoomIN.Resize(4)
            //};
            //ZoomINButton.FlatAppearance.BorderSize = 0;
            //Controls.Add(ZoomINButton);
            //ZoomINButton.Click += ZoomINclicked;

            //var ZoomOUTButton = new NoSelectButton
            //{
            //    Location = new Point(36, 9),
            //    Size = new Size(23, 23),
            //    FlatStyle = FlatStyle.Flat,
            //    BackgroundImage = Images.ZoomOUT.Resize(4),
            //};
            //ZoomOUTButton.FlatAppearance.BorderSize = 0;
            //Controls.Add(ZoomOUTButton);
            //ZoomOUTButton.Click += ZoomOUTclicked;

            // City window
            CityWindowZoom = 0;   // TODO: Save city zoom level (-1/0/1) option somewhere in game options/settings
            CityWindowLocation = new Point((this.Width / 2) - (636 / 2 * (2 + CityWindowZoom) / 2 + 2 * 11), (this.Height / 2) - (421 / 2 * (2 + CityWindowZoom) / 2 + 11 + (CityWindowZoom == -1 ? 21 : (CityWindowZoom == 0 ? 27 : 39))));

            // Starting animation
            animationTimer = new UITimer(); // Timer for waiting unit/ viewing piece
            animationTimer.Elapsed += OnAnimationElapsedEvent;
            animType = AnimationType.Waiting;

            // Center the map view and draw map
            MapViewChange(Map.StartingClickedXY);
        }

        // Draw map here
        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            // Draw map (for round world draw it from 2 parts)
            //e.Graphics.DrawImage(map, mapSrc1, mapDest);
            //e.Graphics.DrawImage(Map.ActiveCivMap, PanelMap_offsetpx[0], PanelMap_offsetpx[1], mapRect1, GraphicsUnit.Pixel);
            //e.Graphics.DrawImage(Map.ActiveCivMap, PanelMap_offsetpx[0] + mapRect1.Width, PanelMap_offsetpx[1], mapRect2, GraphicsUnit.Pixel);

            // Update whole map
            if (updateMap)
            {
                e.Graphics.DrawImage(map, mapSrc1, mapDest);
                updateMap = false;
                StartAnimation(animType);
            }
            // Draw animation
            else
            {
                switch (animType)
                {
                    case AnimationType.Waiting:
                        {
                            e.Graphics.DrawImage(animationFrames[animationCount % 2], ActiveOffsetPx.X, ActiveOffsetPx.Y - Map.Ypx + mapDest.Y);
                            break;
                        }
                    case AnimationType.UnitMoving:
                        {
                            e.Graphics.DrawImage(animationFrames[animationCount], Game.GetActiveUnit.PrevXYpx[0] - MapStartPx.X - (2 * Map.Xpx), Game.GetActiveUnit.PrevXYpx[1] - MapStartPx.Y - (3 * Map.Ypx) + mapDest.Y);
                            break;
                        }
                    case AnimationType.Attack:
                        {
                            e.Graphics.DrawImage(animationFrames[animationCount], Game.GetActiveUnit.Xpx - MapStartPx.X - (2 * Map.Xpx), Game.GetActiveUnit.Ypx - MapStartPx.Y - (3 * Map.Ypx) + mapDest.Y);
                            break;
                        }
                }
            }
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            // If clicked location is beyond map limits --> exit method
            if (e.Location.X < mapDest.X || e.Location.X > mapDest.X + mapSrc1.Width || e.Location.Y < mapDest.Y || e.Location.Y > mapDest.Y + mapSrc1.Height) return;
            // Else you clicked within the map
            clickedXY = PxToCoords((int)e.Location.X - mapDest.X, (int)e.Location.Y - mapDest.Y, Map.Zoom);
            clickedXY[0] += mapStartXY[0];
            clickedXY[1] += mapStartXY[1];
            
            Debug.WriteLine($"clickedXY={clickedXY[0]},{clickedXY[1]}");


            // TODO: Make sure that edge black tiles are also ignored!

            if (e.Buttons == MouseButtons.Primary)  // Left button
            {
                var unitsHere = Game.UnitsHere(clickedXY[0], clickedXY[1]);

                // City clicked
                if (Game.AnyCitiesPresentHere(clickedXY[0], clickedXY[1]))
                {
                    // If in view piece mode move the viewing piece there
                    if (Map.ViewPieceMode)
                    {
                        Map.ActiveXY = clickedXY;
                        animType = AnimationType.Waiting;
                        UpdateMap();
                    }

                    // If city window is already open, close it before opening new instance
                    if (cityWindow != null)
                    {
                        CityWindowLocation = cityWindow.Location;
                        cityWindow.Close();
                    }
                    
                    cityWindow = new CityWindow(main, this, Game.CityHere(clickedXY[0], clickedXY[1]), CityWindowZoom);
                    cityWindow.Show();
                }
                // Unit clicked
                else if (unitsHere.Count > 0 && unitsHere.Last().Owner == Game.GetActiveCiv)
                {
                    // Single unit on square
                    if (unitsHere.Count == 1)
                    {
                        Game.SetActiveUnit(unitsHere.First());
                        unitsHere.First().Order = OrderType.NoOrders;   // Always clear order when clicked, no matter if the unit is activated
                        Map.ViewPieceMode = false;
                        MapViewChange(clickedXY);
                    }
                    // Multiple units on this square => open unit selection dialog
                    else
                    {
                        var selectUnitDialog = new SelectUnitDialog(main, unitsHere);
                        selectUnitDialog.ShowModal(main);

                        if (selectUnitDialog.SelectedIndex >= 0)
                        {
                            Game.SetActiveUnit(unitsHere[selectUnitDialog.SelectedIndex]);
                            Map.ViewPieceMode = false;
                            MapViewChange(clickedXY);
                        }
                    }
                }
                // Something else clicked
                else
                {
                    if (Map.ViewPieceMode) Map.ActiveXY = clickedXY;
                    MapViewChange(clickedXY);
                }
            }
            else    // Right click
            {
                Map.ViewPieceMode = true;
                Map.ActiveXY = clickedXY;
                MapViewChange(clickedXY);
                //StartAnimation(AnimationType.Waiting);
            }
        }

        private void MapViewChange(int[] newCenterCoords)
        {
            if (map != null) map.Dispose();
            SetCoordsAtMapViewChange(newCenterCoords);
            map = Draw.MapPart(Game.GetActiveCiv.Id, mapStartXY[0], mapStartXY[1], mapDrawSq[0], mapDrawSq[1], Game.Options.FlatEarth, Map.MapRevealed);
            UpdateMap();
            OnMapEvent?.Invoke(null, new MapEventArgs(MapEventType.MapViewChanged, mapStartXY, mapDrawSq));
        }

        public void UpdateMap()
        {
            updateMap = true;
            if (map != null) map.Dispose();
            map = Draw.MapPart(Map.WhichCivsMapShown, mapStartXY[0], mapStartXY[1], mapDrawSq[0], mapDrawSq[1], Game.Options.FlatEarth, Map.MapRevealed);
            drawPanel.Invalidate();
        }

        #region Zoom events
        public void ZoomINclicked(Object sender, EventArgs e)
        {
            if (Map.Zoom != 8)
            {
                Map.Zoom++;
                MapViewChange(CentrXY);
                //StartAnimation(animType);
                //drawPanel.Invalidate();
            }
        }

        public void ZoomOUTclicked(Object sender, EventArgs e)
        {
            if (Map.Zoom != 8)
            {
                Map.Zoom--;
                MapViewChange(CentrXY);
                //StartAnimation(animType);
                //drawPanel.Invalidate();
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
                        animType = AnimationType.Waiting;
                        UpdateMap();
                        break;
                    }
                case MapEventType.ViewPieceMoved:
                    {
                        StartAnimation(AnimationType.Waiting);
                        break;
                    }
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        drawPanel.Update(new Rectangle(0, 0, drawPanel.Width, drawPanel.Height));
                        break;
                    }
                case MapEventType.ZoomChanged:
                    {
                        MapViewChange(CentrXY);
                        //StartAnimation(animType);
                        //drawPanel.Invalidate();
                        break;
                    }
                case MapEventType.CenterView:
                    {
                        MapViewChange(Map.ActiveXY);
                        //drawPanel.Invalidate();
                        break;
                    }
                case MapEventType.ToggleGrid:
                    {
                        MapViewChange(CentrXY);
                        //StartAnimation(animType);
                        //drawPanel.Invalidate();
                        break;
                    }
                case MapEventType.WaitAtEndOfTurn:
                    {
                        Map.ViewPieceMode = true;
                        StartAnimation(AnimationType.Waiting);
                        break;
                    }
                case MapEventType.UpdateMap:
                    {
                        UpdateMap();
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
                        if (Game.GetActiveUnit != null) Map.ViewPieceMode = false;
                        animationTimer.Stop();
                        animationCount = 0;
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
                        animType = AnimationType.UnitMoving;
                        if (IsActiveSquareOutsideMapView) 
                        { 
                            MapViewChange(Map.ActiveXY);
                        }
                        StartAnimation(animType);
                        break;
                    }
                case UnitEventType.Attack:
                    {
                        animationFrames = GetAnimationFrames.UnitAttack(e);
                        StartAnimation(AnimationType.Attack);
                        break;
                    }
                case UnitEventType.StatusUpdate:
                    {
                        animType = AnimationType.Waiting;
                        if (IsActiveSquareOutsideMapView) MapViewChange(Map.ActiveXY);
                        UpdateMap();
                        break;
                    }
            }
        }

        // If ENTER pressed when view piece above city --> enter city view
        //private void CheckIfCityCanBeViewed(object sender, CheckIfCityCanBeViewedEventArgs e)
        //{
        //    if (Map.ViewPieceMode && Game.GetCities.Any(city => city.X == Map.ActiveXY[0] && city.Y == Map.ActiveXY[1]))
        //    {
        //        //CityForm cityForm = new CityForm(this, Game.Cities.Find(city => city.X == ActiveXY[0] && city.Y == ActiveXY[1]));
        //        //cityForm.Show();
        //    }
        //}

        #region Animation
        private void StartAnimation(AnimationType anim)
        {
            switch (anim)
            {
                case AnimationType.Waiting:
                    animType = AnimationType.Waiting;
                    animationTimer.Stop();
                    animationFrames = GetAnimationFrames.Waiting(Map.ActiveXY);
                    animationCount = 0;
                    animationTimer.Interval = 0.2;    // sec
                    animationTimer.Start();
                    break;
                case AnimationType.UnitMoving:
                    animType = AnimationType.UnitMoving;
                    animationFrames = GetAnimationFrames.UnitMoving(Game.GetActiveUnit);
                    animationTimer.Stop();
                    animationCount = 0;
                    animationTimer.Interval = 0.02;    // sec
                    animationTimer.Start();
                    break;
                case AnimationType.Attack:
                    animType = AnimationType.Attack;
                    animationTimer.Stop();
                    animationCount = 0;
                    animationTimer.Interval = 0.07;    // sec
                    animationTimer.Start();
                    break;
            }
        }

        private void OnAnimationElapsedEvent(object sender, EventArgs e)
        {
            switch (animType)
            {
                case AnimationType.Waiting:
                    {
                        // At new unit turn initially re-draw the whole map
                        //if (animationCount == 0) drawPanel.Update(new Rectangle(0, 0, drawPanel.Width, drawPanel.Height));
                        //else drawPanel.Update(new Rectangle(ActiveOffsetPx.X, ActiveOffsetPx.Y - Map.Ypx, 2 * Map.Xpx, 3 * Map.Ypx));
                        drawPanel.Update(new Rectangle(ActiveOffsetPx.X, ActiveOffsetPx.Y - Map.Ypx, 2 * Map.Xpx, 3 * Map.Ypx));
                        break;
                    }
                case AnimationType.UnitMoving:
                    {
                        drawPanel.Update(new Rectangle(ActiveOffsetPx.X, ActiveOffsetPx.Y, 6 * Map.Xpx, 7 * Map.Ypx));
                        if (animationCount == 7)  // Unit has completed movement
                        {
                            animationTimer.Stop();
                            Game.UpdateActiveUnit();
                        }
                        break;
                    }
                case AnimationType.Attack:
                    {
                        drawPanel.Update(new Rectangle(ActiveOffsetPx.X, ActiveOffsetPx.Y, 6 * Map.Xpx, 7 * Map.Ypx));
                        if (animationCount == animationFrames.Count - 1)
                        {
                            animationTimer.Stop();
                            Game.UpdateActiveUnit();
                        }
                        break;
                    }
            }

            animationCount++;
        }
        #endregion

        private bool IsActiveSquareOutsideMapView
        {
            get
            {
                if (Map.ActiveXY[0] >= mapStartXY[0] + PanelSq[0] - 2 || Map.ActiveXY[0] <= mapStartXY[0] || Map.ActiveXY[1] >= mapStartXY[1] + PanelSq[1] - 2 || Map.ActiveXY[1] <= mapStartXY[1]) return true;
                else return false;
            }
        }

        // Function which sets various variables for drawing map on grid
        private void SetCoordsAtMapViewChange(int[] proposedCentralCoords)
        {
            CentrXY = proposedCentralCoords;

            // For making an image of map part
            mapStartXY = new int[] { 0, 0 };   // Starting square for drawn map pic
            mapDrawSq = new int[] { 0, 0 }; // Squares to be drawn on map pic
            // For drawing map on panel
            mapSrc1 = mapSrc2 = new Rectangle(0, 0, 0, 0);  // Rectangle part of map pic to be drawn
            mapDest = new Point(0, 0);  // XY coords of where map should be drawn on panel (in px)

            int fullMapWidth = Map.Xpx * (2 * Map.XDim + 1);
            int fullMapHeight = Map.Ypx * (Map.YDim + 1);

            // No of squares of panel and map
            //int[] panelSq = { (int)Math.Ceiling((double)drawPanel.Width / Game.Xpx), (int)Math.Ceiling((double)drawPanel.Height / Game.Ypx) };
            int[] panelSq = PanelSq;
            centrOffset = new int[] { panelSq[0] / 2, panelSq[1] / 2 };
            if (centrOffset[0] % 2 != 1 && centrOffset[1] % 2 == 1) centrOffset[1]--;
            if (centrOffset[0] % 2 == 1 && centrOffset[1] % 2 != 1) centrOffset[0]--;

            // Number of drawn squares in both directions (in line with how game works). It's always multiple of 2 squares.
            mapDrawSq[0] = 2 * (int)Math.Floor((double)Math.Min(fullMapWidth, drawPanel.Width) / (2 * Map.Xpx));
            mapDrawSq[1] = 2 * (int)Math.Floor((double)Math.Min(fullMapHeight, drawPanel.Height) / (2 * Map.Ypx));

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
                else if (mapStartXY[1] + mapDrawSq[1] >= Map.YDim)
                {
                    mapStartXY[1] = Map.YDim - mapDrawSq[1];
                }
                else
                {
                    if (mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                }
            }
            else if (mapStartXY[0] + mapDrawSq[0] >= 2 * Map.XDim)
            {
                mapStartXY[0] = 2 * Map.XDim - mapDrawSq[0];
                if (mapStartXY[1] <= 0)
                {
                    mapStartXY[1] = 0;
                }
                else if (mapStartXY[1] + mapDrawSq[1] >= Map.YDim)
                {
                    mapStartXY[1] = Map.YDim - mapDrawSq[1];
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
                else if (mapStartXY[1] + mapDrawSq[1] >= Map.YDim)
                {
                    mapStartXY[1] = Map.YDim - mapDrawSq[1];
                    if (mapStartXY[0] % 2 != 0) mapStartXY[0]--;
                }
                else
                {
                    if (mapStartXY[0] % 2 == 0 && mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                    if (mapStartXY[0] % 2 != 0 && mapStartXY[1] % 2 == 0) mapStartXY[0]--;
                }
            }

            // Determine drawing rectangles
            if (panelSq[0] > 2 * Map.XDim + 1)
            {
                mapSrc1.Width = fullMapWidth;
                mapDest.X = (drawPanel.Width - fullMapWidth) / 2;
            }
            else
            {
                mapSrc1.Width = drawPanel.Width;
                mapDest.X = 0;
            }

            if (panelSq[1] > Map.YDim + 1)
            {
                mapSrc1.Height = fullMapHeight;
                mapDest.Y = (drawPanel.Height - fullMapHeight) / 2;
            }
            else
            {
                mapSrc1.Height = drawPanel.Height;
                mapDest.Y = 0;
            }

            Debug.WriteLine($"panelSq {panelSq[0]},{panelSq[1]}");
            Debug.WriteLine($"mapStartXY {mapStartXY[0]},{mapStartXY[1]}");
            Debug.WriteLine($"mapDrawSq X={mapDrawSq[0]} Y={mapDrawSq[1]}");
            Debug.WriteLine($"mapSrc1 {mapSrc1}");
            Debug.WriteLine($"mapDest {mapDest}");
            Debug.WriteLine("");
        }

        private int[] PanelSq => new int[] { (int)Math.Ceiling((double)drawPanel.Width / Map.Xpx), (int)Math.Ceiling((double)drawPanel.Height / Map.Ypx) };   // No of squares of panel and map
        private int[] ActiveOffsetXY => new int[] { Map.ActiveXY[0] - mapStartXY[0], Map.ActiveXY[1] - mapStartXY[1] };
        private Point ActiveOffsetPx => new Point(Map.Xpx * ActiveOffsetXY[0], Map.Ypx * ActiveOffsetXY[1]);
        private Point MapStartPx => new Point(Map.Xpx * mapStartXY[0], Map.Ypx * mapStartXY[1]);

        // Determine XY civ2 coords from x-y pixel location on panel
        private int[] PxToCoords(int x, int y, int zoom)
        {
            double[] nxy = new double[] { x - 2 * y, -(-y - 0.5 * x) };  //crossing at x,y-axis
            int[] nXY = new int[] { Convert.ToInt32(Math.Floor((nxy[0] + 4 * (8 + zoom)) / (8 * (8 + zoom)))), Convert.ToInt32(Math.Floor((nxy[1] - 2 * (8 + zoom)) / (4 * (8 + zoom)))) };   //converting crossing to int
            return new int[] { nXY[0] + nXY[1], nXY[1] - nXY[0] };
        }
    }
}
