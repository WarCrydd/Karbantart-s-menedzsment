using System.Data;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Json_Classes;

namespace Server_2.Sassions
{
    public abstract class Sassion
    {
        #region variables
        public static Dictionary<string, Sassion> sassions = new Dictionary<string, Sassion>();
        public const int BufferSize = 10000;
        public const int session_timout = 300000000;
        public bool live = false;

        SqliteConnection connection = new SqliteConnection("Data Source=karbantartas-menedzsment.db");
        
        protected static JsonSerializerOptions options = new JsonSerializerOptions
        {
            IgnoreNullValues = true
        };

        public string name = "";
        protected Int64 id = 0;
        protected bool bejelentkezve = false;
        string mhash = "Nincs még érték!! Ezt nem lenne szabad látni!!";
        StringBuilder logsb = new StringBuilder();
        DateTime? karbantartas_update = null;
        bool update_need = false;
        #endregion

        public Sassion(string _hash)
        {
            mhash = _hash;
            live = true;
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("New Sassion started! \t -->" + mhash);
            Console.ResetColor();
        }

        ~Sassion()
        {
            sassions.Remove(mhash);
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
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine( "(" + mhash + ") Sassion: " + ms);
            Console.ResetColor();
        }

        public void log(string ms)
        {
            logsb.AppendLine(ms);
#if MYDEBUG
            write(ms);
#endif
        }

        public string getLog()
        {
            return logsb.ToString();
        }

        public static void swrite(string ms)
        {
#if MY_DEBUG
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(">>>>>>>>    " + "Kóbor áram :D" + "    <<<<<<<<");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(ms);
            Console.ResetColor();
#endif
        }

        public abstract string solve(string json);

        static string RandomHash()
        {
            string new_hash = Guid.NewGuid().ToString("N");
            while(sassions.ContainsKey(new_hash))
            {
                new_hash = Guid.NewGuid().ToString("N");
            }
            return new_hash;
        }

        protected bool checkHash(string? hash)
        {
            return mhash == hash;
        }

        public static JsonCommunication dataToJson(string data)
        {
            return JsonSerializer.Deserialize<JsonCommunication>(data);
        }

        public static string? createOrGetSassion(string js_s)
        {
            JsonCommunication js = dataToJson(js_s);
            JsonCommunicationResponse jsr;
            if (js.hash != null)
            {
                if (sassions.ContainsKey(js.hash))
                {
                    return js.hash;
                }
            }

            SqliteConnection connection = new SqliteConnection("Data Source=karbantartas-menedzsment.db");
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT Nev, Szerep ,ID
                FROM Felhasznalo
                WHERE FelhasznaloNev =  '" + js.username + "' " +
                "AND Jelszo ='" + js.password + "'";

            connection.Open();
            Int64 f_id = 0;
            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    jsr = new JsonCommunicationResponse
                    {
                        state = 0,
                        name = (string)record[0],
                        role = (string)record[1]
                    };
                    f_id = (Int64)record[2];
                }
                else
                {
                    return null;
                }
            }
            connection.Close();

            foreach (var item in sassions) //már bejelentkezett felhasználók szűrése
            {
                if (item.Value.id == f_id && item.Value.live)
                {
                    return null;
                }
            }

            string new_hash = RandomHash();
            switch (jsr.role)
            {
                case "admin":
                    sassions.Add(new_hash, new SassionForAdmin(new_hash));
                    break;

                case "operator":
                    sassions.Add(new_hash, new SassionForOperator(new_hash));
                    break;

                case "eszkozfelelos":
                    sassions.Add(new_hash, new SassionForEszkozfelelos(new_hash));
                    break;

                case "karbantarto":
                    sassions.Add(new_hash, new SassionForKarbantarto(new_hash));
                    break;

                case "hibabejelento":
                    sassions.Add(new_hash, new SassionForHibabejelento(new_hash));
                    break;

                default:
                    return null;
            }

