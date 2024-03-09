using System.Net;
using System.Net.Sockets;
using System.Text;

const int port = 9001;
const int clientCount = 100000;

var tasks = new List<Task>();
for (var i = 0; i < clientCount; i++)
{
    var task = Task.Run(() => SimulateClient(IPAddress.Loopback, port));
    tasks.Add(task);
}

await Task.WhenAll(tasks);
Console.WriteLine("All clients have completed");
return;

static async Task SimulateClient(IPAddress serverAddress, int port)
{
    try
    {
        using var client = new UdpClient();
        var serverEndpoint = new IPEndPoint(serverAddress, port);
        var message = "1,2,3,4,5";
        var bytesToSend = Encoding.UTF8.GetBytes(message);
        Console.WriteLine($"Sending: {message}");

        await client.SendAsync(bytesToSend, bytesToSend.Length, serverEndpoint);

        var serverResponseData = await client.ReceiveAsync();
        var serverResponse = Encoding.UTF8.GetString(serverResponseData.Buffer);
        Console.WriteLine($"Received max number: {serverResponse}");

        client.Close();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}");
    }
}