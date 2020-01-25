using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2.Enums
{
    public enum OrderType
    {
        Fortify = 1,
        Fortified = 2,
        Sleep = 3,
        BuildFortress = 4,
        BuildRoad = 5,
        BuildIrrigation = 6,
        BuildMine = 7,
        Transform = 8,
        CleanPollution = 9,
        BuildAirbase = 10,
        GoTo = 11,
        NoOrders = 255
    }
}
