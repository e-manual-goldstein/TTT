﻿using System;
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
            var socketHub = new SocketHub();
            while (true)
            {
                try
                {
                    var guid = Guid.NewGuid();
                    socketHub.RequestSocketConnection(guid);
                    socketHub.OpenConnection(guid);
                }
                catch
                {

                }
            }
            
        }
    }
}