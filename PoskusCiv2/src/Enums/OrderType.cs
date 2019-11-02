using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2.Enums
{
    public enum OrderType
    {
        Fortify = 0,
        Fortified = 1,
        Sleep = 2,
        BuildFortress = 3,
        BuildRoad = 4,
        BuildIrrigation = 5,
        BuildMine = 6,
        Transform = 7,
        CleanPollution = 8,
        BuildAirbase = 9,
        GoTo = 10,
        NoOrders = 255
    }
}
