using System;

namespace EtoFormsUI.WinForms
{   
    public class HScrollBarHandler : Eto.WinForms.Forms.WindowsControl<System.Windows.Forms.HScrollBar, HScrollBar, HScrollBar.ICallback>, HScrollBar.IHandler
    {
        public HScrollBarHandler()
        {
            this.Control = new System.Windows.Forms.HScrollBar();
            this.Control.LargeChange = 1;
            this.Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
        }

        public override int Width
        {
            get { return this.Control.Width; }
            set { this.Control.Width = value; }
        }

        public int Value
        {
            get { return this.Control.Value; }
            set { this.Control.Value = value; }
        }

        public int Maximum
        {
            get { return this.Control.Maximum; }
            set { this.Control.Maximum = value; }
        }
    }
}
