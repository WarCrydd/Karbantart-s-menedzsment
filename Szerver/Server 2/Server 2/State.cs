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

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public const int BufferSize = 1024;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public Socket workSocket = null;

        public string mhash = "";

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
            Console.WriteLine(">>>>>>>>    " + mhash + "    <<<<<<<<");
            Console.ForegroundColor = ConsoleColor.Blue;
            //Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine(ms);
            Console.ResetColor();
        }

        public static void swrite(string ms)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(">>>>>>>>    " + "Kóbor áram :D" + "    <<<<<<<<");
            Console.ForegroundColor = ConsoleColor.Blue;
            //Console.BackgroundColor = ConsoleColor.Gray;
            Console.WriteLine(ms);
            Console.ResetColor();
        }

        public string solve(string json)
        {
            //write(json);
            JsonCommunication? js = JsonSerializer.Deserialize<JsonCommunication>(json);

            switch (js?.code)
            {
                case 1:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(belepes(js), options);
                    break;

                case 2:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(kilepes(js), options);
                    break;

                case 3:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujKat(js), options);
                    break;

                case 4:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(listKategorioa(js), options);
                    break;

                case 5:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujFelhasznalo(js), options);
                    break;

                case 6:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujEszkoz(js), options);
                    break;

                case 7:
                    return JsonSerializer.Serialize<JsonCommunicationResponse>(ujKepesites(json), options);

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
                SELECT Nev, Szerep
                FROM Felhasznalo
                WHERE FelhasznaloNev =  '" + js.username + "' " +
                "AND Jelszo ='" + js.password + "'";

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    JsonCommunicationResponse jsrok = new JsonCommunicationResponse
                    {
                        hash = mhash,
                        state = 0,
                        name = (string)record[0],
                        role = (string)record[1]
                    };

                    return jsrok;
                }
                write("Nincs a kért Felhasználó a rendszerben.");
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
                state = 0
            };
            workSocket.Close();
            return jsr;
        }

        JsonCommunicationResponse listKategorioa(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Megnevezes, SzuloKategoriaID, NormaIdo, Karb_periodus, Instrukciok
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

                    Int64 id_ = 0;
                    try 
                    {
                        id_ = Convert.ToInt64(records[0]);
                    }
                    catch
                    {
                        id_ = -1;
                    }
                    Int64 parent_ = 0;
                    try 
                    {
                        parent_ = Convert.ToInt64(records[2]);
                    }
                    catch
                    {
                        parent_ = -1;
                    }
                    Int64 normaido_ = 0;
                    try 
                    {
                        normaido_ = Convert.ToInt64(records[3]);

                    }
                    catch
                    {
                        normaido_ = -1;
                    }
                    Int64 karbperiod_ = 0;
                    try
                    {
                        karbperiod_ = Convert.ToInt64(records[4]);
                    }
                    catch
                    {
                        karbperiod_ = -1;
                    }

                    jsr.kategoria.Add(new JsonKategoria
                    {
                        id = id_,
                        name = (string)records[1],
                        parent = parent_,
                        normaido = normaido_,
                        karbperiod = karbperiod_,
                        leiras = (string)records[5]
                    });
                    
                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse listEszkozok(JsonCommunication js)
        {
            return null;
        }

        JsonCommunicationResponse ujKat(JsonCommunication js)
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
                INSERT INTO Kategoria (Megnevezes, SzuloKategoriaID, NormaIdo, Karb_periodus, Instrukciok)
                VALUES( '"
                        + js.name + "', "
                        + js.parent + ", "
                        + js.normaido + ", "
                        + js.karbperiod + ", '"
                        + js.leiras + "')";
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

        JsonCommunicationResponse ujFelhasznalo(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };

            try
            {
                //dbOpen();
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                INSERT INTO Felhasznalo (Nev, FelhasznaloNev, Jelszo, KepesitesID, Munkaorak_szama, Szerep)
                VALUES( '"
                        + js.name + "', '"
                        + js.username + "', '"
                        + js.password + "', "
                        + getKepesitesID(js.school) + ", "
                        + "2" +", '" 
                        + js.role + "')";

                write(command.CommandText);
                dbOpen();
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
                INSERT INTO Eszkozok (Megnevezes, KategoriaID, Leiras, Elhelyezkedes)
                VALUES( '"
                    + js.name + "', "
                    + js.kategoriaid + ", '"
                    + js.leiras + "', '"
                    + js.elhelyezkedes + "' "
                    + ")";
            write(command.CommandText);
            if (command.ExecuteNonQuery() == -1)
            {
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }
            dbClose();
            return jsr;
        }

        JsonCommunicationResponse ujKepesites(string json)
        {
            JsonVegzetseg js = JsonSerializer.Deserialize<JsonVegzetseg>(json);

            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO Kepesites (Megnevezes)
                VALUES( '"
                    + js.name + "')";
            dbOpen();
            write(command.CommandText);
            if (command.ExecuteNonQuery() == -1)
            {
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }

            command.CommandText = "SELECT MAX(ID) FROM Kepesites";
            var reader = command.ExecuteReader();
            if (!reader.Read())
            {
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }
            IDataRecord records = (IDataRecord)reader;
            int id = (int)records[0];

            command.CommandText = "INSERT INTO Szerelheti (KategoriaID, KepesitesID) VALUES";
            foreach(var i in js.kategoriak)
            {
                command.CommandText += "(" + i + "," + id + "),";
            }
            command.CommandText = command.CommandText.Substring(0, command.CommandText.Length - 1);
            if (command.ExecuteNonQuery() == -1)
            {
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }
            dbClose();
                
            return jsr;
        }

        static void dbOpen()
        {
            connection.Open();
            Sassion.swrite("DB kinyit");
        }

        static void dbClose()
        {
            connection.Close();
            Sassion.swrite("DB zár");
        }

        public static JsonCommunication dataToJson(string data)
        {
            return JsonSerializer.Deserialize<JsonCommunication>(data);
        }

        Int64 getKepesitesID(string data)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select ID
                FROM Kepesites
                WHERE Megnevezes = '" + data +"'";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    result = (Int64)record[0];                    
                }
            }

            dbClose();
            return result;
        }
    }
}
