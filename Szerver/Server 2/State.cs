using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace Server_2
{
    public class Sassion
    {
        #region variables
        static string db_null = "null";

        static List<string> hashs = new List<string>();

        static SqliteConnection connection = new SqliteConnection("Data Source=karbantartas-menedzsment.db");

        static bool connection_opened = false;

        public ManualResetEvent allDone = new ManualResetEvent(false);

        JsonSerializerOptions options = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public const int BufferSize = 1024;

        public bool live = false;

        public byte[] buffer = new byte[BufferSize];

        public StringBuilder sb = new StringBuilder();

        public Socket workSocket = null;

        public string mhash = "";
        #endregion

        public Sassion(bool blank = false)
        {
            if (blank)
                return;

            live = true;
            mhash = RandomHash();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("New Sassion started! \t -->" + mhash);
            Console.ResetColor();
        }

        ~Sassion()
        {
            hashs.Remove(mhash);
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
            Console.WriteLine(ms);
            Console.ResetColor();
        }

        public static void swrite(string ms)
        {
#if DB_DEBUG           
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(">>>>>>>>    " + "Kóbor áram :D" + "    <<<<<<<<");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(ms);
            Console.ResetColor();
#endif
        }

        public string solve(string json)
        {
            JsonCommunication? js = JsonSerializer.Deserialize<JsonCommunication>(json);

            string response = "";
            switch (js?.code)
            {
                case 1:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(belepes(js), options);
                    break;

                case 2:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(kilepes(js), options);
                    break;

                case 3:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(ujKat(js), options);
                    break;

                case 4:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(listKategorioa(js), options);
                    break;

                case 5:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(ujFelhasznalo(js), options);
                    break;

                case 6:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(ujEszkoz(js), options);
                    break;

                case 7:
                    response = JsonSerializer.Serialize<JsonCommunicationResponse>(ujKepesites(js), options);
                    break;

                default:
                    Console.WriteLine("Nem ismert kérés");
                    break;

            }

            sb.Clear();

            return response;
        }

        string RandomHash()
        {
            string new_hash = Guid.NewGuid().ToString("N");
            while(hashs.Contains(new_hash))
            {
                new_hash = Guid.NewGuid().ToString("N");
            }
            hashs.Add(new_hash);
            return new_hash;
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

                    jsr.kategoria.Add(new JsonKategoria
                    {
                        id = records[0] == db_null ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1],
                        parent = records[2] == db_null ? -1 : Convert.ToInt64(records[2]),
                        normaido = records[3] == db_null ? -1 : Convert.ToInt64(records[3]),
                        karbperiod = records[4] == db_null ? -1 : Convert.ToInt64(records[4]),
                        leiras = (string)records[5]
                    });
                    
                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse listEszkozok(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Megnevezes, KategoriaID, Leiras, Elhelyezkedes
                FROM Eszkozok
                ";
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                eszkoz = new List<JsonEszkoz>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.eszkoz.Add(new JsonEszkoz
                    {
                        id = records[0] == db_null ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1],
                        kategoria_id = records[2] == db_null ? -1 : Convert.ToInt64(records[2]),
                        leiras = (string)records[3],
                        elhelyezkedes = (string)records[4]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse listFelhasznalo(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Nev, FelhasznaloNev, KepesitesID, Munkaorak_szama, Szerep
                FROM Felhasznalo
                ";
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                felhasznalo = new List<JsonFelhasznalo>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.felhasznalo.Add(new JsonFelhasznalo
                    {
                        id = records[0] == db_null ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1],
                        username = (string)records[2],
                        kepesites_id = records[3] == db_null ? -1 : Convert.ToInt64(records[3]),
                        munkaorakszama = records[4] == db_null ? -1 : Convert.ToInt64(records[4]),
                        role = (string)records[5]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse listKarbantartas(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, EszkozId, Tipus, Allapot, Sulyossag, Mettol, Meddig
                FROM Karbantartas
                ";
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                karbantartas = new List<JsonKarbantartas>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.karbantartas.Add(new JsonKarbantartas
                    {
                        id = records[0] == db_null ? -1 : Convert.ToInt64(records[0]),
                        eszkoz_id = records[1] == db_null ? -1 : Convert.ToInt64(records[1]),
                        tipus = (string)records[2],
                        allapot = (string)records[3],
                        sulyossag = (string)records[4],
                        mettol = records[5] == db_null ? new DateTime() : Convert.ToDateTime(records[5]),
                        meddig = records[6] == db_null ? new DateTime() : Convert.ToDateTime(records[6])
                    });

                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse listKepesites(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Megnevezes
                FROM Kepesites
                ";
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                kepesites = new List<JsonKepesites>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.kepesites.Add(new JsonKepesites
                    {
                        id = records[0] == db_null ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        JsonCommunicationResponse listMunkaElfogadas(JsonCommunication js) //nincs kész.
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Megnevezes
                FROM Kepesites
                ";
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                kepesites = new List<JsonKepesites>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.kepesites.Add(new JsonKepesites
                    {
                        id = records[0] == db_null ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1]
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
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                INSERT INTO Felhasznalo (Nev, FelhasznaloNev, Jelszo, KepesitesID, Munkaorak_szama, Szerep)
                VALUES( '"
                        + js.name + "', '"
                        + js.username + "', '"
                        + js.password + "', "
                        + getKepesitesIdByName(js.school) + ", "
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

        JsonCommunicationResponse ujKepesites(JsonCommunication js)
        {

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
            dbClose();
            dbOpen();

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
            Int64 id = (Int64)records[0];

            reader.Close();
            dbClose();

            command.CommandText = "INSERT INTO Szerelheti (KategoriaID, KepesitesID) VALUES";
            foreach(var i in js.kategoriaaz)
            {
                command.CommandText += "(" + getKategoriaIdByName(i) + "," + id + "),";
            }
            dbOpen();
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
            while (!connection_opened) ;
            connection_opened = true;
            connection.Open();
            Sassion.swrite("DB kinyit");
        }

        static void dbClose()
        {
            connection.Close();
            connection_opened = false;
            Sassion.swrite("DB zár");
        }

        public static JsonCommunication dataToJson(string data)
        {
            return JsonSerializer.Deserialize<JsonCommunication>(data);
        }

        Int64 getKepesitesIdByName(string data)
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

        Int64 getKategoriaIdByName(string data)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select ID
                FROM Kategoria
                WHERE Megnevezes = '" + data + "'";

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
