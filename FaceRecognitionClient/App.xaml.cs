﻿using LogInClient;
using System.Windows;

namespace FaceRecognitionClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WindowService windowService = new WindowService();
        }
    }

}
