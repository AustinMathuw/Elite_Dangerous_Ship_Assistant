using System;
using System.Security;
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
using PubNubMessaging.Core;
using Windows.UI.Core;
using Windows.UI;
using Windows.UI.Popups;
using System.Threading;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PubnubWindowsStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PubnubOperation : Page
    {
        private static AddedContentReader _continuousFileReader = null;
        string channel = "";
        string channelGroup = "";
        PubnubConfigData data = null;
        static Pubnub pubnub = null;

        Popup publishPopup = null;
        Popup hereNowPopup = null;
        Popup whereNowPopup = null;
        Popup globalHereNowPopup = null;
        Popup userStatePopup = null;
        Popup changeUUIDPopup = null;


        public PubnubOperation()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            data = e.Parameter as PubnubConfigData;

            if (data != null)
            {
                pubnub = new Pubnub(data.publishKey, data.subscribeKey, data.secretKey, data.cipherKey, data.ssl);
                /*pubnub.Origin = data.origin;
                pubnub.SessionUUID = data.sessionUUID;
                pubnub.SubscribeTimeout = data.subscribeTimeout;
                pubnub.NonSubscribeTimeout = data.nonSubscribeTimeout;
                pubnub.NetworkCheckMaxRetries = data.maxRetries;
                pubnub.NetworkCheckRetryInterval = data.retryInterval;
                pubnub.LocalClientHeartbeatInterval = data.localClientHeartbeatInterval;*/
                pubnub.EnableResumeOnReconnect = data.resumeOnReconnect;
                /*pubnub.AuthenticationKey = data.authKey;
                pubnub.PresenceHeartbeat = data.presenceHeartbeat;
                pubnub.PresenceHeartbeatInterval = data.presenceHeartbeatInterval;*/

                

            }

        }



        private void btnTime_Click(object sender, RoutedEventArgs e)
        {
            DisplayMessageInTextBox("Running Time:");
            pubnub.Time<string>(PubnubCallbackResult, PubnubDisplayErrorMessage);
        }

        /// <summary>
        /// Callback method captures the response in JSON string format for all operations
        /// </summary>
        /// <param name="result"></param>
        void PubnubCallbackResult(string result)
        {
            DisplayMessageInTextBox(result);
            //Console.WriteLine("REGULAR CALLBACK:");
            //Console.WriteLine(result);
            //Console.WriteLine();
        }

        /// <summary>
        /// Callback method for error messages
        /// </summary>
        /// <param name="result"></param>
        void PubnubDisplayErrorMessage(PubnubClientError result)
        {
            DisplayMessageInTextBox(result.Description);

            switch (result.StatusCode)
            {
                case 103:
                    //Warning: Verify origin host name and internet connectivity
                    break;
                case 104:
                    //Critical: Verify your cipher key
                    break;
                case 106:
                    //Warning: Check network/internet connection
                    break;
                case 108:
                    //Warning: Check network/internet connection
                    break;
                case 109:
                    //Warning: No network/internet connection. Please check network/internet connection
                    break;
                case 110:
                    //Informational: Network/internet connection is back. Active subscriber/presence channels will be restored.
                    break;
                case 111:
                    //Informational: Duplicate channel subscription is not allowed. Internally Pubnub API removes the duplicates before processing.
                    break;
                case 112:
                    //Informational: Channel Already Subscribed/Presence Subscribed. Duplicate channel subscription not allowed
                    break;
                case 113:
                    //Informational: Channel Already Presence-Subscribed. Duplicate channel presence-subscription not allowed
                    break;
                case 114:
                    //Warning: Please verify your cipher key
                    break;
                case 115:
                    //Warning: Protocol Error. Please contact PubNub with error details.
                    break;
                case 116:
                    //Warning: ServerProtocolViolation. Please contact PubNub with error details.
                    break;
                case 117:
                    //Informational: Input contains invalid channel name
                    break;
                case 118:
                    //Informational: Channel not subscribed yet
                    break;
                case 119:
                    //Informational: Channel not subscribed for presence yet
                    break;
                case 120:
                    //Informational: Incomplete unsubscribe. Try again for unsubscribe.
                    break;
                case 121:
                    //Informational: Incomplete presence-unsubscribe. Try again for presence-unsubscribe.
                    break;
                case 122:
                    //Informational: Network/Internet connection not available. C# client retrying again to verify connection. No action is needed from your side.
                    break;
                case 123:
                    //Informational: During non-availability of network/internet, max retries for connection were attempted. So unsubscribed the channel.
                    break;
                case 124:
                    //Informational: During non-availability of network/internet, max retries for connection were attempted. So presence-unsubscribed the channel.
                    break;
                case 125:
                    //Informational: Publish operation timeout occured.
                    break;
                case 126:
                    //Informational: HereNow operation timeout occured
                    break;
                case 127:
                    //Informational: Detailed History operation timeout occured
                    break;
                case 128:
                    //Informational: Time operation timeout occured
                    break;
                case 4000:
                    //Warning: Message too large. Your message was not sent. Try to send this again smaller sized
                    break;
                case 4001:
                    //Warning: Bad Request. Please check the entered inputs or web request URL
                    break;
                case 4002:
                    //Warning: Invalid Key. Please verify the publish key
                    break;
                case 4010:
                    //Critical: Please provide correct subscribe key. This corresponds to a 401 on the server due to a bad sub key
                    break;
                case 4020:
                    // PAM is not enabled. Please contact PubNub support
                    break;
                case 4030:
                    //Warning: Not authorized. Check the permimissions on the channel. Also verify authentication key, to check access.
                    break;
                case 4031:
                    //Warning: Incorrect public key or secret key.
                    break;
                case 4140:
                    //Warning: Length of the URL is too long. Reduce the length by reducing subscription/presence channels or grant/revoke/audit channels/auth key list
                    break;
                case 5000:
                    //Critical: Internal Server Error. Unexpected error occured at PubNub Server. Please try again. If same problem persists, please contact PubNub support
                    break;
                case 5020:
                    //Critical: Bad Gateway. Unexpected error occured at PubNub Server. Please try again. If same problem persists, please contact PubNub support
                    break;
                case 5040:
                    //Critical: Gateway Timeout. No response from server due to PubNub server timeout. Please try again. If same problem persists, please contact PubNub support
                    break;
                case 0:
                    //Undocumented error. Please contact PubNub support with full error object details for further investigation
                    break;
                default:
                    break;
            }
        }

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            var frame = new Frame();
            frame.Navigate(typeof(PubnubDemoStart));
            Window.Current.Content = frame;
        }

        private void Subscribe()
        {
            channel = data.channelName + "B";
            DisplayMessageInTextBox("Running Subscribe:");
            pubnub.Subscribe<string>(channel, PubnubCallbackResult, PubnubConnectCallbackResult, PubnubDisplayErrorMessage);
        }

        void PubnubConnectCallbackResult(string result)
        {
            DisplayMessageInTextBox("Connect Callback:");
            DisplayMessageInTextBox(result);
        }

        private void PubnubDisconnectCallbackResult(string result)
        {
            DisplayMessageInTextBox("Disconnect Callback:");
            DisplayMessageInTextBox(result);
        }


        private void btnPublish_Click()
        {
            channel = data.channelName + "A";
            this.IsEnabled = false;
            Border border = new Border();
            border.BorderBrush = new SolidColorBrush(Colors.Black);
            border.BorderThickness = new Thickness(5.0);

            StackPanel publishStackPanel = new StackPanel();
            publishStackPanel.Background = new SolidColorBrush(Colors.Blue);
            publishStackPanel.Width = 320;
            publishStackPanel.Height = 550;

            publishPopup = new Popup();
            publishPopup.Height = 550;
            publishPopup.Width = 320;
            publishPopup.VerticalOffset = 100;
            publishPopup.HorizontalOffset = 10;


            PublishMessageUserControl control = new PublishMessageUserControl();
            publishStackPanel.Children.Add(control);
            border.Child = publishStackPanel;

            publishPopup.Child = border;
            publishPopup.IsOpen = true;

            publishPopup.Closed += (senderPopup, argsPopup) =>
            {
                if (control.IsOKButtonEntered)
                {
                    RadioButton radNormalPublish = control.FindName("radNormalPublish") as RadioButton;
                    if (radNormalPublish != null && radNormalPublish.IsChecked.Value)
                    {
                        TextBox txtPublish = control.FindName("txtPublish") as TextBox;
                        string publishMsg = (txtPublish != null) ? txtPublish.Text : "";

                        CheckBox chkStoreInHistory = control.FindName("chkStoreInHistory") as CheckBox;
                        bool storeInHistory = (chkStoreInHistory != null) ? chkStoreInHistory.IsChecked.Value : true;

                        if (publishMsg != "")
                        {
                            DisplayMessageInTextBox("Running Publish:");

                            double doubleData;
                            int intData;
                            if (int.TryParse(publishMsg, out intData)) //capture numeric data
                            {
                                pubnub.Publish<string>(channel, intData, storeInHistory, PubnubCallbackResult, PubnubDisplayErrorMessage);
                            }
                            else if (double.TryParse(publishMsg, out doubleData)) //capture numeric data
                            {
                                pubnub.Publish<string>(channel, doubleData, storeInHistory, PubnubCallbackResult, PubnubDisplayErrorMessage);
                            }
                            else
                            {
                                pubnub.Publish<string>(channel, publishMsg, storeInHistory, PubnubCallbackResult, PubnubDisplayErrorMessage);
                            }
                        }
                    }

                    RadioButton radToastPublish = control.FindName("radToastPublish") as RadioButton;
                    if (radToastPublish != null && radToastPublish.IsChecked.Value)
                    {
                        MpnsToastNotification toast = new MpnsToastNotification();
                        toast.text1 = "hardcode message";
                        Dictionary<string, object> dicToast = new Dictionary<string, object>();
                        dicToast.Add("pn_mpns", toast);

                        pubnub.EnableDebugForPushPublish = true;
                        pubnub.Publish<string>(channel, dicToast, PubnubCallbackResult, PubnubDisplayErrorMessage);

                    }

                    RadioButton radFlipTilePublish = control.FindName("radFlipTilePublish") as RadioButton;
                    if (radFlipTilePublish != null && radFlipTilePublish.IsChecked.Value)
                    {
                        pubnub.PushRemoteImageDomainUri.Add(new Uri("http://cdn.flaticon.com"));

                        MpnsFlipTileNotification tile = new MpnsFlipTileNotification();
                        tile.title = "front title";
                        tile.count = 6;
                        tile.back_title = "back title";
                        tile.back_content = "back message";
                        tile.back_background_image = "Assets/Tiles/pubnub3.png";
                        tile.background_image = "http://cdn.flaticon.com/png/256/37985.png";
                        Dictionary<string, object> dicTile = new Dictionary<string, object>();
                        dicTile.Add("pn_mpns", tile);

                        pubnub.EnableDebugForPushPublish = true;
                        pubnub.Publish<string>(channel, dicTile, PubnubCallbackResult, PubnubDisplayErrorMessage);
                    }

                    RadioButton radCycleTilePublish = control.FindName("radCycleTilePublish") as RadioButton;
                    if (radCycleTilePublish != null && radCycleTilePublish.IsChecked.Value)
                    {
                        MpnsCycleTileNotification tile = new MpnsCycleTileNotification();
                        tile.title = "front title";
                        tile.count = 2;
                        tile.images = new string[] { "Assets/Tiles/pubnub1.png", "Assets/Tiles/pubnub2.png", "Assets/Tiles/pubnub3.png", "Assets/Tiles/pubnub4.png" };

                        Dictionary<string, object> dicTile = new Dictionary<string, object>();
                        dicTile.Add("pn_mpns", tile);

                        pubnub.EnableDebugForPushPublish = true;
                        pubnub.Publish<string>(channel, dicTile, PubnubCallbackResult, PubnubDisplayErrorMessage);
                    }

                    RadioButton radIconicTilePublish = control.FindName("radIconicTilePublish") as RadioButton;
                    if (radIconicTilePublish != null && radIconicTilePublish.IsChecked.Value)
                    {
                        MpnsIconicTileNotification tile = new MpnsIconicTileNotification();
                        tile.title = "front title";
                        tile.count = 2;
                        tile.wide_content_1 = "my wide content";

                        Dictionary<string, object> dicTile = new Dictionary<string, object>();
                        dicTile.Add("pn_mpns", tile);

                        pubnub.EnableDebugForPushPublish = true;
                        pubnub.Publish<string>(channel, dicTile, PubnubCallbackResult, PubnubDisplayErrorMessage);
                    }
                }
                publishPopup = null;
                this.IsEnabled = true;
            };

        }

        private async void DisplayMessageInTextBox(string msg)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (msg.Length > 200)
                {
                    msg = string.Concat(msg.Substring(0, 200), "..(truncated)");
                }

                if (txtResult.Text.Length > 200)
                {
                    txtResult.Text = string.Concat("(Truncated)..\n", txtResult.Text.Remove(0, 200));
                }

                txtResult.Text += msg + "\n";
                txtResult.Select(txtResult.Text.Length - 1, 1);
            });
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            PubnubCleanup();
        }

        void PubnubCleanup()
        {
            if (pubnub != null)
            {
                pubnub.EndPendingRequests();
                pubnub = null;
            }
        }

        private async void txtResult_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            MessageDialog messageDialog = new MessageDialog("Confirm Delete");

            messageDialog.Commands.Add(new UICommand("Delete", new UICommandInvokedHandler(this.CommandInvokedHandler)));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.CommandInvokedHandler)));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Set the command to be invoked when escape is pressed
            messageDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await messageDialog.ShowAsync();

        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Delete")
            {
                txtResult.Text = "";
            }
        }
        public class AddedContentReader
        {

            private readonly FileStream _fileStream;
            private readonly StreamReader _reader;

            //Start position is from where to start reading first time. consequent read are managed by the Stream reader
            public AddedContentReader(string fileName, long startPosition = 0)
            {
                //Open the file as FileStream
                _fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                _reader = new StreamReader(_fileStream);
                //Set the starting position
                _fileStream.Position = startPosition;
            }


            //Get the current offset. You can save this when the application exits and on next reload
            //set startPosition to value returned by this method to start reading from that location
            public long CurrentOffset
            {
                get { return _fileStream.Position; }
            }

            public bool NewDataReady()
            {
                return (_fileStream.Length >= _fileStream.Position);
            }

            //Returns the lines added after this function was last called
            public string GetAddedLine()
            {
                return _reader.ReadLine();
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }
    }
}
