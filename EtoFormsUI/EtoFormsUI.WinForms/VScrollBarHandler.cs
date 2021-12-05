using System;

namespace EtoFormsUI.WinForms
{   
    public class VScrollBarHandler : Eto.WinForms.Forms.WindowsControl<System.Windows.Forms.VScrollBar, VScrollBar, VScrollBar.ICallback>, VScrollBar.IHandler
    {
        public VScrollBarHandler()
        {
            this.Control = new System.Windows.Forms.VScrollBar();
            this.Control.ValueChanged += (sender, e) => Callback.OnValueChanged(Widget, EventArgs.Empty);
        }

        public override int Height
        {
            get { return this.Control.Height; }
            set { this.Control.Height = value; }
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
