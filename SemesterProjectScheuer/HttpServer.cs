namespace SemesterProjectScheuer;

using System.Net;
using System.Net.Sockets;

public class HttpServer(int port)
{
    private int _port = port;
    private TcpListener _listener = null!;
    private bool _isListening;

    public async Task Start()
    {
        _isListening = true;
        _listener = new TcpListener(IPAddress.Any, _port); // Erstellt einen TCP-Listener, der auf allen IP-Adressen und dem angegebenen Port auf Verbindungen wartet
        _listener.Start();
        
        Console.WriteLine($"[HttpServer] Server is running on port {_port} ... ");

        while (_isListening) //Solange der Server läuft apzeptiert er asynchron clients
        {
            try
            {
                var client = await _listener.AcceptTcpClientAsync();
                Console.WriteLine($"[HttpServer] Client connected.");
                
                //Asynchrone Verarbeitung der clients 
                _ = HandleClientAsync(client); //Discard -> sagt dem compiler "ich weiss, dass ich auf die Variable nicht warten muss"
            }
            catch (SocketException ) when (!_isListening)
            {
                //wenn der Server gestopped wird soll einfach nur gebreaked werden.
                break;
            }
            catch (Exception e)
            {
                Console.WriteLine(e); //printe die Exception
                throw;
            }
        }
    }
    public void Stop()
    {
        _isListening = false;
        try { _listener.Stop(); } catch { /* ignoriiere */ }
        Console.WriteLine("[HttpServer] Server gestopped.");
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        using(client) // wenn ich fertig bin, wird der client automatisch geschlossen

        await using (var stream = client.GetStream()) 
        {
            try
            {
                var request = await HttpRequestParser.ParseFromStreamAsync(stream);
                Console.WriteLine($"[HttpServer] Request => Methode: {request.Method}, Pfad: {request.Path}");

                var response = Router.Route(request); //übergibt den request an den Router

                var responseBytes = response.GetBytes();

                await stream.WriteAsync(responseBytes, 0, responseBytes.Length);
                Console.WriteLine($"[HttpServer] Response gesendet mit Status Code: {response.StatusCode} ");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
