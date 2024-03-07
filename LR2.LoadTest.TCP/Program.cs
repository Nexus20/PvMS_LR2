using System.Net.Sockets;
using System.Text;

// const int port = 11377;
// const string address = "5.tcp.eu.ngrok.io";
const int port = 9000;
const string address = "localhost";
const int clientCount = 5000;

var tasks = new List<Task>();
for (var i = 0; i < clientCount; i++)
{
    var task = Task.Run(() => SimulateClient(address, port));
    tasks.Add(task);
}

await Task.WhenAll(tasks);
Console.WriteLine("All clients have completed");
return;

static async Task SimulateClient(string serverAddress, int port)
{
    try
    {
        using var client = new TcpClient();
        await client.ConnectAsync(serverAddress, port);
        const string message = "1,2,3";
        var buffer = Encoding.UTF8.GetBytes(message);
        await client.GetStream().WriteAsync(buffer);

        // Optionally, read response from server
        buffer = new byte[256];
        var bytesRead = await client.GetStream().ReadAsync(buffer);
        var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        Console.WriteLine($"Received response: {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}