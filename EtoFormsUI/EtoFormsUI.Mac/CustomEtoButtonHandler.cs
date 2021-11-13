using Eto;
using Eto.Mac;
using Eto.Drawing;

namespace EtoFormsUI.Mac
{   
    public class CustomEtoButtonHandler : Eto.Mac.Forms.Controls.ButtonHandler, CustomEtoButton.IHandler
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
