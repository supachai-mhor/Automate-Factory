using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Channels;

namespace AutomateBussiness.Hubs
{
    public class StreamHub : Hub
    {
        public ChannelReader<int> DelayCounter(int delay)
        {
            var channel = Channel.CreateUnbounded<int>();

            _ = WriteItems(channel.Writer, 20, delay);

            return channel.Reader;
        }

        private async Task WriteItems(ChannelWriter<int> writer, int count, int delay)
        {
            for (var i = 0; i < count; i++)
            {
                await writer.WriteAsync(i);
                await Task.Delay(delay);
            }

            writer.TryComplete();
        }
        public async Task UploadStream(ChannelReader<string> stream)
        {
            while (await stream.WaitToReadAsync())
            {
                while (stream.TryRead(out var item))
                {
                    // do something with the stream item
                    Console.WriteLine(item);
                }
            }
        }
    }
}
