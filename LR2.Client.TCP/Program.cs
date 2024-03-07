using System.Net;
using System.Net.Sockets;
using System.Text;

var ip = IPAddress.Loopback;
var port = 11377;
var address = Dns.GetHostEntry("5.tcp.eu.ngrok.io").AddressList[0];
using var client = new TcpClient("5.tcp.eu.ngrok.io", port);
using var stream = client.GetStream();

Console.WriteLine("Enter a comma-separated list of numbers (e.g., 1,2,3,4,5):");
var message = Console.ReadLine();
var buffer = Encoding.UTF8.GetBytes(message!);
stream.Write(buffer, 0, buffer.Length);
Console.WriteLine($"Sent: {message}");

buffer = new byte[256];
var bytesRead = stream.Read(buffer, 0, buffer.Length);
var response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
Console.WriteLine($"Received max number: {response}");

stream.Close();
client.Close();