using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Signus0x539.Heras
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private HeosSSDP discovery;
        private HeosAPI api;
        private string mainDeviceIp = string.Empty;

        public MainPage()
        {
            this.InitializeComponent();
            discovery = new HeosSSDP();
            api = new HeosAPI();

            try
            {
                Task.Run(async () => await Init());

            }
            catch (Exception exeption)
            {
                // OH NOES!
            }
        }

        private async Task Init()
        {
            await discovery.LocateHeosDevices();
            mainDeviceIp = discovery.Devices.FirstOrDefault()?.Location?.DnsSafeHost;

            if (string.IsNullOrEmpty(mainDeviceIp)) { return; }

            var commandTask = api.SendCommand(mainDeviceIp,
                HeosAction.Register_For_Change_Events,
                new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("enable", "off")
                    });

            await Task.WhenAll(commandTask);
            await GetCurrentSong("-768839342");
        }

        private async void Get_Playing(object sender, RoutedEventArgs args)
        {
            await GetCurrentSong("-768839342");
        }

        private async void Play_Next(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(mainDeviceIp)) { return; }

            try
            {
                string result = await api.SendCommand(
                                            mainDeviceIp,
                                            HeosAction.Play_Next,
                                            new List<KeyValuePair<string, string>>
                                                {
                                                    new KeyValuePair<string, string>("pid", "-768839342")
                                                });

            }
            catch (Exception exception)
            {
                // OH NOES!
            }
        }

        private async Task GetCurrentSong(string pid)
        {
            if (string.IsNullOrEmpty(mainDeviceIp)) { return; }

            try
            {
                string result = await api.SendCommand(
                                            mainDeviceIp,
                                            HeosAction.Get_Now_Playing_Media,
                                            new List<KeyValuePair<string, string>>
                                                {
                                                    new KeyValuePair<string, string>("pid",pid)
                                                });

                dynamic json = JObject.Parse(result);
                string albumUrl = json?.payload?.image_url;

                if (!string.IsNullOrEmpty(albumUrl))
                {
                    //  http://stackoverflow.com/a/38150056
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        //UI code here
                        AlbumCover.Source = new BitmapImage(new Uri(albumUrl, UriKind.Absolute));
                    });
                }
            }
            catch (Exception exception)
            {
                // OH NOES!
            }
        }
    }
}
