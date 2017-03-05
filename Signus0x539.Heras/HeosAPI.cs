using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signus0x539.Heras
{
    public class HeosAPI
    {
        public HeosAPI()
        {

        }

        public async Task<string> SendCommand(string device, HeosAction action, List<KeyValuePair<string,string>> parameters)
        {
            var command = GetCommand(action, parameters);

            StreamSocketClient client = new StreamSocketClient();
            return await client.Connect(device, "1255", command);
        }

        private string GetCommand(HeosAction action, List<KeyValuePair<string, string>> parameters)
        {
            return string.Format("heos://{0}",
                            string.Format(
                                string.Format(Actions[action],
                                    string.Join("&",
                                        parameters.Select(kvp =>
                                            $"{kvp.Key}={kvp.Value}")))));
        }

        public readonly Dictionary<HeosAction, string> Actions = new Dictionary<HeosAction, string>
        {
            { HeosAction.Register_For_Change_Events,    "system/register_for_change_events?{0}\r\n" },
            { HeosAction.Get_Players,                   "player/get_players\r\n" },
            { HeosAction.Play_Next,                     "player/play_next?{0}\r\n" },
            { HeosAction.Get_Now_Playing_Media,         "player/get_now_playing_media?{0}\r\n" }
        };
    }

    public enum HeosAction
    {
        Register_For_Change_Events = 0,
        Get_Players,
        Play_Next,
        Get_Now_Playing_Media
    }
}
