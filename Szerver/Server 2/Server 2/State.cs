using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Server_2
{
    internal class Sassion
    {
        static SqliteConnection connection = new SqliteConnection("Data Source=karbantartas-menedzsment.db");

        public ManualResetEvent allDone = new ManualResetEvent(false);

        public const int BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public Socket workSocket = null;

        string mhash = "";

        public Sassion()
        {
            mhash = RandomHash();
            Console.WriteLine("New Sassion started! \t -->" + mhash);
        }

        ~Sassion()
        {
            Console.WriteLine("Sassion deleted!\t-->" + mhash);
        }

        public static void sassionStopd(Sassion s)
        {
            Console.WriteLine("Sassion stoped!\t-->" + s.mhash);
        }

        public void write(string ms)
        {
            Console.WriteLine(mhash + "--->" + ms);
        }

        public string solve(string json)
        {
            JsonCommunication? js = JsonSerializer.Deserialize<JsonCommunication>(json);

            switch (js?.code)
            {
                case 1:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(belepes(js));
                    break;

                case 2:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(kilepes(js));
                    break;

                case 3:
                    break;

                default:
                    Console.WriteLine("Nem ismert kérés");
                    break;

            }

            return "";
        }

        string RandomHash()
        {
            return Guid.NewGuid().ToString("N");
        }

        JsonCommunicationResponse belepes(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT Nev
                FROM Felhasznalo
                WHERE FelhasznaloNev =  '" + js.username + "' " +
                "AND Jelszo ='" + js.password + "'";
            //Console.WriteLine(command.CommandText);

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    JsonCommunicationResponse jsrok = new JsonCommunicationResponse
                    {
                        hash = mhash,
                        state = 0,
                        name = reader.GetString(0),
                        role = "admin"
                    };

                    return jsrok;
                }
                write("Nincs kért Felhasználó");
            }
            dbClose();
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 1
            };
            return jsr;
        }

        JsonCommunicationResponse kilepes(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 1
            };
            workSocket.Close();
            return jsr;
        }

        static void dbOpen()
        {
            connection.Open();
        }

        static void dbClose()
        {
            connection.Close();
        }
    }
}
