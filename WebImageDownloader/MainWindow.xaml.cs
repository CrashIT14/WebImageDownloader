using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using Microsoft.Win32;

namespace WebImageDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Model model;
        private SolidColorBrush errorTextBackground;
        private SolidColorBrush correctTextBackground;
        private Uri uriToDownload = null;

        public MainWindow()
        {
            InitializeComponent();
            model = Model.GetInstance();
            errorTextBackground = new SolidColorBrush(Color.FromArgb(0x80, 0xFF, 0, 0));
            correctTextBackground = Brushes.White;
            LoadSettings();
        }

        private void LoadSettings()
        {
            if (!Properties.Settings.Default.BaseDirectoryChanged)
            {
                var baseDir = Environment.CurrentDirectory + @"\output";
                if (!Directory.Exists(baseDir))
                {
                    Directory.CreateDirectory(baseDir);
                }
                Properties.Settings.Default.BaseDirectory = baseDir;
                Properties.Settings.Default.Save();
            }

            BaseOutputTextBox.Text = Properties.Settings.Default.BaseDirectory;
            SavedOutputTextBox.Text = BaseOutputTextBox.Text;
            Topmost = Properties.Settings.Default.TopMost;
            OpacitySlider.Value = Properties.Settings.Default.WindowOpacity;
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.BaseDirectory = BaseOutputTextBox.Text;
            Properties.Settings.Default.WindowOpacity = Opacity;
            Properties.Settings.Default.TopMost = Topmost;
            Properties.Settings.Default.Save();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void WindowMain_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void WindowMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog {ShowNewFolderButton = true};
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                BaseOutputTextBox.Text = dialog.SelectedPath;
            }
        }

        private void BaseOutputTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (BaseOutputTextBox.Text != Properties.Settings.Default.BaseDirectory &&
                Directory.Exists(BaseOutputTextBox.Text))
            {
                Properties.Settings.Default.BaseDirectoryChanged = true;
                Properties.Settings.Default.BaseDirectory = BaseOutputTextBox.Text;
                Properties.Settings.Default.Save();
            }
            else
            {
                BaseOutputTextBox.Text = Properties.Settings.Default.BaseDirectory;
            }
        }

        private void URLTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Uri uri;
            if (Uri.TryCreate(URLTextBox.Text, UriKind.Absolute, out uri))
            {
                URLTextBox.Background = correctTextBackground;
                uriToDownload = uri;
                StartDownloadButton.IsEnabled = true;
                UpdateOutputLabel();
            }
            else if (URLTextBox.Text.Trim() == "")
            {
                URLTextBox.Background = correctTextBackground;
                uriToDownload = null;
                StartDownloadButton.IsEnabled = true;
                UpdateOutputLabel();
            }
            else
            {
                URLTextBox.Background = errorTextBackground;
                uriToDownload = null;
                StartDownloadButton.IsEnabled = false;
            }
        }

        private void UpdateOutputLabel()
        {
            var baseDir = Properties.Settings.Default.BaseDirectory;
            if (URLTextBox.Text.Trim() != "" && uriToDownload.Segments.Length > 0)
            {
                var pageName = uriToDownload.Segments[uriToDownload.Segments.Length - 1];
                if (pageName == @"/")
                {
                    pageName = uriToDownload.Host;
                }
                SavedOutputTextBox.Text = baseDir + @"\" + pageName;
            }
            else
            {
                SavedOutputTextBox.Text = baseDir;
            }
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            var path = SavedOutputTextBox.Text;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Process.Start(path);
        }

        private void StartDownloadButton_Click(object sender, RoutedEventArgs e)
        {
            if (URLTextBox.Text.Trim() == "")
            {
                MessageBox.Show("You must specify an URL", "No URL", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                model.DownloadUrl(SavedOutputTextBox.Text);
            }
        }
    }
}
