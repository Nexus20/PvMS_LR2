using System.Net;
using System.Net.Sockets;
using System.Text;

var server = new UdpClient(8001);
var remoteIpEndPoint = new IPEndPoint(IPAddress.Any, 0);
Console.WriteLine("Server is running");

while (true)
{
    Console.WriteLine("Waiting for a message...");
    var receiveBytes = server.Receive(ref remoteIpEndPoint);
    var receivedData = Encoding.UTF8.GetString(receiveBytes);
    Console.WriteLine($"Received: {receivedData}");

    var numbers = receivedData.Split(',').Select(int.Parse).ToArray();
    var maxNumber = numbers.Max();
    var responseData = Encoding.UTF8.GetBytes(maxNumber.ToString());

    server.Send(responseData, responseData.Length, remoteIpEndPoint);
    Console.WriteLine($"Sent back the max number: {maxNumber}");
}