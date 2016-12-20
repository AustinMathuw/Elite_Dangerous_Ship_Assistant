using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PubnubWindowsStore
{
    /// <summary>
    /// Page that is used .
    /// </summary>
    public sealed partial class PubnubDemoStart : Page
    {
        public PubnubDemoStart()
        {
            this.InitializeComponent();
            /* //Uncomment if hosting yourself and you have a static session id
            PubnubConfigData data = new PubnubConfigData();
            data.channelName = "my_device"; //If you are hosting, this is where your session id would go
            data.ssl = true;
            data.resumeOnReconnect = true;
            data.publishKey = ""; ///Your publish key goes here
            data.subscribeKey = ""; ///Your subscribe key goes here
            var frame = new Frame();
            frame.Navigate(typeof(PubnubOperation), data);
            Window.Current.Content = frame;*/
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) //Runs when button is clicked
        {
            PubnubConfigData data = new PubnubConfigData();
            data.ssl = true;
            data.resumeOnReconnect = true;
            data.publishKey = "pub-c-4ca344c3-e90b-4d78-b9a6-30319a781440"; ///Your publish key goes here
            data.subscribeKey = "sub-c-f5e683d4-c676-11e6-b82b-0619f8945a4f"; ///Your subscribe key goes here
            data.channelName = txtChannelName.Text.Trim();
            var frame = new Frame();
            frame.Navigate(typeof(PubnubOperation), data);
            Window.Current.Content = frame;
        }
    }
}
