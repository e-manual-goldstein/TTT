using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TTT.Common;

namespace TTT.Host
{
    public class ViewManager
    {
        MainWindow _mainWindow;
        Dictionary<IUpdatingElement, FrameworkElement> _uiElements = new Dictionary<IUpdatingElement, FrameworkElement>();
        View _currentView;

        public ViewManager(MainWindow mainWindow)
        {
            _mainWindow = mainWindow;
        }

        public void SetContent(View view)
        {
            _currentView = view;
            _mainWindow.Content = view.Content;
            _mainWindow.SizeChanged += (object sender, SizeChangedEventArgs eventArgs) =>
            {
                view.SizeChanged(eventArgs.NewSize);
            };
        }

        public void AddBoundElement(IUpdatingElement updatingElement, FrameworkElement viewObject)
        {
            //_content.Children.Add(viewObject);
            _uiElements[updatingElement] = viewObject;
        }

        public void AddButtons(Panel content)
        {
            _mainWindow.AddConnectButton(content);
            _mainWindow.AddStartButton(content);
            _mainWindow.AddTestButton(content);
        }

        public void Show()
        {
            _currentView.Show();
        }

        public void Update()
        {
            _currentView.Refresh();
        }
    }
}
