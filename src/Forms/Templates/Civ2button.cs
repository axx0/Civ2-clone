using System.Windows.Forms;
using System.Drawing;

namespace civ2.Forms
{
    public class Civ2button : Button
    {
        public Civ2button()
        {
            FlatStyle = FlatStyle.Flat;
            BackColor = Color.FromArgb(192, 192, 192);
            FlatAppearance.MouseOverBackColor = Color.FromArgb(192, 192, 192);
            SetStyle(ControlStyles.Selectable, false);  // Lose focus from button (cannot be selected by tab)
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            // Play with this to make blurry text like in the original
            //using var sf = new StringFormat();
            //sf.LineAlignment = StringAlignment.Center;
            //sf.Alignment = StringAlignment.Center;
            //pe.Graphics.DrawString("Info", new Font("Arial", 9), new SolidBrush(Color.FromArgb(223, 223, 223)), new Point(Width / 2 + 1, Height / 2), sf);
            //pe.Graphics.DrawString("Info", new Font("Arial", 9), new SolidBrush(Color.Black), new Point(Width / 2, Height / 2), sf);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), 0, 0, this.Width - 1, 0);   // 1st layer of border
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), 0, 0, 0, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), this.Width - 1, 0, this.Width - 1, this.Height - 1);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(100, 100, 100)), 0, this.Height - 1, this.Width - 1, this.Height - 1);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, this.Width - 2, 1);   // 2nd layer of border
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 1, 1, 1, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), this.Width - 2, 2, this.Width - 2, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), 2, this.Height - 2, this.Width - 2, this.Height - 2);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 2, 2, this.Width - 3, 2);   // 3rd layer of border
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(255, 255, 255)), 2, 2, 2, this.Height - 3);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), this.Width - 3, 3, this.Width - 3, this.Height - 3);
            pe.Graphics.DrawLine(new Pen(Color.FromArgb(128, 128, 128)), 3, this.Height - 3, this.Width - 3, this.Height - 3);
            pe.Dispose();
        }
    }
}
