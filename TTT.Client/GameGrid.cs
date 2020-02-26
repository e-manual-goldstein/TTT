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

namespace TTT.Client
{
    public class GameGrid
    {
        Context _context;
        Cell[] _allCells;
        float _screenWidth;
        float _screenHeight;
       

        public GameGrid(Context context, float screenWidth, float screenHeight)
        {
            _context = context;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            
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

        public void DrawCells()
        {
            var baseLayout = new FrameLayout.LayoutParams(Cell.Size, Cell.Size);
            var baseX = (_screenWidth - (3 * Cell.Size)) / 2;
            var baseY = (_screenHeight - (3 * Cell.Size)) / 2;
            foreach (var cell in _allCells)
            {
                var button = new Button(_context);
                button.LayoutParameters = baseLayout;
                var x = baseX + (cell.I * Cell.Size);
                var y = baseY + (cell.J * Cell.Size);
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
    }   
}