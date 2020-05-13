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

            appVersionText.Text = $"App type: {App.AppVersionInfo}";
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
            if (App.IsPackaged)
            {
                logDirectory = Path.Combine(ApplicationData.Current.LocalFolder.Path);
            }
            else
            {
                logDirectory = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            }

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
            WriteLog("App launched");
        }

        void WriteLog(string message)
        {
            if (logfile != null)
            {
                logfile.WriteLine($"{message} at {DateTime.Now.ToUniversalTime()}");
            }
        }

        void LoadSettings()
        {
            key = Registry.CurrentUser.CreateSubKey(registryKeyName, true);
            var favoriteTech = key.GetValue(registryValueName, "") as string;

            // Ignore registry value; we know MSIX is the best.
            return;

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

            // Ignore what was written before; we know why MSIX is the best.
            return;

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

            WriteLog("Registry and file saved");
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
            SaveData(userFilePath);
        }

        void SaveData(string filename)
        {
            File.Delete(filename);
            using (var writer = new StreamWriter(File.OpenWrite(filename)))
            {
                writer.Write(explainTextbox.Text);
            }
        }

        void CloseLogFile()
        {
            if (logfile != null)
            {
                WriteLog("App closed");
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

        private void DoSaveCopy(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                Filter = "Text files|*.txt",
                FileName = "explain.txt",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            var result = dialog.ShowDialog();
            if (result.GetValueOrDefault())
            {
                WriteLog($"File exported to {dialog.FileName}");
                SaveData(dialog.FileName);
            }
        }
    }
}
