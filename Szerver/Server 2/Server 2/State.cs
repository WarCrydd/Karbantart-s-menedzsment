using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace Server_2
{
    public class Sassion
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
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("New Sassion started! \t -->" + mhash);
            Console.ResetColor();
        }

        ~Sassion()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sassion deleted!\t-->" + mhash);
            Console.ResetColor();
        }

        public static void sassionStopd(Sassion s)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Sassion stoped!\t-->" + s.mhash);
            Console.ResetColor();
        }

        public void write(string ms)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(mhash + "--->");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(ms);
            Console.ResetColor();
        }

        public string solve(string json)
        {
            write(json.Substring(2));
            JsonCommunication? js = JsonSerializer.Deserialize<JsonCommunication>(json.Substring(2));

            switch (js?.code)
            {
                case 1:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(belepes(js));
                    break;

                case 2:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(kilepes(js));
                    break;

                case 3:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujKat(js));
                    break;

                case 4:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(katList(js));
                    break;

                case 5:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujFelhasznalo(js));
                    break;

                case 6:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujEszkoz(js));
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

        JsonCommunicationResponse katList(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Megnevezes, SzuloKategoriaID, NormaIdo, Karb_periouds, Instrukciok
                FROM Kategoria
                ";
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                kategoria = new List<JsonKategoria>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.kategoria.Add(new JsonKategoria
                    {
                        id = (int)records[0],
                        name = (string)records[1],
                        parent = (int)records[2],
                        normaido = (int)records[3],
                        karbperiod = (int)records[4],
                        leiras = (string)records[5]
                    });
                    
                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse ujKat(JsonCommunication js)
        {

            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            try
            {
                dbOpen();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                INSERT INTO Kategoria (Megnevezes, SzuloKategoriaID, NormaIdo, Karb_periouds, Instrukciok)
                VALUES( '"
                        + js.name + "', "
                        + js.parent + ", "
                        + js.normaido + ", "
                        + js.karbperiod + ", '"
                        + js.leiras + "')";
                if (command.ExecuteNonQuery() == -1)
                {
                    return new JsonCommunicationResponse
                    {
                        state = 1
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            dbClose();
            return jsr;
        }

        JsonCommunicationResponse ujFelhasznalo(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            dbOpen();
            try
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                INSERT INTO Felhasznalo (Nev, FelhasznaloNev, Jelszo, KepesitesID, Munkaorak_szama)
                VALUES( '"
                        + js.name + "', '"
                        + js.username + "', '"
                        + js.password + "', "
                        + js.school + ", "
                        + "8" + ")";
                write(command.CommandText);
                if (command.ExecuteNonQuery() == -1)
                {
                    return new JsonCommunicationResponse
                    {
                        state = 1
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }
            dbClose();
            return jsr;
        }

        JsonCommunicationResponse ujEszkoz(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO Eszkozok (Nev, KategoriaID, Leiras, Elhelyezkedes)
                VALUES( '"
                    + js.name + "', "
                    + js.katid + ", '"
                    + js.leiras + "', '"
                    + js.elhelyezkedes + "' "
                    +")";
            if (command.ExecuteNonQuery() == -1)
            {
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }

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
