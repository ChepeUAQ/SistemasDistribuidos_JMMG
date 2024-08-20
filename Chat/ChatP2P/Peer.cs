using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ChatP2P;

public class Peer {
    private readonly TcpListener _listener;
    private TcpClient? _client;
    private const int Port = 8080;

    public Peer() => _listener = new TcpListener(IPAddress.Any, Port);
    
    public async Task ConnectToPeer(string ipAdd, string port) {
        try {
            _client = new TcpClient(ipAdd, Convert.ToInt32(port));
            Console.WriteLine("Connected to Peer");

            var recieveTask = RecieveMessage();
            await SendMessage();
            await recieveTask;
        } catch (Exception ex) {
            Console.WriteLine("Connection closed " + ex.Message);
        }
    }

    public async Task StartListening() {
        try {
            _listener.Start();
            Console.WriteLine("Listening for incoming connections...");
            _client = await _listener.AcceptTcpClientAsync();
            Console.WriteLine("Connected to peer...");

            var recieveTask = RecieveMessage();
            await SendMessage();
            await recieveTask;
        } catch (Exception ex) {
            Console.WriteLine("Connection closed " + ex.Message);
        }
    }

    private async Task RecieveMessage() {
        try {
            var stream = _client!.GetStream();
            var reader = new StreamReader(stream, Encoding.UTF8);
            
            while (true) {
                var message = await reader.ReadLineAsync();
                if (string.IsNullOrEmpty(message)) break;

                Console.WriteLine($"Peer message: {message}");
            }
        } catch (Exception ex) {            
            Console.WriteLine($"Error recieveng message: {ex.Message}");
        } finally {
            Close();
        }
    }

    private async Task SendMessage() {
        try {
            var stream = _client!.GetStream();
            var writer = new StreamWriter(stream, Encoding.UTF8) {AutoFlush = true};

            while (true) {
                var message = Console.ReadLine();
                if (string.IsNullOrEmpty(message)) break;

                await writer.WriteLineAsync(message);
            }
        } catch (Exception ex) {
            Console.WriteLine($"Error recieveng message: {ex.Message}");
        } finally {
            Close();
        }
    }

    private void Close() {
        _client?.Close();
        _listener.Stop();
    }
}