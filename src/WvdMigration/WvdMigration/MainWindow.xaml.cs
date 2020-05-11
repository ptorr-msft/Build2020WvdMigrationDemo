using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using Windows.ApplicationModel;
using Windows.Storage;

namespace WvdMigration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            //installLocationText.Text = Package.Current.InstalledLocation.Path;
        }
    }
}
