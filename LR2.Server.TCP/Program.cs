using System.Net;
using System.Net.Sockets;
using System.Text;

var connectionsCount = 0;
object locker = new();
var ip = IPAddress.Any;
const int port = 9000;
var listener = new TcpListener(IPAddress.Any, port);
using var cancellationTokenSource = new CancellationTokenSource();
var token = cancellationTokenSource.Token;
try
{
    listener.Start();
    Console.WriteLine($"Listening on {ip}:{port}");

    var listenTask = Task.Run(async () =>
    {
        while (!token.IsCancellationRequested)
        {
            var clientTask = listener.AcceptTcpClientAsync(); // Use async method

            lock (locker)
            {
                connectionsCount++;
            }
            
            if (clientTask == await Task.WhenAny(clientTask, Task.Delay(-1, token)))
            {
                var client = await clientTask;
                Console.WriteLine("Client connected");

                await using var stream = client.GetStream();
                var buffer = new byte[client.ReceiveBufferSize];
                var bytesRead = await stream.ReadAsync(buffer, token);
                var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var numbers = Array.ConvertAll(request.Split(','), int.Parse);
                var maxNumber = numbers.Max();

                var response = Encoding.UTF8.GetBytes(maxNumber.ToString());
                await stream.WriteAsync(response, token);
                Console.WriteLine($"Sent back the max number: {maxNumber}");

                client.Close();
            }
        }
    }, token);

    Console.WriteLine("Press any key to stop the server...");
    Console.ReadKey();

    cancellationTokenSource.Cancel();
    await listenTask; // Wait for the listen task to complete.
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}
finally
{
    listener.Stop();
    Console.WriteLine("Server stopped");
    Console.WriteLine($"Total connections: {connectionsCount}");
}