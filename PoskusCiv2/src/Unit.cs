using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoskusCiv2
{
    class Unit
    {
        public string type { get; set; }
        public string until { get; set; }
        public int domain { get; set; }

        public Unit()
        {
            type = "Settlers";
            until = "Exp";
            domain = 1;
        }
    }
}
