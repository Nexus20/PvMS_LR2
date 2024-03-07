using System.Net.Sockets;
using System.Text;

var connectionsCount = 0;
object locker = new();

var server = new UdpClient(9001);
Console.WriteLine("Server is running");
Console.WriteLine("Waiting for a message...");

using var cancellationTokenSource = new CancellationTokenSource();
var token = cancellationTokenSource.Token;

try
{
    var receiveMessagesTask = Task.Run(async () =>
    {
        while (!token.IsCancellationRequested)
        {
            var receiveTask = server.ReceiveAsync();
            lock (locker)
            {
                connectionsCount++;
            }

            if (receiveTask == await Task.WhenAny(receiveTask, Task.Delay(-1, token)))
            {
                var receiveResult = await receiveTask;

                var receivedData = Encoding.UTF8.GetString(receiveResult.Buffer);
                Console.WriteLine($"Received: {receivedData}");

                var numbers = receivedData.Split(',').Select(int.Parse).ToArray();
                var maxNumber = numbers.Max();
                var responseData = Encoding.UTF8.GetBytes(maxNumber.ToString());

                await server.SendAsync(responseData, responseData.Length, receiveResult.RemoteEndPoint);
                Console.WriteLine($"Sent back the max number: {maxNumber}");
            }
        }
    });
    
    Console.WriteLine("Press any key to stop the server...");
    Console.ReadKey();

    cancellationTokenSource.Cancel();
    await receiveMessagesTask; // Wait for the receive messages task to complete.
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
}
finally
{
    server.Close();
    Console.WriteLine("Server stopped");
    Console.WriteLine($"Total connections: {connectionsCount}");
}