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
        

        public Cell(int i, int j, Marker? marker = null)
        {
            I = i;
            J = j;
            Marker = marker;
        }

        public Marker? Marker { get; set; }
        
        public bool Active { get; set; }

        public void ClickCell(object sender, EventArgs e)
        {
            if (Marker == null)
            {
                _takeTurnAction(this);
            }
        }

        public override string ToString()
        {
            return $"{I}, {J}";
        }


        //Action<Marker> _updateValueAction;
        //public void SetUpdateValueAction(Action<Marker> action)
        //{
        //    _updateValueAction = action;
        //}

        public void UpdateValue(Marker marker)
        {
            Marker = marker;
            //_updateValueAction(marker);
        }

        Action<Cell> _takeTurnAction;
        public void SetTakeTurnAction(Action<Cell> action)
        {
            _takeTurnAction = action;
        }

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