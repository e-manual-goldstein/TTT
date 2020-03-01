using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTT.Core
{
    public class Cell
    {
        public int I;
        public int J;

        public Cell(int i, int j)
        {
            I = i;
            J = j;

        }
        public static int Size = 80;
        public Marker? Value { get; set; }
        public Action<string> UpdateValue { get; internal set; }

        public override string ToString()
        {
            return $"{I}, {J}";
        }


    }
}