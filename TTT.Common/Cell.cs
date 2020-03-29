using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TTT.Common
{
    public class Cell
    {
        public int I;
        public int J;
        public static Marker Turn = Common.Marker.X;

        public Cell(int i, int j)
        {
            I = i;
            J = j;
        }

        public Marker? Marker { get; set; }

        public void ClickCell(object sender, EventArgs e)
        {
            if (Marker == null)
            {
                Marker = Turn;
                TakeTurnAction(this);
            }
        }

        public override string ToString()
        {
            return $"{I}, {J}";
        }

        

        public Action<Marker> UpdateValue { get; set; }

        public Action<Cell> TakeTurnAction { internal get; set; }


        public void SwapTurn()
        {
            switch (Turn)
            {
                case Common.Marker.X:
                    Turn = Common.Marker.O;
                    break;
                case Common.Marker.O:
                    Turn = Common.Marker.X;
                    break;
                default:
                    break;
            }
        }
    }
}