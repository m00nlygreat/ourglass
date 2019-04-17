﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ourglass
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow wnd = new MainWindow();
            wnd.Show();
            if (e.Args.Length == 1)
            {
                try
                {
                    wnd.cmbTime.Text = e.Args[0];
                    wnd.btnStart.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                }
                catch { }
            }
        }
    }
}
