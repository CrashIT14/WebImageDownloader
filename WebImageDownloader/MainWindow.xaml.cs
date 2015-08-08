using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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
using HtmlAgilityPack;
using Microsoft.Win32;
using Se.Creotec.WPFToastMessage;
using WebImageDownloader.Model;

namespace WebImageDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        private readonly SolidColorBrush _errorTextBackground;
        private readonly SolidColorBrush _correctTextBackground;
        private Uri uriToDownload;
        private readonly BackgroundWorker _backgroundWorker;

        public MainWindow()
        {
            InitializeComponent();
            _errorTextBackground = 
            _correctTextBackground = Brushes.White;

            _backgroundWorker = new BackgroundWorker();
            SetupBackgroundWorker(_backgroundWorker);

            LoadSettings();
        }

        private void SetupBackgroundWorker(BackgroundWorker bw)
        {
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
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
                URLTextBox.Background = _correctTextBackground;
                uriToDownload = uri;
                StartDownloadButton.IsEnabled = true;
                UpdateOutputLabel();
            }
            else if (URLTextBox.Text.Trim() == "")
            {
                URLTextBox.Background = _correctTextBackground;
                uriToDownload = null;
                StartDownloadButton.IsEnabled = true;
                UpdateOutputLabel();
            }
            else
            {
                URLTextBox.Background = _errorTextBackground;
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
            else if (!_backgroundWorker.IsBusy)
            {
                _backgroundWorker.RunWorkerAsync(new WorkerArgument(SavedOutputTextBox.Text, URLTextBox.Text));
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            worker.ReportProgress(0);
            var argument = (WorkerArgument) e.Argument;

            var localPath = argument.LocalPath;
            var url = argument.Url;
            if (!Directory.Exists(localPath))
            {
                Directory.CreateDirectory(localPath);
            }

            var uri = new Uri(url);
            var client = new WebClient();
            var webSource = client.DownloadString(uri);
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(webSource);

            List<string> targets = ParserUtil.GetTargetsFromFilters(htmlDoc, new NodeFilter(NodeType.Link, "", "fileThumb")); // TODO: get filters

            int max = targets.Count;
            int current = 0;
            int completed = 0;

            WebClient webClient = new WebClient();
            foreach (string target in targets)
            {
                if (worker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                var targetUri = ParserUtil.GetUriFromTargetString(target, url);
                string filePath = localPath + @"\" + ParserUtil.GetFileNameFromUri(targetUri);
                if (!File.Exists(filePath))
                {
                    webClient.DownloadFile(targetUri, filePath);
                }
                current++; // TODO: Add error handling
                completed++;
                worker.ReportProgress((int)(((float)current / max) * 100));
            }
            worker.ReportProgress(100);
            e.Result = new WorkerResult(completed, max, localPath);
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                MainProgressBar.Value = 0;
            }

            else if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message, "An error occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            else
            {
                var result = (WorkerResult) e.Result;
                ToastMessage.Show("Downloaded " + result.Downloaded + " / " + result.Total + " to:\n" + result.LocalPath,
                    "Download complete", 5, ToastMessage.Position.BOTTOM_RIGHT, new DownloadCompleteAction(result.LocalPath));
            }
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            int percent = e.ProgressPercentage;
            if (percent == 0)
            {
                MainProgressBar.IsIndeterminate = true;
            }
            else
            {
                MainProgressBar.IsIndeterminate = false;
                MainProgressBar.Value = e.ProgressPercentage;
            }
        }

    }
}
