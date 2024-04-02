using System.Net;
using System.Text;
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
                var request = context.Request;
                var response = context.Response;

                using var reader = new StreamReader(request.InputStream, request.ContentEncoding);
                string res = await reader.ReadToEndAsync();

                Packet? packet = Packet.FromJson(res);

                if(request.Url?.AbsolutePath == "/health") {
                    string responseString = "Service is online";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseString);
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    output.Write(buffer, 0, buffer.Length);
                    output.Close();
                    Console.WriteLine($"Received health check");
                    continue;
                }

                if(packet?.Type == PacketType.STARTUP) {
                    byte[] buffer = Encoding.UTF8.GetBytes("Recieved");
                    response.ContentLength64 = buffer.Length;
                    Stream output = response.OutputStream;
                    await output.WriteAsync(buffer, 0, buffer.Length);
                    output.Close();
                }

                MessageReceived?.Invoke(this, packet);
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
