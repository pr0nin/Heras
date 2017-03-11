using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Signus0x539.Heras
{
    public class StreamSocketClient
    {
        private StreamSocket socket;

        public async Task<string> Connect(string host, string port, string message)
        {
            HostName hostName;

            using (socket = new StreamSocket())
            {
                hostName = new HostName(host);

                socket.Control.NoDelay = false;

                try
                {
                    await socket.ConnectAsync(hostName, port);

                    await Send(message);
                    return await Read();
                }
                catch (Exception exception)
                {
                    switch (SocketError.GetStatus(exception.HResult))
                    {
                        case SocketErrorStatus.HostNotFound:
                            throw;
                        default:
                            throw;
                    }
                }
            }
        }

        private async Task Send(string message)
        {
            DataWriter writer;

            using (writer = new DataWriter(socket.OutputStream))
            {
                writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                writer.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                writer.MeasureString(message);
                writer.WriteString(message);

                try
                {
                    await writer.StoreAsync();
                }
                catch (Exception exception)
                {
                    switch (SocketError.GetStatus(exception.HResult))
                    {
                        case SocketErrorStatus.HostNotFound:
                            throw;
                        default:
                            throw;
                    }
                }

                await writer.FlushAsync();

                writer.DetachStream();
            }
        }
        private async Task<string> Read()
        {
            DataReader reader;
            StringBuilder stringBuilder;

            using (reader = new DataReader(socket.InputStream))
            {
                stringBuilder = new StringBuilder();

                reader.InputStreamOptions = Windows.Storage.Streams.InputStreamOptions.Partial;
                reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                reader.ByteOrder = Windows.Storage.Streams.ByteOrder.LittleEndian;

                await reader.LoadAsync(8192);

                while (reader.UnconsumedBufferLength > 0)
                {
                    stringBuilder.Append(reader.ReadString(reader.UnconsumedBufferLength));

                    if (reader.UnconsumedBufferLength > 0)
                    {
                        await reader.LoadAsync(8192);
                    }
                }

                reader.DetachStream();
                return stringBuilder.ToString();
            }
        }
    }
}
