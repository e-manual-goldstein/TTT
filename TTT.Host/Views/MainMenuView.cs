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
        private List<Button> _buttons = new List<Button>();

        public MainMenuView(MainMenu mainMenu)
        {
            _model = mainMenu;
            Content = new Canvas();
        }

        public override void Show()
        {
            //Add Action Buttons to Canvas
            AddButtons();
        }

        private void AddButtons()
        {
            var actions = _model.MenuActions();
            double baseY = (Content.Height - (actions.Count * 20)) / 2;
            foreach (var kvp in _model.MenuActions())
            {
                var button = new Button();
                button.Content = kvp.Key;
                Canvas.SetTop(button, baseY);
                Canvas.SetLeft(button, 20);
                button.Click += (object sender, RoutedEventArgs eventArgs) => kvp.Value();
                Content.Children.Add(button);
                _buttons.Add(button);
                baseY += 20;
            }
        }

        public override void SizeChanged(Size newSize)
        {
            double baseY = (newSize.Height - (_buttons.Count * 20)) / 2;
            foreach (var button in _buttons)
            {
                Canvas.SetTop(button, baseY);
                baseY += 20;
            }
        }

    }
}
