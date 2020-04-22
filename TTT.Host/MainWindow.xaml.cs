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

namespace TTT.Host
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        public void AddConnectButton(Panel canvas)
        {
            var button = new Button();
            button.Content = "Connect";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 20);
            button.Click += ConnectButton_Click;
            canvas.Children.Add(button);
        }

        public void AddStartButton(Panel canvas)
        {
            var button = new Button();
            button.Content = "Start";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 100);
            button.Click += StartButton_Click;
            canvas.Children.Add(button);
        }

        public void AddTestButton(Panel canvas)
        {
            var button = new Button();
            button.Content = "Test";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 150);
            button.Click += TestButton_Click;
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

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            TestButtonAction();
        }

        public Action ConnectButtonAction { get; set; }
        public Action StartButtonAction { get; set; }
        public Action TestButtonAction { get; internal set; }
    }
}
