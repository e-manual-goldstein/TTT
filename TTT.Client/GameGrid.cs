using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class GameGrid
    {
        Context _context;
        Cell[] _allCells;
        float _screenWidth;
        float _screenHeight;
        Func<float> _widthFunc;
        Func<float> _heightFunc;

        public GameGrid(Context context, float screenWidth, float screenHeight)
        {
            _context = context;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            
            _allCells = CreateCells();

        }

        public GameGrid(Context context, Func<float> widthFunc, Func<float> heightFunc)
        {
            _context = context;
            _widthFunc = widthFunc;
            _heightFunc = heightFunc;

            _allCells = CreateCells();
        }

        public FrameLayout FrameLayout { get; set; }

        public Cell[] CreateCells()
        {
            var cells = new List<Cell>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = new Cell(i, j);
                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }

        //public void DrawCells()
        //{
        //    var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
        //    var baseX = (_screenWidth - (3 * Constants.CellSizeClient)) / 2;
        //    var baseY = (_screenHeight - (3 * Constants.CellSizeClient)) / 2;
        //    foreach (var cell in _allCells)
        //    {
        //        var button = new Button(_context);
        //        button.LayoutParameters = baseLayout;
        //        var x = baseX + (cell.I * Constants.CellSizeClient);
        //        var y = baseY + (cell.J * Constants.CellSizeClient);
        //        button.SetX(x);
        //        button.SetY(y);
        //        button.SetBackgroundColor(Color.Gray);
        //        button.SetTextColor(Color.White);
        //        button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
        //        button.Text = cell.Value.ToString();
        //        button.Click += cell.ClickCell;
        //        FrameLayout.AddView(button);
        //    }
        //}

        public void DrawCells_Func()
        {
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (_widthFunc() - (3 * Constants.CellSizeClient)) / 2;
            var baseY = (_heightFunc() - (3 * Constants.CellSizeClient)) / 2;
            foreach (var cell in _allCells)
            {
                var button = new Button(_context);
                button.LayoutParameters = baseLayout;
                var x = baseX + (cell.I * Constants.CellSizeClient);
                var y = baseY + (cell.J * Constants.CellSizeClient);
                button.SetX(x);
                button.SetY(y);
                button.SetBackgroundColor(Color.Gray);
                button.SetTextColor(Color.White);
                button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
                button.Text = cell.Value.ToString();
                button.Click += cell.ClickCell;
                FrameLayout.AddView(button);
            }
        }

        public void TakeTurn(Cell cell)
        {

        }
    }   
}