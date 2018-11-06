using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Resources;

namespace PoskusCiv2
{
    public static class Resources
    {
        public static void LoadResources(string civIIpath)
        {
            // Define a resource file for GIFs
            using (ResXResourceWriter resx = new ResXResourceWriter(@".\ResourcesGIF.resx"))
            {
                resx.AddResource("CITIESGIF", "Classic American Cars");
            }
        }
    }
}
