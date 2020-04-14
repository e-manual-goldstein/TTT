using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TTT.Core;

namespace TTT.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(GameGrid gameGrid)
        {
            InitializeComponent();


            gameGrid.DrawCells((float)Width, (float)Height);
            Content = gameGrid.Canvas;
            AddConnectButton(gameGrid.Canvas);
            AddStartButton(gameGrid.Canvas);
        }

        private void AddConnectButton(Canvas canvas)
        {
            var button = new Button();
            button.Content = "Connect";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 20);
            button.Click += ConnectButton_Click;
            canvas.Children.Add(button);
        }

        private void AddStartButton(Canvas canvas)
        {
            var button = new Button();
            button.Content = "Start";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 50);
            button.Click += StartButton_Click;
            canvas.Children.Add(button);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            ConnectButtonAction();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            StartButtonAction();
        }

        public Action ConnectButtonAction { get; set; }
        public Action StartButtonAction { get; set; }
    }
}
