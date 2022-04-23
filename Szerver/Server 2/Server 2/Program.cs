//#define DB_DEBUG

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server_2;

public class Server
{
    #region variables
    Socket? listener;
    IPEndPoint? localEndPoint;
    List<Sassion> sassion_list = new List<Sassion>();
    List<Thread> sassion_threads = new List<Thread>();
    public bool live = false;
    Thread main_thread;
    #endregion

    public Server()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());

        for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
        {
            Console.WriteLine("[" + i + "]: " + ipHostInfo.AddressList[i].ToString());
        }
        IPAddress ipAddress = ipHostInfo.AddressList[Convert.ToInt16(Console.ReadLine())];
        localEndPoint = new IPEndPoint(ipAddress, 8888);
        Console.Clear();
        //Console.WriteLine(ipAddress.ToString());

        listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
    }

    public void startListening()
    {
        main_thread = new Thread(startServer);
        live = true;
        main_thread.Start();

        while (live)
        {
            string input = Console.ReadLine();

            if(input == "clear")
            {
                Console.Clear();
            }

            if(input == "close")
            {
                live = false;
            }

            if(input.Contains("list"))
            {
                if(input.Contains("sockets"))
                {
                    int j = 0;
                }
            }
        }
        main_thread.Join();

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();
    }

    public void startServer()
    {
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (live)
            {
                Sassion aktual_sassion = new Sassion(false, listener.Accept());
                sassion_list.Add(aktual_sassion);
                Thread aktual_thread = new Thread(sassionThread);
                aktual_thread.Start(aktual_sassion);
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void sassionThread(Object obj)
    {
        Sassion aktual_sassion = (Sassion)obj;

        while(aktual_sassion.workSocket.Connected && aktual_sassion.live)
        {
            try
            {
                int bytes_read = aktual_sassion.workSocket.Receive(aktual_sassion.buffer, 0, Sassion.BufferSize, 0);

                if (bytes_read > 0)
                {
                    aktual_sassion.sb.Append(Encoding.UTF8.GetString(aktual_sassion.buffer, 0, bytes_read));
                    String receive_content = aktual_sassion.sb.ToString();
                    aktual_sassion.write("Read " + bytes_read + " bytes from client: " + receive_content);

                    String send_content = aktual_sassion.solve(receive_content);
                    byte[] send_bytes = Encoding.UTF8.GetBytes(send_content + "\n");
                    int bytes_send = aktual_sassion.workSocket.Send(send_bytes);
                    aktual_sassion.write("Send " + bytes_send + " bytes to client: " + send_content);
                    aktual_sassion.sb.Clear();
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    aktual_sassion.workSocket.Shutdown(SocketShutdown.Both);
                    aktual_sassion.workSocket.Close();
                    aktual_sassion.live = false;
                    return;
                }

                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        Sassion.sassionStopd(aktual_sassion);
    }
}

public class Program
{
    public static int Main(String[] args)
    {
        Server server = new Server();
        server.startListening();
        return 0;
    }
}