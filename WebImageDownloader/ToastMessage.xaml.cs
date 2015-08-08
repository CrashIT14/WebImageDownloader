using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace Se.Creotec.WPFToastMessage
{
    public partial class ToastMessage : Window
    {
        private const double TOAST_MARGIN = 20.0;

        private const int DEFAULT_TIMER = 3;
        private const String DEFAULT_TITLE = "Notification";
        private const ToastMessage.Position DEFAULT_POS = ToastMessage.Position.TOP_RIGHT;

        private DispatcherTimer timer = new DispatcherTimer();

        private string path;
        private int completed;
        private int max;

        private bool downloadToast = false;

        public enum Position
        {
            TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT
        }

        #region Constructors
        public ToastMessage() { }
        
        public ToastMessage(String message)
            : this(message, DEFAULT_TITLE, DEFAULT_TIMER, DEFAULT_POS) {}
        public ToastMessage(String message, String title)
            : this(message, title, DEFAULT_TIMER, DEFAULT_POS) {}

        public ToastMessage(String message, String title, int delay) 
            : this(message, title, delay, DEFAULT_POS) {}

        public ToastMessage(String message, String title, int delay, ToastMessage.Position position)
        {
            InitializeComponent();

            textToastMessage.Text = message;
            labelToastTitle.Content = title;
            setPosition(position);

            downloadToast = false;

            SetupTimer(delay);
        }

        public ToastMessage(string path, int completed, int max)
        {
            InitializeComponent();
            downloadToast = true;

            textToastMessage.Text = "Downloaded " + completed + "/" + max + " files to:\n" + path;
            labelToastTitle.Content = "Download complete";
            setPosition(Position.BOTTOM_RIGHT);

            this.path = path;
            this.completed = completed;
            this.max = max;

            SetupTimer(5);
        }

        #endregion

        private void SetupTimer(int delay)
        {
            timer.Tick += closeToast;
            timer.Interval = new TimeSpan(0, 0, delay);
            timer.Start();
        }

        /// <summary>
        /// Display a toast message for a defined amount of time
        /// </summary>
        /// <param name="message">The message to be shown</param>
        /// <param name="title">The title of the toast</param>
        /// <param name="delay">Time in seconds that the toast will be shown</param>
        /// <param name="position">The position of the toast on the screen</param>
        public static void Show(String message, String title, int delay, Position position)
        {
            new ToastMessage(message, title, delay, position).Show();
        }

        public static void ShowDownloadComplete(string path, int completed, int max)
        {
            new ToastMessage(path, completed, max).Show();
        }

        /// <summary>
        /// Sets the position of the toast based on a predefined position.
        /// </summary>
        /// <param name="position">The position where the toast will show</param>
        private void setPosition(ToastMessage.Position position)
        {
            // Using WorkArea to take things as the TaskBar in mind
            double screenWidth = System.Windows.SystemParameters.WorkArea.Width;
            double screenHeight = System.Windows.SystemParameters.WorkArea.Height;

            switch (position) {
                case ToastMessage.Position.TOP_LEFT:
                    this.Left = TOAST_MARGIN;
                    this.Top = TOAST_MARGIN;
                    break;
                case ToastMessage.Position.TOP_RIGHT:
                    this.Left = screenWidth - Width - TOAST_MARGIN;
                    this.Top = TOAST_MARGIN;
                    break;
                case ToastMessage.Position.BOTTOM_LEFT:
                    this.Left = TOAST_MARGIN;
                    this.Top = screenHeight - Height - TOAST_MARGIN;
                    break;
                case ToastMessage.Position.BOTTOM_RIGHT:
                    this.Left = screenWidth - Width - TOAST_MARGIN;
                    this.Top = screenHeight - Height - TOAST_MARGIN;
                    break;
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            // Pauses the timer when the user hover over the toast
            timer.Stop();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            // Resumes the timer when the mouse leaves
            timer.Start();
        }        

        private void closeToast(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonToastClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (downloadToast)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                Process.Start(path);
            }
        }
    }
}