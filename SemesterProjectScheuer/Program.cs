
namespace SemesterProjectScheuer
{
    class main
    {
        static void Main()
        {
            int port = 10001;
            HttpServer meinServer = new HttpServer(port);
            meinServer.Start();

            Console.WriteLine("Press 'q' to stop the server...");
            while (true)
            {
                // Console.WriteLine("Waiting for a request..."); anm. war ein kurzer test 
                if (Console.ReadKey(true).KeyChar == 'q')
                {
                    meinServer.Stop();
                    break;
                }
            }
            Console.WriteLine("Server gestoppt. Tschüss!");
        }
    }
}