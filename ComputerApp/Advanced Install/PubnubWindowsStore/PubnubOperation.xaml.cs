using System;
using System.Threading;
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
using Windows.Storage;
// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace PubnubWindowsStore
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PubnubOperation : Page
    {

        string subscribeMessageResult = "";
        string publishSend = "";
        string publishChannel = "";
        string subscribeChannel = "";
        PubnubConfigData data = null;
        static Pubnub pubnub = null;

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
                pubnub = new Pubnub(data.publishKey, data.subscribeKey, data.secretKey, data.cipherKey, data.ssl); //Initialize PubNub
                pubnub.EnableResumeOnReconnect = data.resumeOnReconnect; 

                

            }
            mainLoop();
        }

        /// <summary>
        /// Callback method captures the response in JSON string format for all operations
        /// </summary>
        /// <param name="result"></param>
        



        /// <summary>
        /// Callback method for error messages
        /// </summary>
        /// <param name="result"></param>
        void PubnubDisplayErrorMessage(PubnubClientError result) //Error Callback
        {
            

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

        void PubnubSubscribeCallbackResult(string result)
        {

            updateSubscribeResult(result);
        } //Handels Command subscribe

        public void updateSubscribeResult(string result)
        {
            if(subscribeMessageResult != result)
            {
                subscribeMessageResult = result;
                DisplaySubscribeMessageInTextBox(result);
            }
        } //Handels Command subscribe

        void PubnubPublishCallbackResult(string result)
        {

        }

        private void PubnubDisconnectCallbackResult(string result)
        {
            
        }

        void DisplaySubscribeConnectStatusMessage(string result)
        {
            
        }
        void DisplayErrorMessage(PubnubClientError pubnubError)
        {
            
        }

        private async void mainLoop()
        {
            try
            {
                await KnownFolders.DocumentsLibrary.GetFileAsync("Elite Dangerous Ship Assistant\\commands.json");
            }
            catch
            {
                await KnownFolders.DocumentsLibrary.CreateFileAsync("Elite Dangerous Ship Assistant\\commands.json");
                
            }
            var shipCommand = await KnownFolders.DocumentsLibrary.GetFileAsync("Elite Dangerous Ship Assistant\\commands.json");
            await FileIO.WriteTextAsync(shipCommand,"");
            long lastUpdate = 0;
            publishChannel = data.channelName + "A";
            subscribeChannel = data.channelName + "B";

            pubnub.Subscribe<string>(
                subscribeChannel,
                PubnubSubscribeCallbackResult,
                DisplaySubscribeConnectStatusMessage,
                DisplayErrorMessage
            );

            while (true)
            {
                long milliseconds = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
                if (milliseconds - lastUpdate > 5)
                {
                    lastUpdate = milliseconds;
                    

                    //Publish Info
                    try
                    {
                        StorageFile shipInfo = await KnownFolders.DocumentsLibrary.GetFileAsync("Elite Dangerous Ship Assistant\\shipData.json");

                        string shipInfoText = await FileIO.ReadTextAsync(shipInfo);
                        string publishMsg = shipInfoText.Replace("\\", "");
                        bool storeInHistory = true;
                        

                        if (publishSend != publishMsg)
                        {
                            pubnub.Publish<string>(publishChannel, publishMsg, storeInHistory, PubnubPublishCallbackResult, PubnubDisplayErrorMessage);
                            publishSend = publishMsg;
                            string shipInfoTextBox = "";
                            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                            {
                                if (publishMsg.Length > 200)
                                {
                                    shipInfoTextBox = string.Concat(publishMsg.Substring(0, 200), "..(truncated)");
                                }

                                if (publishResult.Text.Length > 200)
                                {
                                    publishResult.Text = string.Concat("(Truncated)..\n", publishResult.Text.Remove(0, 200));
                                }

                                publishResult.Text += shipInfoTextBox + "\n";
                                publishResult.Select(publishResult.Text.Length - 1, 1);
                            });
                        }
                    }
                    catch
                    {

                    }
                }
            };
        } //Main function loop

        private async void DisplaySubscribeMessageInTextBox(string msg)
        {
            var shipCommand = await KnownFolders.DocumentsLibrary.GetFileAsync("Elite Dangerous Ship Assistant\\commands.json");

            var folder = await KnownFolders.DocumentsLibrary.GetFolderAsync("Elite Dangerous Ship Assistant");

            string path = shipCommand.Path;

            try
            {
                File.AppendAllText(path, Environment.NewLine + msg);
            }
            catch
            {

            }

            try
            {
                await shipCommand.CopyAsync(folder, "commandsTo.json");
            }
            catch
            {
                try
                {
                    var shipTo = await folder.GetFileAsync("commandsTo.json");
                    await shipTo.DeleteAsync();
                    await shipCommand.CopyAsync(folder, "commandsTo.json");
                }
                catch
                {

                }
            }
            

            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    if (msg.Length > 200)
                    {
                        msg = string.Concat(msg.Substring(0, 200), "..(truncated)");
                    }

                    if (subscribeResult.Text.Length > 200)
                    {
                        subscribeResult.Text = string.Concat("(Truncated)..\n", subscribeResult.Text.Remove(0, 200));
                    }

                    subscribeResult.Text += msg + "\n";
                    subscribeResult.Select(subscribeResult.Text.Length - 1, 1);
                });

        } //Gets commands

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
                publishResult.Text = "";
                subscribeResult.Text = "";
            }
        }

        private void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var frame = new Frame();
            frame.Navigate(typeof(PubnubDemoStart), data);
            Window.Current.Content = frame;
        } //Handels back button
    }
}
