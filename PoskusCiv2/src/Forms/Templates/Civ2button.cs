using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace RTciv2.Forms
{
    public partial class Civ2button : Button
    {
        public Civ2button()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.FromArgb(192, 192, 192);
            FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            SetStyle(ControlStyles.Selectable, false);  //Lose focus from button (cannot be selected by tab)
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), 0, 0, this.Width - 1, 0);   //1st layer of border
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), 0, 0, 0, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 2, 1);   //2nd layer of border
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), this.Width - 2, 2, this.Width - 2, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), 2, this.Height - 2, this.Width - 2, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 2, 2, this.Width - 3, 2);   //3rd layer of border
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 2, 2, 2, this.Height - 3);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), this.Width - 3, 3, this.Width - 3, this.Height - 3);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), 3, this.Height - 3, this.Width - 3, this.Height - 3);
            pe.Dispose();
        }
    }
}
