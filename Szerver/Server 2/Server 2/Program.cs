using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server_2;

public class AsynchronousSocketListener
{
    public static ManualResetEvent allDone = new ManualResetEvent(false);

    static List<Thread> threads = new List<Thread>();

    public AsynchronousSocketListener()
    {
    }

    public static void StartListening()
    {
        IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
        for(int i = 0; i< ipHostInfo.AddressList.Length; i++)
        {
            Console.WriteLine("[" + i + "]: " + ipHostInfo.AddressList[i].ToString());
        }

        IPAddress ipAddress = ipHostInfo.AddressList[Convert.ToInt16(Console.ReadLine())];
        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 8888);
        Console.WriteLine(ipAddress.ToString());
 
        Socket listener = new Socket(ipAddress.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
 
        try
        {
            listener.Bind(localEndPoint);
            listener.Listen(100);

            while (true)
            {
                allDone.Reset();
 
                //Console.WriteLine("Waiting for a connection...");
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

        Console.WriteLine("\nPress ENTER to continue...");
        Console.Read();

    }

    public static void AcceptCallback(IAsyncResult ar)
    {
        allDone.Set();

        Socket listener = (Socket)ar.AsyncState;
        Socket handler = listener.EndAccept(ar);

        Sassion state = new Sassion();
        state.workSocket = handler;
        while(state.workSocket.Connected)
        {
            state.allDone.Reset();
            handler.BeginReceive(state.buffer, 0, Sassion.BufferSize, 0,
            new AsyncCallback(ReadCallback), state);
            state.allDone.WaitOne();
        }
        Sassion.sassionStopd(state);
        //threads.Add(new Thread(state.start));
    }

    public static void ReadCallback(IAsyncResult ar)
    {
        try
        {
            String content = String.Empty;

            Sassion state = (Sassion)ar.AsyncState;
            Socket handler = state.workSocket;

            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                state.sb.Append(Encoding.UTF8.GetString(
                    state.buffer, 0, bytesRead));

                content = state.sb.ToString();

                state.write("Read " + content.Length +" bytes from client: " + content);
            }

            Send(state, content.Substring(0, 2) + state.solve(content)+"\r\n");
        }
        catch (Exception ex)
        {
            Sassion state = (Sassion)ar.AsyncState;
            if(!state.workSocket.Connected)
                Console.WriteLine(ex.ToString());
        }
    }

    private static void Send(Sassion state, String data)
    {
        try
        {
            byte[] byteData = Encoding.UTF8.GetBytes(data);
            state.write("data to client:" + data);
            state.workSocket.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), state);
        }
        catch(Exception ex)
        {
            if (!state.workSocket.Connected)
                Console.WriteLine(ex.ToString());
        }
    }

    private static void SendCallback(IAsyncResult ar)
    {
        Sassion state = (Sassion)ar.AsyncState;
        try
        {  
            int bytesSent = state.workSocket.EndSend(ar);
            state.write("Sent " + bytesSent + " bytes to client.");

            state.workSocket.Shutdown(SocketShutdown.Both);
            state.workSocket.Close();
            
        }
        catch (Exception e)
        {
            if (!state.workSocket.Connected)
                Console.WriteLine(e.ToString());
        }
        state.allDone.Set();
    }

    public static int Main(String[] args)
    {
        StartListening();
        return 0;
    }
}