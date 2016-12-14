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
            PubnubConfigData data = new PubnubConfigData();
            data.ssl = true;
            data.resumeOnReconnect = true;
            data.publishKey = "pub-c-234c4038-44a7-4173-9fd7-e1f6151c56d7"; ///Your publish key goes here
            data.subscribeKey = "sub-c-53cc9228-a35d-11e6-a1b1-0619f8945a4f"; ///Your subscribe key goes here

            /* //Uncomment if hosting yourself and you have a static session id
            data.channelName = "my_device"; //If you are hosting, this is where your session id would go
            var frame = new Frame();
            frame.Navigate(typeof(PubnubOperation), data);
            Window.Current.Content = frame;*/
        }

        private void btnContinue_Click(object sender, RoutedEventArgs e) //Runs when button is clicked
        {
            PubnubConfigData data = new PubnubConfigData();
            data.channelName = txtChannelName.Text.Trim();
            var frame = new Frame();
            frame.Navigate(typeof(PubnubOperation), data);
            Window.Current.Content = frame;
        }
    }
}
