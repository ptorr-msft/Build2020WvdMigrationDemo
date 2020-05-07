using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Win32;

using Windows.Storage;

namespace WvdMigration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // MSIX apps can keep using the Registry, as long as it is per-user private app data.
        readonly string registryKeyName = @"Software\Fabrikam\WvdMigration\Settings";
        readonly string registryValueName = "FavoriteInstallTech";

        string dataFileDirectory;
        string logDirectory;
        string userFilePath;

        RegistryKey key;
        StreamWriter logfile;

        List<Tuple<string, RadioButton>> setupTechButtons;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                SetupButtonsList();
                SetupDirectories();
                OpenLogFile();
                LoadSettings();
                LoadData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error starting app");
                Application.Current.Shutdown();
            }

            registryKeyNameText.Text = $"Registry key: HKCU\\{registryKeyName}";
            logFileLocationText.Text = $"Log directory: {logDirectory}";
            dataFileLocationText.Text = $"Save directory: {dataFileDirectory}";
        }

        private void SetupButtonsList()
        {
            setupTechButtons = new List<Tuple<string, RadioButton>>()
            {
                new Tuple<string, RadioButton>("Setup", setupButton),
                new Tuple<string, RadioButton>("ClickOnce", clickOnceButton),
                new Tuple<string, RadioButton>("MSI", msiButton),
                new Tuple<string, RadioButton>("MSIX", msixButton)
            };
        }

        void SetupDirectories()
        {
            // BEFORE
            // ------
            // logDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

            // AFTER
            // -----
            if (App.IsPackaged)
            {
                logDirectory = Path.Combine(ApplicationData.Current.LocalFolder.Path);
            }
            else
            {
                logDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }

            // BEFORE
            // ------
            // dataFileDirectory = Environment.ExpandEnvironmentVariables(@"%ProgramData%\Fabrikam");

            // AFTER
            // -----
            dataFileDirectory = Environment.ExpandEnvironmentVariables(@"%AppData%\Fabrikam");


            if (!Directory.Exists(dataFileDirectory))
            {
                Directory.CreateDirectory(dataFileDirectory);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            CloseLogFile();
        }

        void OpenLogFile()
        {
            var fullLogPath = Path.Combine(logDirectory, "activity.log");
            logfile = new StreamWriter(fullLogPath);
            logfile.WriteLine($"App started at {DateTime.Now.ToUniversalTime()}");
        }

        void LoadSettings()
        {
            key = Registry.CurrentUser.CreateSubKey(registryKeyName, true);
            var favoriteTech = key.GetValue(registryValueName, "") as string;

            foreach ((var name, var button) in setupTechButtons)
            {
                if (favoriteTech == name)
                {
                    button.IsChecked = true;
                    break;
                }
            }
        }

        void LoadData()
        {
            userFilePath = Path.Combine(dataFileDirectory, "explain.txt");
            if (File.Exists(userFilePath))
            {
                using (var reader = File.OpenText(userFilePath))
                {
                    explainTextbox.Text = reader.ReadToEnd();
                }
            }
        }

        void DoSave(object sender, RoutedEventArgs e)
        {
            SaveSettings();
            SaveData();

            logfile.WriteLine($"Saved data at {DateTime.Now.ToUniversalTime()}");
        }

        void SaveSettings()
        {
            foreach ((var name, var button) in setupTechButtons)
            {
                if (button.IsChecked.GetValueOrDefault())
                {
                    key.SetValue(registryValueName, name);
                    break;
                }
            }
        }

        void SaveData()
        {
            File.Delete(userFilePath);
            using (var writer = new StreamWriter(File.OpenWrite(userFilePath)))
            {
                writer.Write(explainTextbox.Text);
            }
        }

        void CloseLogFile()
        {
            if (logfile != null)
            {
                logfile.WriteLine($"App closing at {DateTime.Now.ToUniversalTime()}");
                logfile.Dispose();
            }
        }

        private void CopyLogFileLocation(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clipboard.SetText(logDirectory);
        }

        private void CopyDataFileLocation(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clipboard.SetText(dataFileDirectory);
        }

        private void CopyRegistryKeyName(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Clipboard.SetText("HKCU\\" + registryKeyName);
        }
    }
}
