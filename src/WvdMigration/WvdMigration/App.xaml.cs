using System;
using System.Windows;

using Windows.ApplicationModel;

namespace WvdMigration
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        internal static bool IsPackaged
        {
            get
            {
                try
                {
                    var _ = Package.Current;
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}
