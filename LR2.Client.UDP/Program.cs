using System.Net;
using System.Net.Sockets;
using System.Text;

var client = new UdpClient();
var serverEndpoint = new IPEndPoint(IPAddress.Loopback, 9001);

var message = "1,2,3,4,5";
var bytesToSend = Encoding.UTF8.GetBytes(message);
Console.WriteLine($"Sending: {message}");

client.Send(bytesToSend, bytesToSend.Length, serverEndpoint);

var serverResponseData = client.Receive(ref serverEndpoint);
var serverResponse = Encoding.UTF8.GetString(serverResponseData);
Console.WriteLine($"Received max number: {serverResponse}");

client.Close();