using Eto;
using Eto.WinForms;
using Eto.Drawing;

namespace EtoFormsUI.WinForms
{   
    public class CustomEtoButtonHandler : Eto.WinForms.Forms.Controls.ButtonHandler, CustomEtoButton.IHandler
    {
        public CustomEtoButtonHandler()
        {
            this.Control.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Control.FlatAppearance.BorderSize = 0;
            this.Control.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(100, 100, 100);
        }

        static readonly object Image_Key = new object();

        public Image BackgroundImage 
        {
            get { return Widget.Properties.Get<Image>(Image_Key); }
            set { Widget.Properties.Set(Image_Key, value, () => this.Control.BackgroundImage = value.ToSD()); }
        }
    }
}
