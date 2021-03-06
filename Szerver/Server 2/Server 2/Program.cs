//#define MYDEBUG

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Server_2;
using Server_2.Sassions;

public class Server
{
    #region variables
    Socket? listener;
    IPEndPoint? localEndPoint;
    List<Thread> sassion_threads = new List<Thread>();
    public bool live = false;
    Thread main_thread;
    #endregion

    void write(string ms)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("A SERVER: " + ms);
        Console.ResetColor();
    }

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
        write("A server IP címe: " + ipAddress.ToString());

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

            string[] datas = input.Split(' ');

            if(datas[0] == "clear")
            {
                Console.Clear();
                write("Done!");
            }
            else if(datas[0] == "close")
            {
                if(datas.Length <= 1)
                {
                    write("A \"close\" parancs önmagában nem értelmezett");
                }
                if (datas[1] == "server")
                {
                    live = false;
                    main_thread.Abort();
                    foreach (var th in sassion_threads)
                    {
                        th.Join();
                    }
                    write("Done!");
                }
                else if (datas[1] == "sassion")
                {
                    if (!Sassion.sassions.ContainsKey(datas[2]))
                    {
                        write("A rendszer nem tartalmazza a megadott sassion-t!");
                        continue;
                    }
                    Sassion.sassions[datas[2]].live = false;
                    write("Done!, Az utolsó kommmunikáció befejezése után a sassion leáll.");
                }
            }
            else if(datas[0] == "list")
            {
                foreach(var sassion in Sassion.sassions)
                {
                    write("[" + sassion.Key + "] -- " + sassion.Value.name);
                }
            }
            else if (datas[0] == "shutdown")
            {
                main_thread.Abort();
                foreach (var th in sassion_threads)
                {
                    th.Abort();
                }
                live = false;
                write("Done!");
            }
            else if (datas[0] == "save")
            {
                if (datas[1] == "log")
                {
                    saveLog();
                }
            }
        }

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
                Socket _socket = listener.Accept();
                Thread aktual_thread = new Thread(sassionThread);
                sassion_threads.Add(aktual_thread);
                aktual_thread.Start(_socket);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.ToString());
        }
    }

    public void sassionThread(Object obj)
    {
        Socket workSocket = (Socket)obj;
        workSocket.ReceiveTimeout = Sassion.session_timout+1000;
        string? aktual_sassion = null;
        byte[] buffer = new byte[Sassion.BufferSize];
        StringBuilder sb = new StringBuilder();

        do
        {
            try
            {
                int bytes_read = 0;
                if (workSocket.Poll(Sassion.session_timout, SelectMode.SelectRead))
                {
                    bytes_read = workSocket.Receive(buffer, 0, Sassion.BufferSize, 0);
                }
                else
                {
                    if(aktual_sassion != null)
                    {
                        break;
                    }

                    aktual_sassion = "Nem azonosított";
                    break;
                }

                if (bytes_read > 0)
                {
                    sb.Append(Encoding.UTF8.GetString(buffer, 0, bytes_read));
                    String receive_content = sb.ToString();
                    if (aktual_sassion == null)
                    {
                        aktual_sassion = Sassion.createOrGetSassion(receive_content);
                    }

                    if(aktual_sassion == null)
                    {
                        byte[] _send_bytes = Encoding.UTF8.GetBytes("{\"state\":1}\n");
                        int _bytes_send = workSocket.Send(_send_bytes);
                        break;
                    }
                    Sassion.sassions[aktual_sassion].log("Read " + bytes_read + " bytes from client: " + receive_content);
                    Sassion.sassions[aktual_sassion].write("Read " + bytes_read + " bytes from client");

                    String send_content = Sassion.sassions[aktual_sassion].solve(receive_content);
                    byte[] send_bytes = Encoding.UTF8.GetBytes(send_content + "\n");
                    int bytes_send = workSocket.Send(send_bytes);
                    Sassion.sassions[aktual_sassion].log("Send " + bytes_send + " bytes to client: " + send_content);
                    Sassion.sassions[aktual_sassion].write("Send " + bytes_send + " bytes to client");
                    sb.Clear();
                }
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.ErrorCode == 10054)
                {
                    workSocket.Shutdown(SocketShutdown.Both);
                    workSocket.Close();
                    Sassion.sassions[aktual_sassion].live = false;
                    return;
                }

                Console.WriteLine(ex.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        } while (workSocket.Connected && Sassion.sassions[aktual_sassion].live);

        if(aktual_sassion != null)
        {
            Sassion.sassions[aktual_sassion].live = false;
            write("A " + aktual_sassion + " véget ért.");
        }
    }

    void saveLog()
    {
        int year = DateTime.Now.Year;
        int month = DateTime.Now.Month;
        int day = DateTime.Now.Day;
        int hour = DateTime.Now.Hour;

        if(!Directory.Exists("logs"))
        {
            Directory.CreateDirectory("logs");
        }

        if (!Directory.Exists("logs/" + year))
        {
            Directory.CreateDirectory("logs/" + year);
        }

        if (!Directory.Exists("logs/" + year + "/" + month))
        {
            Directory.CreateDirectory("logs/" + year + "/" + month);
        }

        if (!Directory.Exists("logs/" + year + "/" + month + "/" + day))
        {
            Directory.CreateDirectory("logs/" + year + "/" + month + "/" + day);
        }

        if (!Directory.Exists("logs/" + year + "/" + month + "/" + day + "/" + hour))
        {
            Directory.CreateDirectory("logs/" + year + "/" + month + "/" + day + "/" + hour);
        }

        foreach(var sassion in Sassion.sassions)
        {
            string path = "logs/" + year + "/" + month + "/" + day + "/" + hour + "/Sassion-" + sassion.Key + ".txt";
            using (StreamWriter sw = File.CreateText(path))
            {
                sw.Write(sassion.Value.getLog());
                write("Sassion" + sassion.Key + ":  Kimentve a '" + path + "' helyre!");
            }
        }

        write("Done");
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