using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace TTT.Host
{
    public class ViewManager
    {
        MainWindow _mainWindow;
        Dictionary<object, FrameworkElement> _uiElements = new Dictionary<object, FrameworkElement>();

        public ViewManager(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void SetContent(FrameworkElement content)
        {
            _mainWindow.Content = content;
            _mainWindow.SizeChanged += (object sender, SizeChangedEventArgs eventArgs) =>
            {
                content.Height = eventArgs.NewSize.Height;
                content.Width = eventArgs.NewSize.Width;
            };
        }

        public void AddButtons(Panel content)
        {
            _mainWindow.AddConnectButton(content);
            _mainWindow.AddStartButton(content);
            _mainWindow.AddTestButton(content);
        }

        public void RefreshView()
        {

        }
    }
}
