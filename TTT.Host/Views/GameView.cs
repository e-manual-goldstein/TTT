using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TTT.Common;

namespace TTT.Host
{
    public class GameView : View//<Game>
    {
        Game _model;

        public GameView(Game game)
        {
            _model = game;
            Content = new Canvas();
        }

        public override void Show()
        {
            DrawCells(_model.GameCells);
        }

        public override void SizeChanged(Size newSize)
        {
            
        }

        private Canvas DrawCells(Cell[,] cellGrid)
        {            
            var canvases = new Dictionary<Cell, Canvas>();

            foreach (var cell in cellGrid)
            {
                canvases[cell] = DrawCell(cell);
                Content.Children.Add(canvases[cell]);
            }
            Content.SizeChanged += (object sender, SizeChangedEventArgs eventArgs) =>
            {
                foreach (var cell in cellGrid)
                {
                    TransformCell(canvases[cell], cell, (float)eventArgs.NewSize.Width, (float)eventArgs.NewSize.Height);
                }
            };
            return Content;
        }

        private Canvas DrawCell(Cell cell)
        {
            var cellCanvas = new Canvas();
            cellCanvas.Width = Constants.CellSizeHost;
            cellCanvas.Height = Constants.CellSizeHost;
            TransformCell(cellCanvas, cell, 800, 600);
            cellCanvas.Background = new SolidColorBrush(Colors.Gray);
            cellCanvas.Children.Add(DrawTextBox(cell, Constants.CellSizeHost));
            return cellCanvas;
        }

        private void TransformCell(Canvas cellCanvas, Cell cell, float newX, float newY)
        {
            var baseX = (newX - (3 * Constants.CellSizeHost)) / 2;
            var baseY = (newY - (3 * Constants.CellSizeHost)) / 2;
            Canvas.SetLeft(cellCanvas, baseX + (cell.I * Constants.CellSizeHost));
            Canvas.SetTop(cellCanvas, baseY + (cell.J * Constants.CellSizeHost));
        }

        private Label DrawTextBox(Cell cell, int size)
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
            RegisterUpdatingAction(cell, (c) =>
            {
                label.Content = ((Cell)c).Marker;
                if (((Cell)c).Active)
                    label.Background = new SolidColorBrush(Colors.Tomato);
            });
            //cell.SetUpdateValueAction((newValue) => UpdateCellValue(label, newValue, () => cell.Active));
            return label;
        }

        //private void UpdateCellValue(Label label, Marker newValue, Func<bool> active)
        //{
        //    App.Current.Dispatcher.Invoke(() =>
        //    {
        //        label.Content = newValue.ToString();
        //        if (active())
        //            label.Background = new SolidColorBrush(Colors.Tomato);
        //    });
        //}
    }
}
