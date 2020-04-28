using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TTT.Host
{
    public class MainMenuView : View
    {
        MainMenu _model;

        public MainMenuView(MainMenu mainMenu)
        {
            _model = mainMenu;
            Content = new Canvas();
            AddConnectButton();
            AddStartButton();
            AddTestButton();
        }

        public override void Show()
        {
            //Add Action Buttons to Canvas

        }

        public override void SizeChanged(Size newSize)
        {
            // do nothing;
        }

        public void AddConnectButton()
        {
            var button = new Button();
            button.Content = "Connect";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 20);
            button.Click += ConnectButton_Click;
            Content.Children.Add(button);
        }

        public void AddStartButton()
        {
            var button = new Button();
            button.Content = "Start";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 100);
            button.Click += StartButton_Click;
            Content.Children.Add(button);
        }

        public void AddTestButton()
        {
            var button = new Button();
            button.Content = "Test";
            Canvas.SetTop(button, 20);
            Canvas.SetLeft(button, 150);
            button.Click += TestButton_Click;
            Content.Children.Add(button);
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            _model.ConnectButtonAction();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            _model.StartButtonAction();
        }

        private void TestButton_Click(object sender, RoutedEventArgs e)
        {
            _model.TestButtonAction();
        }


    }
}
