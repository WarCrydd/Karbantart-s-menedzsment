//#define DB_DEBUG

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server_2;

public class AsynchronousSocketListener
{
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    static Socket listener;

    static IPEndPoint localEndPoint;

    public static Dictionary<string, Sassion> sassions = new Dictionary<string, Sassion>();

    public static bool live = false;

    public AsynchronousSocketListener()
    {
    }

    public static void StartListening()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        for (int i = 0; i < ipHostInfo.AddressList.Length; i++)
        {
            Console.WriteLine("[" + i + "]: " + ipHostInfo.AddressList[i].ToString());
        }

        IPAddress ipAddress = ipHostInfo.AddressList[Convert.ToInt16(Console.ReadLine())];
        localEndPoint = new IPEndPoint(ipAddress, 8888);
        Console.Clear();
        Console.WriteLine(ipAddress.ToString());

        listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);

        Thread main = new Thread(start);
        main.Start();

        live = true;

        while(live)
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
                    foreach(var i in sassions)
                    {
                        Console.WriteLine("[" + j + "]---->>>>" + i.Key);
                    }
                }
            }
        }
        main.Join();

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static void start()
    {
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (live)
            {
                allDone.Reset();

                listener.BeginAccept(
                    new AsyncCallback(AcceptCallback),
                    listener);

                allDone.WaitOne();
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        allDone.Set();

        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        Sassion state = new Sassion();
        sassions.Add(state.mhash, state);
        state.workSocket = handler;
        while(state.workSocket.Connected || state.live)
        {
            state.allDone.Reset();
            handler.BeginReceive(state.buffer, 0, Sassion.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
            state.allDone.WaitOne();
        }
        Sassion.sassionStopd(state);
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        Sassion state = new Sassion(true);
        try
        {
            String content = String.Empty;

            state = (Sassion)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead));
                content = "";
                content = state.sb.ToString();
                while (content.IndexOf("}{") > 0)
                {
                    content = content.Replace("}{", ",");
                }
                state.write("Read " + content.Length + " bytes from client: " + content);
            }

            JsonCommunication js = Sassion.dataToJson(content);
            if (js.code != 1)
            {
                sassions[js.hash].workSocket = state.workSocket;
                state = sassions[js.hash];
            }

            Send(state,state.solve(content) + "\n");
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            if (ex.ErrorCode == 10054)
            {
                state.workSocket.Shutdown(SocketShutdown.Both);
                state.workSocket.Close();
                state.live = false;
                return;
            }

            Console.WriteLine(ex.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static void Send(Sassion state, String data)
    {
        try
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            state.write("Sent " + byteData.Length + "byte data to client: " + data.Substring(0, data.Length-1));
            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
        }
        catch(System.Net.Sockets.SocketException ex)
        {
            if (ex.ErrorCode == 10054)
            { 
                state.workSocket.Shutdown(SocketShutdown.Both);
                state.workSocket.Close();
                state.live = false;
                return;
            }

            Console.WriteLine(ex.ToString());
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        Sassion state = (Sassion)ar.AsyncState;
        try
        {  
            int bytesSent = state.workSocket.EndSend(ar);
            
        }
        catch (System.Net.Sockets.SocketException ex)
        {
            if (ex.ErrorCode == 10054)
            {
                state.workSocket.Shutdown(SocketShutdown.Both);
                state.workSocket.Close();
                state.live = false;
                return;
            }

            Console.WriteLine(ex.ToString());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        state.allDone.Set();
    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}