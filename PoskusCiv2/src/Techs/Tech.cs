using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RTciv2.Enums;

namespace RTciv2.Techs
{
    internal class Tech : ITech
    {
        //From RULES.TXT
        public string Name { get; set; }
        public int AIvalue { get; set; }
        public int Modifier { get; set; }
        public TechType Prereq1 { get; set; }
        public TechType Prereq2 { get; set; }
        public int Epoch { get; set; }
        public int Category { get; set; }

        public TechType Type { get; set; }
        public int Id => (int)Type;

        //When making a new tech
        public Tech(TechType type)
        {
            Name = ReadFiles.TechName[(int)(type)];
            AIvalue = ReadFiles.TechAIvalue[(int)(type)];
            Modifier = ReadFiles.TechModifier[(int)(type)];
            //find correct Prereq1 TO-DO
            //find correct Prereq2 TO-DO
            Epoch = ReadFiles.TechEpoch[(int)(type)];
            Category = ReadFiles.TechCategory[(int)(type)];
        }
    }
}
