using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine;
using Civ2engine.Enums;
using Civ2engine.Events;
using Civ2engine.MapObjects;
using Civ2engine.Units;
using EtoFormsUI.Animations;

namespace EtoFormsUI
{
    public class MapPanel : Civ2panel
    {
        private Game Game => Game.Instance;
        
        private Main main;
        private readonly Action<int[], int[]> _updateMiniMap;
        private readonly UITimer animationTimer;   // Timer for blinking (unit or viewing piece), moving unit, etc.
        public Drawable drawPanel;

        public int[] CentrXY, centrOffset;
        private Rectangle mapSrc1;
        private int[] mapStartXY,  mapDrawSq;
        private Point mapDest;
        private bool updateMap;
        private CityWindow cityWindow;
        public Point CityWindowLocation;
        public int CityWindowZoom;

        private readonly Queue<IAnimation> _animationQueue = new();

        public IAnimation CurrentAnimation;

        private Bitmap mapImage;
        private bool _longHold;
        private int[] _mouseDownTile;
        private readonly Timer _clickTimer;

        public MapPanel(Main parent, int width, int height, Action<int[], int[]> updateMiniMap) : base(parent.ActiveInterface.Look,width, height,
            38, 10)
        {
            main = parent;
            _updateMiniMap = updateMiniMap;

            drawPanel = new Drawable() {  Size = new Size(MainPanel.Width - 2 * 11, MainPanel.Height - 38 - 10), BackgroundColor = Colors.Black };
            
            drawPanel.Paint += DrawPanel_Paint;
            drawPanel.MouseDown += DrawPanel_MouseDowm;
            drawPanel.MouseUp += DrawPanel_MouseUp;
            MainPanelLayout.Add(drawPanel, 11, 38);
            MainPanel.Content = MainPanelLayout;

            MainPanel.Paint += (sender, e) => Draw.Text(e.Graphics, $"{Game.GetActiveCiv.Adjective} Map", new Font("Times new roman", 17, FontStyle.Bold), Color.FromArgb(135, 135, 135), new Point(MainPanel.Width / 2, 38 / 2), true, true, Colors.Black, 1, 1);

            Game.OnUnitEvent += UnitEventHappened;
            Game.OnMapEvent += MapEventHappened;
            parent.OnMapEvent += MapEventHappened;

            _clickTimer = new Timer{ AutoReset = false, Interval = 500};
            _clickTimer.Elapsed += MouseHeldTime_Elapsed;

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
            CityWindowLocation = new Point((this.Width / 2) - (636 / 2 * (2 + CityWindowZoom) / 2 + 2 * 11), (this.Height / 2) - (421 / 2 * (2 + CityWindowZoom) / 2 + 11 + (CityWindowZoom switch
            {
                -1 => 21,
                0 => 27,
                _ => 39
            })));

            // Starting animation
            animationTimer = new UITimer(); // Timer for waiting unit/ viewing piece
            animationTimer.Elapsed += OnAnimationElapsedEvent;
            animationTimer.Start();

            // Center the map view and draw map
            MapViewChange(Game.CurrentMap.StartingClickedXY ?? Game.GetPlayerCiv.Units[0].XY);
            
            SizeChanged += OnSizeChanged;
        }

        private void OnSizeChanged(object sender, EventArgs e)
        {
            var drawWidth = MainPanel.Width - 2 * 11;
                var drawHeight = MainPanel.Height - 38 - 10;
            if (drawWidth > 0 && drawHeight > 0)
            {
                drawPanel.Size = new Size(drawWidth, drawHeight);
            }
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
                e.Graphics.DrawImage(mapImage, mapSrc1, mapDest);
                updateMap = false;
            }
            // Draw animation
            else
            {
                if(CurrentAnimation == null) return;
                var map = Game.CurrentMap;
                e.Graphics.DrawImage(CurrentAnimation.CurrentFrame,  CurrentAnimation.GetXDrawOffset(map.Xpx, mapStartXY[0]), CurrentAnimation.GetYDrawOffset(map.Ypx, mapStartXY[1]) + mapDest.Y );
            }
        }

        public void ShowCityWindow(City city)
        {
            if (cityWindow != null)
            {
                CityWindowLocation = cityWindow.Location;
                cityWindow.Close();
            }
                    
            cityWindow = new CityWindow(main, this, city, CityWindowZoom);
            cityWindow.Show();
        }

