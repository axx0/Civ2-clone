using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;
using Civ2engine.Units;
using Civ2engine.Enums;

namespace EtoFormsUI
{
    public class SelectUnitDialog : Civ2dialog
    {
        public int SelectedIndex;
        private readonly RadioButtonList radioBtnList;
        private readonly Drawable innerPanel;

        public SelectUnitDialog(Main parent, List<IUnit> units) : base(parent, 514 + 2 * 11, 46 * Math.Min(units.Count, 9) + 38 + 46, 38, 46, "Select Unit To Activate")
        {
            var shownUnits = units.Take(9).ToList();

            radioBtnList = new RadioButtonList()
            {
                DataStore = shownUnits,
                Orientation = Orientation.Vertical
            };
            radioBtnList.SelectedIndexChanged += (sender, e) => innerPanel.Invalidate();
            radioBtnList.GotFocus += (sender, e) => innerPanel.Invalidate(); 
            Layout.Add(radioBtnList, 11 + 10, 40);

            innerPanel = new Drawable() { Size = new Size(514, 46 * shownUnits.Count) };
            innerPanel.Paint += (sender, e) =>
            {
                e.Graphics.AntiAlias = false;

                // Background
                var imgSize = Images.PanelInnerWallpaper.Size;
                for (int row = 0; row < this.Height / imgSize.Height + 1; row++)
                {
                    for (int col = 0; col < this.Width / imgSize.Width + 1; col++)
                    {
                        e.Graphics.DrawImage(Images.PanelInnerWallpaper, col * imgSize.Width, row * imgSize.Height);
                    }
                }

                // Draw units, unit outline, text
                for (int row = 0; row < shownUnits.Count; row++)
                {
                    Draw.Unit(e.Graphics, shownUnits[row], false, -1, new Point(2, 2 + 46 * row));

                    string _homeCity = shownUnits[row].HomeCity == null ? "NONE" : shownUnits[row].HomeCity.Name;
                    Draw.Text(e.Graphics, $"{shownUnits[row].Owner.Adjective} {shownUnits[row].Name} ({_homeCity})", new Font("Times New Roman", 18), Color.FromArgb(51, 51, 51), new Point(61, 9 + 46 * row), false, false, Color.FromArgb(191, 191, 191), 1, 1);

                    using var _pen = new Pen(Color.FromArgb(223, 223, 223));
                    if (radioBtnList.SelectedIndex == row) e.Graphics.DrawRectangle(_pen, new Rectangle(1, 1 + 46 * row, 57, 42));
                }
            };
            innerPanel.MouseDown += (sender, e) =>
            {
                int clickedRow = ((int)e.Location.Y - 1) / 46;

                shownUnits[clickedRow].Order = OrderType.NoOrders;

                radioBtnList.SelectedIndex = clickedRow;
                innerPanel.Invalidate();
            };
            Layout.Add(innerPanel, 11, 38);

            // Buttons
            DefaultButton = new Civ2button("OK", 258, 36, new Font("Times new roman", 11));
            AbortButton = new Civ2button("Cancel", 258, 36, new Font("Times new roman", 11));
            Layout.Add(DefaultButton, 9, 42 + innerPanel.Height);
            Layout.Add(AbortButton, 11 + DefaultButton.Width, 42 + innerPanel.Height);

            DefaultButton.Click += (sender, e) =>
            {
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                SelectedIndex = radioBtnList.SelectedIndex;
                Close();
            };

            AbortButton.Click += (sender, e) =>
            {
                foreach (MenuItem item in parent.Menu.Items) item.Enabled = true;
                SelectedIndex = int.MinValue;
                Close();
            };

            Content = Layout;
        }
    }
}
