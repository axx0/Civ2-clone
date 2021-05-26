using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;
using Civ2engine;
using Civ2engine.Events;

namespace EtoFormsUI
{
    public partial class Main : Form
    {
        public void LoadGameInitialization(Ruleset ruleset, string SAVname)
        {
            Game.LoadGame(ruleset, SAVname);
            Images.LoadGraphicsAssetsFromFiles(ruleset);
        }

        public void LoadScenarioInit(Ruleset ruleset, string SCNname)
        {
            
        }
        
        public void StartPremadeInit(Ruleset ruleset, string SCNname)
        {
            
        }

        public void StartGame()
        {
            // Generate map tile graphics
            Images.MapTileGraphic = new Bitmap[Map.XDim, Map.YDim];
            for (int col = 0; col < Map.XDim; col++)
            {
                for (int row = 0; row < Map.YDim; row++)
                {
                    Images.MapTileGraphic[col, row] = Draw.MakeTileGraphic(Map.Tile[col, row], col, row, Game.Options.FlatEarth, MapImages.Terrains[Map.MapIndex]);
                }
            }
            
            foreach (MenuItem item in this.Menu.Items) item.Enabled = true;

            mapPanel = new MapPanel(this, ClientSize.Width - 262, ClientSize.Height);
            layout.Add(mapPanel, 0, 0);

            minimapPanel = new MinimapPanel(this, 262, 149);
            layout.Add(minimapPanel, ClientSize.Width - 262, 0);

            statusPanel = new StatusPanel(this, 262, ClientSize.Height - 148);
            layout.Add(statusPanel, ClientSize.Width - 262, 148);

            Content = layout;

            BringToFront();
        }

      

       
    }
}
