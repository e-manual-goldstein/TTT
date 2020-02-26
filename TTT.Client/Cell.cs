using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace TTT.Client
{
    public class Cell
    {
        public int I;
        public int J;
        public static Marker Turn = Marker.X;
        public Cell(int i, int j)
        {
            I = i;
            J = j;

        }
        public static int Size = 250;
        public Marker? Value { get; set; }

        internal void ClickCell(object sender, EventArgs e)
        {
            if (Value == null)
            {
                Value = Turn;
                var button = (Button)sender;
                button.Text = Turn.ToString();
                SwapTurn();
            }
        }

        public override string ToString()
        {
            return $"{I}, {J}";
        }

        public void SwapTurn()
        {
            switch (Turn)
            {
                case Marker.X:
                    Turn = Marker.O;
                    break;
                case Marker.O:
                    Turn = Marker.X;
                    break;
                default:
                    break;
            }
        }
    }
}