using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TurnoffTimer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer timer;
        private int minutes = 0;
        private int seconds = 0;
        public MainPage()
        {
            ApplicationView.PreferredLaunchViewSize = new Size(569, 320);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            this.InitializeComponent();
            timer = new DispatcherTimer();
            
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Hand, 1);
        }

        private void Button_PointerExited(object sender, PointerRoutedEventArgs e)
        {

            Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
        }

        private async void Button_Start_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)!Radio_reboot.IsChecked && (bool)!Radio_shutdown.IsChecked)
            {
                var messageDialog = new MessageDialog("Please select an option!");
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
                return;
            }
            var reg = new Regex("^[0-9]+$");
            if(!reg.IsMatch(TextBox_minutes.Text) || !reg.IsMatch(Textbox_Seconds.Text))
            {
                var messageDialog = new MessageDialog("Minutes / Seconds input error!");
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
                return;
            }
            minutes =  int.Parse(TextBox_minutes.Text);
            seconds = int.Parse(Textbox_Seconds.Text);
            label_minutes.Text = minutes.ToString()+":";
            label_seconds.Text = seconds.ToString();
            label_minutes.Visibility = Visibility.Visible;
            label_seconds.Visibility = Visibility.Visible;
            Button_Abort.IsEnabled = true;
            TextBox_minutes.IsEnabled = false;
            Textbox_Seconds.IsEnabled = false;
            Radio_reboot.IsEnabled = false;
            Radio_shutdown.IsEnabled = false;
            timer.Tick += dispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private  void dispatcherTimer_Tick(object sender,object e)
        {
            
            if(minutes==0 && seconds==0)
            {
                timer.Stop();

                //var messageDialog = new MessageDialog("Goodbye!");
                //messageDialog.CancelCommandIndex = 1;

                //// Show the message dialog
                //await messageDialog.ShowAsync();
                
                //Button_Abort.IsEnabled = false;
                //TextBox_minutes.IsEnabled = true;
                //Textbox_Seconds.IsEnabled = true;
                //Radio_reboot.IsEnabled = true;
                //Radio_shutdown.IsEnabled = true;
                //label_minutes.Visibility = Visibility.Collapsed;
                //label_seconds.Visibility = Visibility.Collapsed;
                if((bool)Radio_shutdown.IsChecked)
                {
                    this.ProcessStart(1);
                }
                else
                {
                    this.ProcessStart(0);
                }
            }
            else
            {
                if(seconds==0)
                {
                    minutes--;
                    seconds = 59;
                }
                else
                {
                    seconds--;
                }
                label_minutes.Text = minutes.ToString() + ":";
                label_seconds.Text = seconds.ToString();
            }
        }

        private void Button_Abort_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            Button_Abort.IsEnabled = false;
            TextBox_minutes.IsEnabled = true;
            Textbox_Seconds.IsEnabled = true;
            Radio_reboot.IsEnabled = true;
            Radio_shutdown.IsEnabled = true;
            label_minutes.Visibility = Visibility.Collapsed;
            label_seconds.Visibility = Visibility.Collapsed;

        }

        private void ProcessStart(int option = 0)
        {
            string filename = string.Empty;
            string arguments = string.Empty;

            if(option==1)
            {
                filename = "shutdown.exe";
                arguments = "-s";
            }
            else
            {
                filename = "shutdown.exe";
                arguments = "-r";
            }
            ProcessStartInfo startinfo = new ProcessStartInfo(filename, arguments);
            Process.Start(startinfo);
        }
    }
}
