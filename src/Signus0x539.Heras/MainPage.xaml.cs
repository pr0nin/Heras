using Newtonsoft.Json;
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
        private string mainPid = "-768839342";

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
            mainDeviceIp = discovery.GetMainIp();

            if (string.IsNullOrEmpty(mainDeviceIp)) { return; }

            var commandTask = api.SendCommand(mainDeviceIp,
                HeosAction.Register_For_Change_Events,
                new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("enable", "off")
                    });

            await Task.WhenAll(commandTask);
            await GetCurrentSong(mainPid);
            await Task.Run(async () => await GetCurrentSongLoop());
        }

        private async Task GetCurrentSongLoop()
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await GetCurrentSong(mainPid);
            }
        }

        private async void Get_Playing(object sender, RoutedEventArgs args)
        {
            await GetCurrentSong(mainPid);
        }

        private async void Play_Next(object sender, RoutedEventArgs args)
        {
            if (string.IsNullOrEmpty(mainDeviceIp)) { return; }

            try
            {
                var commandTask = api.SendCommand(
                                            mainDeviceIp,
                                            HeosAction.Play_Next,
                                            new List<KeyValuePair<string, string>>
                                                {
                                                    new KeyValuePair<string, string>("pid", mainPid)
                                                });


                await Task.WhenAll(commandTask);
                if (string.IsNullOrEmpty(commandTask.Result)) { return; }

                await Task.Run( async () =>
                {
                    await Task.Delay(5000);
                    await GetCurrentSong(mainPid);
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

                if (string.IsNullOrEmpty(result)) { return; }

                JObject json = JObject.Parse(result);
                if (json != null)
                {
                    var payload = json["payload"].Value<JObject>();
                    string albumUrl = payload["image_url"].Value<string>();

                    if (!string.IsNullOrEmpty(albumUrl))
                    {
                        //  http://stackoverflow.com/a/38150056
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            //UI code here
                            var albumUri = new Uri(albumUrl, UriKind.Absolute);
                            if (AlbumCover.BaseUri.AbsoluteUri != albumUri.AbsoluteUri)
                            {
                                AlbumCover.Source = new BitmapImage(albumUri);
                            }
                        });
                    }
                }
            }
            catch (Exception exception)
            {
                // OH NOES!
            }
        }
    }
}
