using System.Net;
using System.Net.Sockets;
using System.Text;

var ip = IPAddress.Loopback;
var port = 8000;
var listener = new TcpListener(ip, port);

listener.Start();
Console.WriteLine($"Listening on {ip}:{port}");

while (true)
{
    Console.WriteLine("Waiting for client...");
    var client = listener.AcceptTcpClient();
    Console.WriteLine("Client connected");

    var stream = client.GetStream();
    var buffer = new byte[client.ReceiveBufferSize];
    var bytesRead = stream.Read(buffer, 0, buffer.Length);
    var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
    var numbers = Array.ConvertAll(request.Split(','), int.Parse);
    var maxNumber = numbers.Max();

    var response = Encoding.UTF8.GetBytes(maxNumber.ToString());
    stream.Write(response, 0, response.Length);
    Console.WriteLine($"Sent back the max number: {maxNumber}");

    client.Close();
}