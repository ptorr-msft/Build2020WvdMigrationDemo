using System.Diagnostics;
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

        internal static string AppVersionInfo
        {
            get
            {
                if (App.IsPackaged)
                {
                    return $"MSIX package {Package.Current.Id.FullName}";
                }
                else if (Process.GetCurrentProcess().MainModule.FileName.Contains("wvdm..tion"))
                {
                    return "ClickOnce deployed app";

                }
                else
                {
                    return "Development mode app";
                }

            }
        }
    }
}