        public bool ActivateUnits(Tile tile)
        {
            var unitsHere = tile.UnitsHere;
            if (unitsHere.Count == 0) return true;

            var unit = unitsHere[0];
            if (unitsHere.Count > 1)
            {
                // Multiple units on this square => open unit selection dialog

                var selectUnitDialog = new SelectUnitDialog(main, unitsHere);
                selectUnitDialog.ShowModal(main);

                if (selectUnitDialog.SelectedIndex < 0)
                {
                    return false;
                }

                unit = unitsHere[selectUnitDialog.SelectedIndex];
            }

            
            if (!unit.TurnEnded)
            {
                main.CurrentPlayer.ActiveUnit = unit;
            }

            unit.Order = OrderType.NoOrders; // Always clear order when clicked, no matter if the unit is activated
            main.CurrentGameMode = main.Moving;
            return true;
        }

        private void DrawPanel_MouseDowm(object sender, MouseEventArgs e)
        {
            // If clicked location is beyond map limits --> exit method
            if (e.Location.X < mapDest.X || e.Location.X > mapDest.X + mapSrc1.Width || e.Location.Y < mapDest.Y || e.Location.Y > mapDest.Y + mapSrc1.Height) return;

            _clickTimer.Start();
            var clickedXy = PxToCoords((int)e.Location.X - mapDest.X, (int)e.Location.Y - mapDest.Y, Game.CurrentMap.Zoom);
            clickedXy[0] += mapStartXY[0];
            clickedXy[1] += mapStartXY[1];
            
            Debug.WriteLine($"clickedXY={clickedXy[0]},{clickedXy[1]}");

            if (Game.Instance.CurrentMap.IsValidTileC2(clickedXy[0], clickedXy[1]))
            {
                _mouseDownTile = clickedXy;
            }
        }
        
        
        private void MouseHeldTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            _longHold = true;
            //TODO: Change the cursor to GOTO if in CurrentGameMode == Moving
        }

        private void DrawPanel_MouseUp(object sender, MouseEventArgs e)
        {
            _clickTimer.Stop();
            // If clicked location is beyond map limits --> exit method
            if (e.Location.X < mapDest.X || e.Location.X > mapDest.X + mapSrc1.Width || e.Location.Y < mapDest.Y || e.Location.Y > mapDest.Y + mapSrc1.Height)
            {
                _mouseDownTile = null;
                _longHold = false;
                return;
            }
            // Else you clicked within the map

            
            var clickedXY = PxToCoords((int)e.Location.X - mapDest.X, (int)e.Location.Y - mapDest.Y, Game.CurrentMap.Zoom);
            clickedXY[0] += mapStartXY[0];
            clickedXY[1] += mapStartXY[1];
            
            Debug.WriteLine($"clickedXY={clickedXY[0]},{clickedXY[1]}");

            if (Game.Instance.CurrentMap.IsValidTileC2(clickedXY[0], clickedXY[1]))
            {
                // TODO: Make sure that edge black tiles are also ignored!
                var tile = Game.Instance.CurrentMap.TileC2(clickedXY[0], clickedXY[1]);
                if (main.CurrentGameMode.MapClicked(tile, this, main, _longHold, e, _mouseDownTile))
                {
                    MapViewChange(clickedXY);
                }
            }

            _longHold = false;
        }

        /// <summary>
        /// This is currently a shim method as the refactoring progresses 
        /// </summary>
        /// <param name="centralItem"></param>
        private void MapViewChange(Tile centralItem)
        {
            var xy = new int[] { centralItem.X, centralItem.Y };
            MapViewChange(xy);
        }
        
        internal void MapViewChange(int[] newCenterCoords)
        {
            //if (map != null) map.Dispose();
            SetCoordsAtMapViewChange(newCenterCoords);
            //map = Draw.MapPart(Game.GetActiveCiv.Id, mapStartXY[0], mapStartXY[1], mapDrawSq[0], mapDrawSq[1], Game.Options.FlatEarth, Map.MapRevealed, main.CurrentGameMode != main.Moving);
            UpdateMap();
            _updateMiniMap(mapStartXY, mapDrawSq);
        }

        public void UpdateMap()
        {
            updateMap = true;
            mapImage?.Dispose();
            var map = Game.CurrentMap;
            mapImage = Draw.MapPart(map.WhichCivsMapShown, mapStartXY[0], mapStartXY[1], mapDrawSq[0], mapDrawSq[1], Game.Options.FlatEarth, map.MapRevealed, main.CurrentGameMode != main.Moving, PathTiles, PathDebug);
            drawPanel.Invalidate();
        }

        #region Zoom events
        public void ZoomINclicked(Object sender, EventArgs e)
        {
            var map = Game.CurrentMap;
            if (map.Zoom != 8)
            {
                map.Zoom++;
                MapViewChange(CentrXY);
            }
        }

        public void ZoomOUTclicked(Object sender, EventArgs e)
        {
            var map = Game.CurrentMap;
            if (map.Zoom != 8)
            {
                map.Zoom--;
                MapViewChange(CentrXY);
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
                case MapEventType.ToggleBetweenCurrentEntireMapView:
                    {
                        drawPanel.Update(new Rectangle(0, 0, drawPanel.Width, drawPanel.Height));
                        break;
                    }
                case MapEventType.ZoomChanged:
                    {
                        MapViewChange(CentrXY);
                        break;
                    }
                case MapEventType.CenterView:
                {
                    MapViewChange(Game.ActiveTile);
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
                case MapEventType.UpdateMap:
                    {
                        e.TilesChanged?.ForEach(Images.RedrawTile) ;
                        UpdateMap();
                        break;
                    }
                default: break;
            }
        }

        private void UnitEventHappened(object sender, UnitEventArgs e)
        {
            if (!e.Location.Any(Game.CurrentMap.IsCurrentlyVisible))
            {
                return;
            }
            switch (e.EventType)
            {
                // Unit movement animation event was raised
                case UnitEventType.MoveCommand:
                {
                    if (e is MovementEventArgs mo)
                    {
                        _animationQueue.Enqueue(new MoveAnimation(mo, main.Sounds));
                    }

                    break;
                }
                case UnitEventType.Attack:
                {
                    if (e is CombatEventArgs combatEventArgs)
                    {
                        _animationQueue.Enqueue(new AttackAnimation(combatEventArgs, main.Sounds));
                    }


                    // animationFrames = GetAnimationFrames.UnitAttack(e);
                    // StartAnimation(AnimationType.Attack);
                    break;
                }
                // case UnitEventType.StatusUpdate:
                //     {
                //         animType = AnimationType.Waiting;
                //         if (IsActiveSquareOutsideMapView) MapViewChange(Map.ActiveXY);
                //         UpdateMap();
                //         break;
                //     }
            }
        }
        

        #region Animation

        private void OnAnimationElapsedEvent(object sender, EventArgs e)
        {
            if (CurrentAnimation == null || CurrentAnimation.Finished())
            {
                CurrentAnimation = _animationQueue.Count > 0 ? _animationQueue.Dequeue() : main.CurrentGameMode.GetDefaultAnimation(Game,CurrentAnimation);
                if (CurrentAnimation != null)
                {
                    CurrentAnimation.Initialize();
                    animationTimer.Interval = CurrentAnimation.Interval;
                }
            }
            
            if(CurrentAnimation == null) return;
            if (IsActiveSquareOutsideMapView(CurrentAnimation.Location))
            {
                if (!CurrentAnimation.Recycled)
                {
                    MapViewChange(CurrentAnimation.Location);
                }
            }
            else
            {
                var map = CurrentAnimation.Location.Map;
                drawPanel.Update(new Rectangle(map.Xpx * (CurrentAnimation.Location.X - mapStartXY[0]),
                    map.Ypx * (CurrentAnimation.Location.Y - mapStartXY[1]) - CurrentAnimation.YAdjustment,
                    CurrentAnimation.Width * map.Xpx, CurrentAnimation.Height * map.Ypx));
            }
        }
        #endregion

        private bool IsActiveSquareOutsideMapView(Tile tile)
        {
            var panelSquare = GetPanelSquare(tile.Map);
            return tile.X >= mapStartXY[0] + panelSquare[0] - 2 ||
                   tile.X <= mapStartXY[0] ||
                   tile.Y >= mapStartXY[1] + panelSquare[1] - 2 ||
                   tile.Y <= mapStartXY[1];
        }

        // Function which sets various variables for drawing map on grid
        private void SetCoordsAtMapViewChange(int[] proposedCentralCoords)
        {
            
            var map = Game.CurrentMap;
            CentrXY = proposedCentralCoords;

            // For making an image of map part
            mapStartXY = new int[] { 0, 0 };   // Starting square for drawn map pic
            mapDrawSq = new int[] { 0, 0 }; // Squares to be drawn on map pic
            // For drawing map on panel
            mapSrc1 = new Rectangle(0, 0, 0, 0);  // Rectangle part of map pic to be drawn
            mapDest = new Point(0, 0);  // XY coords of where map should be drawn on panel (in px)

            int fullMapWidth = map.Xpx * (map.XDimMax + 1);
            int fullMapHeight = map.Ypx * (map.YDim + 1);

            // No of squares of panel and map
            //int[] panelSq = { (int)Math.Ceiling((double)drawPanel.Width / Game.Xpx), (int)Math.Ceiling((double)drawPanel.Height / Game.Ypx) };
            var panelSq = GetPanelSquare(map);
            centrOffset = new[] { panelSq[0] / 2, panelSq[1] / 2 };
            if (centrOffset[0] % 2 != 1 && centrOffset[1] % 2 == 1) centrOffset[1]--;
            if (centrOffset[0] % 2 == 1 && centrOffset[1] % 2 != 1) centrOffset[0]--;

            // Number of drawn squares in both directions (in line with how game works). It's always multiple of 2 squares.
            mapDrawSq[0] = 2 * (int)Math.Floor((double)Math.Min(fullMapWidth, drawPanel.Width) / (2 * map.Xpx));
            mapDrawSq[1] = 2 * (int)Math.Floor((double)Math.Min(fullMapHeight, drawPanel.Height) / (2 * map.Ypx));

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
                else if (mapStartXY[1] + mapDrawSq[1] >= map.YDim)
                {
                    mapStartXY[1] = map.YDim - mapDrawSq[1];
                }
                else
                {
                    if (mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                }
            }
            else if (mapStartXY[0] + mapDrawSq[0] >= map.XDimMax)
            {
                mapStartXY[0] = map.XDimMax - mapDrawSq[0];
                if (mapStartXY[1] <= 0)
                {
                    mapStartXY[1] = 0;
                }
                else if (mapStartXY[1] + mapDrawSq[1] >= map.YDim)
                {
                    mapStartXY[1] = map.YDim - mapDrawSq[1];
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
                else if (mapStartXY[1] + mapDrawSq[1] >= map.YDim)
                {
                    mapStartXY[1] = map.YDim - mapDrawSq[1];
                    if (mapStartXY[0] % 2 != 0) mapStartXY[0]--;
                }
                else
                {
                    if (mapStartXY[0] % 2 == 0 && mapStartXY[1] % 2 != 0) mapStartXY[1]--;
                    if (mapStartXY[0] % 2 != 0 && mapStartXY[1] % 2 == 0) mapStartXY[0]--;
                }
            }

            // Determine drawing rectangles
            if (panelSq[0] > map.XDimMax + 1)
            {
                mapSrc1.Width = fullMapWidth;
                mapDest.X = (drawPanel.Width - fullMapWidth) / 2;
            }
            else
            {
                mapSrc1.Width = drawPanel.Width;
                mapDest.X = 0;
            }

            if (panelSq[1] > map.YDim + 1)
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

        /// <summary>
        /// No of squares of panel and map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private int[] GetPanelSquare(Map map) => new[]
            { (int)Math.Ceiling((double)drawPanel.Width / map.Xpx), (int)Math.Ceiling((double)drawPanel.Height / map.Ypx) };

        public IList<Tile> PathTiles { get; set; }
        public Dictionary<Tile, Route> PathDebug { get; set; }

        // Determine XY civ2 coords from x-y pixel location on panel
        private int[] PxToCoords(int x, int y, int zoom)
        {
            double[] nxy = new double[] { x - 2 * y, -(-y - 0.5 * x) };  //crossing at x,y-axis
            int[] nXY = new int[] { Convert.ToInt32(Math.Floor((nxy[0] + 4 * (8 + zoom)) / (8 * (8 + zoom)))), 
                Convert.ToInt32(Math.Floor((nxy[1] - 2 * (8 + zoom)) / (4 * (8 + zoom)))) };   //converting crossing to int
            return new int[] { nXY[0] + nXY[1], nXY[1] - nXY[0] };
        }
    }
}
