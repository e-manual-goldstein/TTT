using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TTT.Common;

namespace TTT.Host
{
    public class GameView : View//<Game>
    {
        Game _model;

        public GameView(Game game, IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _model = game;
            Content = new Canvas();
        }

        public override void Show()
        {
            DrawCells(_model.GameCells);
            AddResetButton();
        }

        private void AddResetButton()
        {
            var resetButton = new Button();
            resetButton.Height = 20;
            resetButton.Width = 80;
            resetButton.Content = "Reset";
            Canvas.SetTop(resetButton, 50);
            Canvas.SetLeft(resetButton, 500);
            resetButton.Click += (object sender, RoutedEventArgs eventArgs) =>
            {
                GetService<GameManager>().CreateNewGame();
                var menu = GetService<MainMenu>();
                menu.CreateActions();
                var menuView = new MainMenuView(menu, _serviceProvider);
                GetService<ViewManager>().SetContent(menuView);
                menuView.Show();
            };
            Content.Children.Add(resetButton);
        }

        public override void SizeChanged(Size newSize)
        {
            
        }

        private Canvas DrawCells(Cell[] cellGrid)
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
    }
}
