using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTciv2.Events
{
    public class MoveUnitCommandEventArgs : EventArgs
    {
        public MoveUnitCommandEventArgs(bool moveUnit)
        {
            MoveUnit = moveUnit;
        }

        public bool MoveUnit { get; set; }
    }
}
