using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using TTT.Common;

namespace TTT.Core
{
    public class GameGrid
    {
        Cell[,] _allCells;
        float _screenWidth;
        float _screenHeight;
        Func<float> _widthFunc;
        Func<float> _heightFunc;

        public GameGrid(float screenWidth, float screenHeight) 
        {
            
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            Canvas = new Canvas();
            Canvas.Width = screenWidth;
            Canvas.Height = screenHeight;
            
            _allCells = CreateCellArray();
        }

        public GameGrid(Func<float> widthFunc, Func<float> heightFunc) : this(widthFunc(), heightFunc())
        {
            _widthFunc = widthFunc;
            _heightFunc = heightFunc;

            _allCells = CreateCellArray();
        }

        public Canvas Canvas { get; set; }

        public Cell[,] GameCells 
        {
            get
            {
                return _allCells;
            }
        }

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

        public Cell[,] CreateCellArray()
        {
            var cellArray = new Cell[3, 3];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    cellArray[i,j] = new Cell(i, j);
                }
            }
            return cellArray;
        }


        public void DrawCells()
        {
            
            var baseX = (_widthFunc() - (3 * Constants.CellSizeHost)) / 2;
            var baseY = (_heightFunc() - (3 * Constants.CellSizeHost)) / 2;

            foreach (var cell in _allCells)
            {
                var cellCanvas = DrawCell(cell, baseX, baseY);
                Canvas.Children.Add(cellCanvas);
            }
        }

        public Canvas DrawCell(Cell cell, float baseX, float baseY)
        {
            var cellCanvas = new Canvas();
            cellCanvas.Width = Constants.CellSizeHost;
            cellCanvas.Height = Constants.CellSizeHost;
            Canvas.SetLeft(cellCanvas, baseX + (cell.I * Constants.CellSizeHost));
            Canvas.SetBottom(cellCanvas, baseY + (cell.J * Constants.CellSizeHost));
            cellCanvas.Background = new SolidColorBrush(Colors.Gray);
            cellCanvas.Children.Add(DrawTextBox(cell, Constants.CellSizeHost));
            return cellCanvas;
        }

        public TextBlock DrawTextBox(Cell cell, int size)
        {
            var textBlock = new TextBlock();
            textBlock.Height = size - 2;
            textBlock.Width = size - 2;
            textBlock.Background = new SolidColorBrush(Colors.White);
            Canvas.SetBottom(textBlock, 1);
            Canvas.SetLeft(textBlock, 1);
            cell.UpdateValue = (newValue) => textBlock.Text = newValue;
            return textBlock;
        }
    }   
}