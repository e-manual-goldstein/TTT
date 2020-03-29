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
        public MainWindow()
        {
            InitializeComponent();
            
            GameGrid = new GameGrid(() => (float)Width,() => (float)Height);
            GameGrid.DrawCells();
            Content = GameGrid.Canvas;
            AddTestButton(GameGrid.Canvas);
        }

        private void AddTestButton(Canvas canvas)
        {
            var button = new Button();
            button.Content = "Test";
            button.Click += Button_Click;
            canvas.Children.Add(button);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ButtonAction();
        }

        public static GameGrid GameGrid { get; set; }

        public static Action ButtonAction { get; set; }
    }
}
