using System.Net;
using System.Net.Sockets;
using System.Text;

var ip = IPAddress.Loopback;
var port = 8000;
var client = new TcpClient("localhost", port);
var stream = client.GetStream();

var message = "1,2,3,4,5"; // Пример массива чисел
var buffer = Encoding.UTF8.GetBytes(message);
stream.Write(buffer, 0, buffer.Length);
Console.WriteLine($"Sent: {message}");

buffer = new byte[256];
var bytesRead = stream.Read(buffer, 0, buffer.Length);
var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
Console.WriteLine($"Received max number: {response}");

stream.Close();
client.Close();