            return new_hash;
        }

        protected JsonCommunicationResponse belepes(JsonCommunication js)
        {
            live = true;
            JsonCommunicationResponse jsr;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT Nev, Szerep, ID
                FROM Felhasznalo
                WHERE FelhasznaloNev =  '" + js.username + "' " +
                "AND Jelszo ='" + js.password + "'";

            using (var reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    jsr = new JsonCommunicationResponse
                    {
                        hash = mhash,
                        state = 0,
                        name = (string)record[0],
                        role = (string)record[1]
                    };
                    id = (Int64)record[2]; 
                    dbClose();
                    name = jsr.name;
                    return jsr;
                }
                write("Nincs a kért Felhasználó a rendszerben.");
            }
            dbClose();
            jsr = new JsonCommunicationResponse
            {
                state = 1
            };
            
            return jsr;
        }

        protected JsonCommunicationResponse kilepes(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            live = false;
            return jsr;
        }

        #region List_comunication
        protected JsonCommunicationResponse listKategorioa(JsonCommunication js)
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
                        id = records[0] == DBNull.Value ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1],
                        parent = records[2] == DBNull.Value ? -1 : Convert.ToInt64(records[2]),
                        normaido = records[3] == DBNull.Value ? -1 : Convert.ToInt64(records[3]),
                        karbperiod = records[4] == DBNull.Value ? -1 : Convert.ToInt64(records[4]),
                        leiras = (string)records[5]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        protected JsonCommunicationResponse listEszkozok(JsonCommunication js)
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
                        id = records[0] == DBNull.Value ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1],
                        kategoria_id = records[2] == DBNull.Value ? -1 : Convert.ToInt64(records[2]),
                        leiras = (string)records[3],
                        elhelyezkedes = (string)records[4]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        protected JsonCommunicationResponse listFelhasznalo(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, Nev, FelhasznaloNev, Munkaorak_szama, Szerep
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
                        id = records[0] == DBNull.Value ? -1 : Convert.ToInt64(records[0]),
                        name = records[1] == DBNull.Value ? "" : (string)records[1],
                        username = records[2] == DBNull.Value ? "" : (string)records[2],
                        munkaorakszama = records[3] == DBNull.Value ? -1 : Convert.ToInt64(records[3]),
                        role = records[4] == DBNull.Value ? "" : (string)records[4]
                    });

                }
            }
            dbClose();

            if(js.eszkozid != null && js.eszkozid != -1)
            {
                for(int i = 0;i < jsr.felhasznalo.Count;i++)
                {
                    bool szerelheti = false;
                    List<Int64> seged = getKepesitesIdByKategoriaID(getKategoriaIdByEszkozID((long)js.eszkozid));
                    foreach(var j in seged)
                    {
                        foreach(var k in getKepesitesIdByFelhasznaloID((long)jsr.felhasznalo[i].id))
                        {
                            if(j == k)
                            {
                                szerelheti = true;
                            }
                        }
                    }

                    if (!szerelheti)
                    {
                        jsr.felhasznalo.Remove(jsr.felhasznalo[i]);
                        i--;
                    }
                }
            }

            if(js.karbantartasid != null && js.karbantartasid >= 0)
            {
                foreach(var felhasznalo in jsr.felhasznalo)
                {
                    List<Int64> szorak = new List<long> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24 };
                    DateTime date = getKarbantartasByID((long)js.karbantartasid).date.Value;
                    JsonCommunicationResponse karbantartasok = listKarbantartas(new JsonCommunication { });
                    foreach(var j in karbantartasok.karbantartas)
                    {
                        if(date.AddDays(-1) <= j.date.Value && j.date.Value <= date.AddDays(1) && j.karbantartoid == felhasznalo.id)
                        {
                            int normaido = (int)getNormaidoByKategoriaID(getKategoriaIdByEszkozID((long)j.eszkoz_id));
                            for (int i = 0; i < normaido; i++)
                            {
                                szorak.Remove(j.date.Value.Hour + i);
                            }
                        }
                    }
                    felhasznalo.szabadorak = szorak;
                }
            }
            else if(js.karbantartasid == -2)
            {
                List<JsonFelhasznalo> felhasznalok = new List<JsonFelhasznalo>(jsr.felhasznalo);
                foreach(var i in felhasznalok)
                {
                    if(i.role != "karbantarto")
                    {
                        jsr.felhasznalo.Remove(i);
                    }
                }
            }

            return jsr;
        }

        protected JsonCommunicationResponse listKarbantartas(JsonCommunication js, bool blank = false)
        {
            if(!blank)
            {
                checkKarbantartas();
            }
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT e.Megnevezes, k.Sulyossag, e.Elhelyezkedes, e.ID, k.ID, k.Allapot, k.KarbantartoID, k.Date, k.Leiras
                FROM Eszkozok e
                INNER JOIN Karbantartas k
                ON k.EszkozID = e.ID
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
                        name = records[0] == DBNull.Value ? "" : Convert.ToString(records[0]),
                        sulyossag = records[1] == DBNull.Value ? "" : Convert.ToString(records[1]),
                        helyszin = records[2] == DBNull.Value ? "" : Convert.ToString(records[2]),
                        eszkoz_id = records[3] == DBNull.Value ? -1 : Convert.ToInt64(records[3]),
                        id = records[4] == DBNull.Value ? -1 : Convert.ToInt64(records[4]),
                        allapot = records[5] == DBNull.Value ? "" : Convert.ToString(records[5]),
                        karbantartoid = records[6] == DBNull.Value ? -1 : Convert.ToInt64(records[6]),
                        date = records[7] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(records[7]),
                        leiras = records[8] == DBNull.Value ? "" : Convert.ToString(records[8])
                    });

                }
            }
            dbClose();

            if(!blank)
            {
                for (int i = 0; i < jsr.karbantartas.Count; i++)
                {
                    if (jsr.karbantartas[i].date < DateTime.Now.AddDays(-1))
                    {
                        jsr.karbantartas.Remove(jsr.karbantartas[i]);
                        i--;
                    }
                }
            }

            if(js.karbantartoid != null && js.karbantartoid != -1)
            {
                List<JsonKarbantartas> karbantartasok = new List<JsonKarbantartas>(jsr.karbantartas);
                foreach(var i in karbantartasok)
                {
                    if(i.karbantartoid != js.karbantartoid)
                    {
                        jsr.karbantartas.Remove(i);
                    }
                }
            }

            return jsr;
        }

        protected JsonCommunicationResponse listKepesites(JsonCommunication js)
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
                        id = records[0] == DBNull.Value ? -1 : Convert.ToInt64(records[0]),
                        name = (string)records[1]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        protected JsonCommunicationResponse listMunkaElfogadas(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT ID, KarbantartasID, KarbantartoID, Allapot, Indoklas
                FROM MunkaElfogadas
                ";

            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                tasks = new List<JsonTask>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.tasks.Add(new JsonTask
                    {
                        id = records[0] == DBNull.Value ? -1 : Convert.ToInt64(records[0]),
                        karbantartas_id = records[1] == DBNull.Value ? -1 : Convert.ToInt64(records[1]),
                        karbantarto_id = records[2] == DBNull.Value ? -1 : Convert.ToInt64(records[2]),
                        allapot = (string)records[3],
                        indoklas = (string)records[4]
                    });

                }
            }
            dbClose();

            return jsr;
        }

        protected JsonCommunicationResponse listSzerelheti(JsonCommunication js)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT KategoriaID, KepesitesID
                FROM Szerelheti
                ";

            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                szerelheti = new List<JsonSzerelheti>()
            };
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsr.szerelheti.Add(new JsonSzerelheti
                    {
                        kategoria_id = records[0] == DBNull.Value ? -1 : Convert.ToInt64(records[0]),
                        kepesites_id = records[1] == DBNull.Value ? -1 : Convert.ToInt64(records[1])
                    });

                }
            }
            dbClose();

            return jsr;
        }

        protected JsonCommunicationResponse listKepzetsegek(JsonCommunication js)
        {
            Int64 id = getFelhasznaloIdByName(js.username);

            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT KepesitesID
                FROM Kepzetsegek
                Where KarbantartoID = " + id + "";

            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
                kepesites = new List<JsonKepesites>()
            };

            List<Int64> ids = new List<Int64>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    ids.Add(Convert.ToInt64(records[0]));
                }
            }
            dbClose();

            foreach (var _id in ids)
            {
                jsr.kepesites.Add(new JsonKepesites
                {
                    id = _id,
                    name = getKepesitesNameByID(Convert.ToInt64(_id)),
                });
            }

            return jsr;
        }
        #endregion

        #region New_communication
        protected JsonCommunicationResponse ujKat(JsonCommunication js)
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
                INSERT INTO Kategoria (Megnevezes, SzuloKategoriaID, NormaIdo, Karb_periodus, Instrukciok)
                VALUES( '"
                        + js.name + "', "
                        + getKategoriaIdByName(js.parent) + ", "
                        + js.normaido + ", "
                        + js.karbperiod + ", '"
                        + js.leiras + "')";
                //write(command.CommandText);
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

        protected JsonCommunicationResponse ujFelhasznalo(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            string role = "";
            switch (js.role)
            {
                case "Hiba bejelentő":
                    role = "hibabejelento";
                    break;
                case "Karbantartó":
                    role = "karbantarto";
                    break;
                case "Operátor":
                    role = "operator";
                    break;
                case "Eszközfelelős":
                    role = "eszkozfelelos";
                    break;
                default:
                    role = js.role;
                    break;
            }
            try
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                INSERT INTO Felhasznalo (Nev, FelhasznaloNev, Jelszo, Munkaorak_szama, Szerep)
                VALUES( '"
                        + js.name + "', '"
                        + js.username + "', '"
                        + js.password + "', "
                        + "8" + ", '"
                        + role + "')";

                //write(command.CommandText);
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

        protected JsonCommunicationResponse ujEszkoz(JsonCommunication js)
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
            //write(command.CommandText);
            if (command.ExecuteNonQuery() == -1)
            {
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }
            dbClose();

            ujKarbantartas(new JsonCommunication
            {
                eszkozid = getEszkozID(js.name, js.kategoriaid, js.leiras, js.elhelyezkedes),
                date = DateTime.Now.AddMonths((int)getKategoriaPeriodusIdByID((long)js.kategoriaid)),
                leiras = ""
            }, "Idoszakos");

            return jsr;
        }

        protected JsonCommunicationResponse ujKepesites(JsonCommunication js)
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
            //write(command.CommandText);
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
            foreach (var i in js.kategoriaaz)
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

        protected JsonCommunicationResponse ujKarbantartas(JsonCommunication js, string sulyossag = "Kritikus")
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO Karbantartas (EszkozID, Allapot, Sulyossag, Date, Leiras)
                VALUES( "
                    + js.eszkozid + ", '"
                    + "" + "', '"
                    + sulyossag + "', '"
                    + js.date + "', '"
                    + js.leiras + "'"
                    + ")";
            //write(command.CommandText);
            if (command.ExecuteNonQuery() == -1)
            {
                dbClose();
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
            Int64 kar_id = (Int64)records[0];

            reader.Close();
            dbClose();

            createKarbantartasLog(kar_id, this.id, "Created");

            return jsr;
        }

        protected JsonCommunicationResponse ujKepzetseg(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0
            };
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                INSERT INTO Kepzetsegek (KarbantartoID, KepesitesID)
                VALUES( "
                    + getFelhasznaloIdByName(js.username) + ", "
                    + getKepesitesIdByName(js.name)
                    + ")";
            //write(command.CommandText);
            dbOpen();
            if (command.ExecuteNonQuery() == -1)
            {
                dbClose();
                return new JsonCommunicationResponse
                {
                    state = 1
                };
            }
            dbClose();
            return jsr;
        }
        #endregion

        #region Update_communication

        protected JsonCommunicationResponse karbantartoKarbantartashozRendeles(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
            };

            var command = connection.CreateCommand();
            DateTime date = getKarbantartasByID((long)js.karbantartasid).date.Value;
            command.CommandText =
                @"
                UPDATE Karbantartas
                SET Allapot = 'Ütemezve', KarbantartoID = " + js.karbantartoid + ", Date = '" + date.AddHours(-1 * date.Hour).AddHours((double)js.ido) +
                "' WHERE ID = " + js.karbantartasid;
            
            dbOpen();
            try
            {
                using (var reader = command.ExecuteReader())
                {

                }
            }
            catch (Exception ex)
            {
                write("Nem jóóó" + ex.Message);
                jsr = new JsonCommunicationResponse
                {
                    state = 1,
                };
            }
            dbClose();

            createKarbantartasLog((long)js.karbantartasid, id, "Ütemezve");

            return jsr;
        }

        protected JsonCommunicationResponse karbantartasElfogadasElutasitas(JsonCommunication js, bool accept)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
            };
            if (getKarbantartasByID((long)js.id).allapot != "Ütemezve")
            {
                jsr.state = 1;
                return jsr;
            }
            var command = connection.CreateCommand();
            DateTime date = getKarbantartasByID((long)js.id).date.Value;
            command.CommandText =
                @"
                UPDATE Karbantartas
                SET Allapot = '" + (accept ? "Elfogadva" : "Elutasítva") +
                "' WHERE KarbantartoID = " + js.karbantartoid + " AND ID = " + js.id;

            dbOpen();
            try
            {
                using (var reader = command.ExecuteReader())
                {

                }
            }
            catch (Exception ex)
            {
                write("Nem jóóó" + ex.Message);
                jsr = new JsonCommunicationResponse
                {
                    state = 1,
                };
            }
            dbClose();

            createKarbantartasLog((long)js.id, id, (accept ? "Elfogadva" : "Elutasítva"));

            return jsr;
        }

        protected JsonCommunicationResponse karbantartasElkezdese(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
            };
            if (getKarbantartasByID((long)js.id).allapot != "Elfogadva")
            {
                jsr.state = 1;
                return jsr;
            }
            var command = connection.CreateCommand();
            DateTime date = getKarbantartasByID((long)js.id).date.Value;
            command.CommandText =
                @"
                UPDATE Karbantartas
                SET Allapot = 'Megkezdve'
                WHERE KarbantartoID = " + js.karbantartoid + " AND ID = " + js.id;

            dbOpen();
            try
            {
                using (var reader = command.ExecuteReader())
                {

                }
            }
            catch (Exception ex)
            {
                write("Nem jóóó" + ex.Message);
                jsr = new JsonCommunicationResponse
                {
                    state = 1,
                };
            }
            dbClose();
            jsr.leiras = getKategoriaByID(getKategoriaIdByEszkozID((long)getKarbantartasByID((long)js.id).eszkoz_id)).leiras;

            createKarbantartasLog((long)js.id, id, "Elkezdve");

            return jsr;
        }

        protected JsonCommunicationResponse karbantartasBefejezese(JsonCommunication js)
        {
            JsonCommunicationResponse jsr = new JsonCommunicationResponse
            {
                state = 0,
            };
            if (getKarbantartasByID((long)js.id).allapot != "Megkezdve")
            {
                jsr.state = 1;
                return jsr;
            }
            var command = connection.CreateCommand();
            DateTime date = getKarbantartasByID((long)js.id).date.Value;
            command.CommandText =
                @"
                UPDATE Karbantartas
                SET Allapot = 'Befejezve'
                WHERE KarbantartoID = " + js.karbantartoid + " AND ID = " + js.id;

            dbOpen();
            try
            {
                using (var reader = command.ExecuteReader())
                {

                }
            }
            catch (Exception ex)
            {
                write("Nem jóóó" + ex.Message);
                jsr = new JsonCommunicationResponse
                {
                    state = 1,
                };
            }
            dbClose();

            createKarbantartasLog((long)js.id, id, "Befejezve");

            return jsr;
        }

        #endregion

        #region Inner_Methods
        protected Int64 getEszkozID(string name, Int64? kategoriaid, string leiras, string elhelyezkedes)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select ID
                FROM Eszkozok
                WHERE Megnevezes = '" + name + "' AND KategoriaID = " + kategoriaid + " AND Leiras = '" + leiras + "' AND Elhelyezkedes = '" + elhelyezkedes +"'";

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

        protected Int64 getKepesitesIdByName(string data)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select ID
                FROM Kepesites
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

        protected string getKepesitesNameByID(Int64 data)
        {
            string result = "";
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select Megnevezes
                FROM Kepesites
                WHERE ID = " + data + "";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    result = (string)record[0];
                }
            }

            dbClose();
            return result;
        }

        protected Int64 getFelhasznaloIdByName(string data)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select ID
                FROM Felhasznalo
                WHERE FelhasznaloNev = '" + data + "'";

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

        protected Int64 getKategoriaIdByName(string data)
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

        protected Int64 getKategoriaIdByEszkozID(Int64 data)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select KategoriaID
                FROM Eszkozok
                WHERE ID = '" + data + "'";

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

        protected Int64 getKategoriaPeriodusIdByID(Int64 data)
        {
            Int64 result = -1;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select Karb_periodus
                FROM Kategoria
                WHERE ID = '" + data + "'";

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

        protected JsonKarbantartas getKarbantartasByID(Int64 data)
        {
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT e.Megnevezes, k.Sulyossag, e.Elhelyezkedes, e.ID, k.ID, k.Allapot, k.KarbantartoID, k.Date, k.Leiras
                FROM Eszkozok e
                INNER JOIN Karbantartas k
                ON k.EszkozID = e.ID WHERE k.ID = " + data;

            JsonKarbantartas jsonKarbantartas = new JsonKarbantartas();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;

                    jsonKarbantartas =  new JsonKarbantartas
                    {
                        name = records[0] == DBNull.Value ? "" : Convert.ToString(records[0]),
                        sulyossag = records[1] == DBNull.Value ? "" : Convert.ToString(records[1]),
                        helyszin = records[2] == DBNull.Value ? "" : Convert.ToString(records[2]),
                        eszkoz_id = records[3] == DBNull.Value ? -1 : Convert.ToInt64(records[3]),
                        id = records[4] == DBNull.Value ? -1 : Convert.ToInt64(records[4]),
                        allapot = records[5] == DBNull.Value ? "" : Convert.ToString(records[5]),
                        karbantartoid = records[6] == DBNull.Value ? -1 : Convert.ToInt64(records[6]),
                        date = records[7] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(records[7]),
                        leiras = records[8] == DBNull.Value ? "" : Convert.ToString(records[8])
                    };

                }
            }
            dbClose();

            return jsonKarbantartas;
        }

        protected JsonKategoria getKategoriaByID(Int64 data)
        {
            Int64 _normaido = getNormaidoByKategoriaID(data);

            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                SELECT Megnevezes, SzuloKategoriaID, Karb_periodus, Instrukciok
                FROM Kategoria 
                WHERE ID = " + data;

            JsonKategoria kategoria = new JsonKategoria();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord records = (IDataRecord)reader;
                    kategoria = new JsonKategoria
                    {
                        name = records[0] == DBNull.Value ? "" : Convert.ToString(records[0]),
                        parent = records[1] == DBNull.Value ? -1 : Convert.ToInt64(records[1]),
                        normaido = _normaido,
                        karbperiod = records[2] == DBNull.Value ? -1 : Convert.ToInt64(records[2]),
                        leiras = records[3] == DBNull.Value ? "" : Convert.ToString(records[3]),
                    };
                }
            }
            dbClose();

            return kategoria;
        }

        protected List<Int64> getKepesitesIdByKategoriaID(Int64 data)
        {
            List<Int64> result = new List<Int64>();
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select KepesitesID
                FROM Szerelheti
                WHERE KategoriaID = " + data + "";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    result.Add((Int64)record[0]);
                }
            }

            dbClose();
            return result;
        }

        protected List<Int64> getKepesitesIdByFelhasznaloID(Int64 data)
        {
            List<Int64> result = new List<Int64>();
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select KepesitesID
                FROM Kepzetsegek
                WHERE KarbantartoID = " + data + "";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    result.Add((Int64)record[0]);
                }
            }

            dbClose();
            return result;
        }

        protected Int64 getNormaidoByKategoriaID(Int64 data)
        {

            Int64 result = 0;
            dbOpen();
            var command = connection.CreateCommand();
            command.CommandText =
                @"
                Select Normaido, SzuloKategoriaID
                FROM Kategoria
                WHERE ID = " + data + "";

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    IDataRecord record = (IDataRecord)reader;
                    if (record[0] == DBNull.Value)
                    {
                        result = getNormaidoByKategoriaID((Int64)record[1]);
                    }
                    else
                    {
                        result = (Int64)record[0];
                    }
                }
            }

            dbClose();
            return result;
        }

        bool createKarbantartasLog(Int64 k_id, Int64 f_id, string allapot, string indoklas = "")
        {
            dbOpen();
            try
            {
                var command = connection.CreateCommand();
                command.CommandText =
                    @"
                INSERT INTO MunkaLog (KarbantartasID, FelhasznaloID, Allapot, Indoklas, Date)
                VALUES( " + k_id + ", " + f_id + ", '" + allapot + "', '" + indoklas + "', '" + DateTime.Now +"')";
                //write(command.CommandText);
                if (command.ExecuteNonQuery() == -1)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            dbClose();
            return true;
        }

        void checkKarbantartas()
        {
            DateTime _dateTime = DateTime.Now;
            if (karbantartas_update != null || update_need)
            {
                if(_dateTime.Hour <= karbantartas_update.Value.Hour || _dateTime.DayOfYear <= karbantartas_update.Value.DayOfYear || _dateTime.Year <= karbantartas_update.Value.Year)
                {
                    return;
                }
            }

            JsonCommunicationResponse karbantartasok = listKarbantartas(new JsonCommunication { }, true);

            foreach(var karbantartas in karbantartasok.karbantartas)
            {
                
                if (karbantartas.sulyossag == "Idoszakos")
                {
                    if (_dateTime > karbantartas.date)
                    {
                        DateTime? _date = karbantartas.date.Value.AddMonths((int)getKategoriaPeriodusIdByID(getKategoriaIdByEszkozID((long)karbantartas.eszkoz_id)));
                        bool a = false;
                        foreach(var _k in karbantartasok.karbantartas)
                        {
                            if(_k.date == _date && _k.eszkoz_id == karbantartas.eszkoz_id)
                            {
                                a = true;
                            }
                        }
                        if(a)
                        {
                            continue;
                        }
                        ujKarbantartas(new JsonCommunication 
                        {
                            eszkozid = karbantartas.eszkoz_id,
                            date = _date,
                            leiras = karbantartas.leiras
                        },"Idoszakos");
                    }
                }
            }

        }

        void dbOpen()
        {
            //while (connection_opened) ;

            connection.Open();
            Sassion.swrite("DB kinyit");
        }

        protected void dbClose()
        {
            connection.Close();
            Sassion.swrite("DB zár");
        }
#endregion
    }
}
