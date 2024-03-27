using System.Net;
using StardewDiscordPacketer;

namespace PierreHateGroupDiscordBot.WebSocketHandler;

internal class StardewSocketHandler
{
    private HttpListener listener;
    private CancellationTokenSource cts;
    public event EventHandler<Packet?>? MessageReceived;

    public StardewSocketHandler(string[] prefixes)
    {
        listener = new HttpListener();
        cts = new CancellationTokenSource();
        foreach(var prefix in prefixes)
        {
            listener.Prefixes.Add(prefix);
        }
    }

    public async Task StartAsync()
    {
        listener.Start();
        Console.WriteLine("Http server started");

        try
        {
            while(!cts.Token.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                var req = context.Request;

                using var reader = new StreamReader(req.InputStream, req.ContentEncoding);
                string packet = await reader.ReadToEndAsync();

                Packet? message = Packet.FromJson(packet);

                MessageReceived?.Invoke(this, message);
            }
        }

        catch(HttpListenerException)
        {
            Console.WriteLine($"Listener failed.");
        }
    }

    public void Stop()
    {
        listener.Stop();
        cts.Cancel();
    }
}
