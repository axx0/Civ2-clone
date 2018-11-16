using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2.Enums
{
    public enum UnitAction
    {
        Wait = 0,
        Fortify = 1,
        Fortified = 2,
        Sentry = 3,
        BuildFortress = 4,
        BuildRoadRR = 5,
        BuildIrrigation = 6,
        BuildMine = 7,
        TransformTerr = 8,
        CleanPollution = 9,
        BuildAirbase = 10,
        GoTo = 11,
        NoOrders = 255
    }
}
