using Eto;
using Eto.GtkSharp;
using Eto.Drawing;

namespace EtoFormsUI.Gtk
{   
    public class CustomEtoButtonHandler : Eto.GtkSharp.Forms.Controls.ButtonHandler, CustomEtoButton.IHandler
    {
        public CustomEtoButtonHandler()
        {
        }

        // TODO: add this property
        public Image BackgroundImage 
        {
            get;set;
        }
    }
}
