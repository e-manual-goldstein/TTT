using System;
using System.Collections.Generic;
using System.Windows;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TTT.Common;
using TTT.Host;

namespace TTT.Core
{
    public class GameGrid
    {
        Cell[,] _allCells;
        float _screenWidth;
        float _screenHeight;
        Func<float> _widthFunc;
        Func<float> _heightFunc;

        public GameGrid(float screenWidth, float screenHeight) : this()
        {
            
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            Canvas = new Canvas();
            Canvas.Width = screenWidth;
            Canvas.Height = screenHeight;
            
        }

        public GameGrid(Func<float> widthFunc, Func<float> heightFunc) : this(widthFunc(), heightFunc())
        {
            _widthFunc = widthFunc;
            _heightFunc = heightFunc;
        }

        public GameGrid()
        {
            _allCells = CreateCellArray();
            Canvas = new Canvas();
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


        public void DrawCells(float width, float height)
        {
            Canvas.Width = width;
            Canvas.Height = height;

            var baseX = (width - (3 * Constants.CellSizeHost)) / 2;
            var baseY = (height - (3 * Constants.CellSizeHost)) / 2;

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
            Canvas.SetTop(cellCanvas, baseY + (cell.J * Constants.CellSizeHost));
            cellCanvas.Background = new SolidColorBrush(Colors.Gray);
            cellCanvas.Children.Add(DrawTextBox(cell, Constants.CellSizeHost));
            return cellCanvas;
        }

        public Label DrawTextBox(Cell cell, int size)
        {
            var label = new Label();
            label.Height = size - 2;
            label.Width = size - 2;
            label.Background = new SolidColorBrush(Colors.White);
            label.FontSize = 40;
            label.HorizontalContentAlignment = HorizontalAlignment.Center;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            Canvas.SetBottom(label, 1);
            Canvas.SetLeft(label, 1);
            cell.UpdateValue = (newValue) => UpdateCellValue(label, newValue);
            return label;
        }

        public void UpdateCellValue(Label label, Marker newValue)
        {
            App.Current.Dispatcher.Invoke(() => 
            {
                label.Content = newValue.ToString();
            });
        }
    }   
}