using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace EtoFormsUI
{
    // control to use in your eto.forms code
    [Handler(typeof(CustomEtoButton.IHandler))]
    public class CustomEtoButton : Button
    {
        new IHandler Handler { get { return (IHandler)base.Handler; } }

        public Image BackgroundImage
        {
            get { return Handler.BackgroundImage; }
            set { Handler.BackgroundImage = value; }
        }

        public new interface IHandler : Button.IHandler
        {
            Image BackgroundImage { get; set; }
        }
    }
}
