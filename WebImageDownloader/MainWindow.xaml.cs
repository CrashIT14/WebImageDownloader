using System;
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

namespace WebImageDownloader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private Model model;

        public MainWindow()
        {
            InitializeComponent();
            model = Model.GetInstance();
            loadSettings();
        }

        private void loadSettings()
        {
            this.Topmost = Properties.Settings.Default.TopMost;
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // TODO: Fix opacity support
        }

        private void AlwaysOnTopCheck_Changed(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.TopMost = (bool) AlwaysOnTopCheck.IsChecked;
            Properties.Settings.Default.Save();
        }
    }
}
