using Json_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server_2.Sassions
{
    internal class SassionForOperator : Sassion
    {
        Dictionary<Int64?, JsonFelhasznalo?>? felhasznaloList;
        public SassionForOperator(string _hash) : base(_hash)
        {
        }

        public override string solve(string json)
        {
            try
            {
                JsonCommunication? js = JsonSerializer.Deserialize<JsonCommunication>(json);
                string response = "";
                if (!checkHash(js.hash) && js.code != 1)
                {
                    write("Nem jó a hash!!!!");
                }
                else switch (js?.code)
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

                        case 8:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listEszkozok(js), options);
                            break;

                        case 9:
                            JsonCommunicationResponse jsr = listFelhasznalo(js);
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(jsr, options);
                            int i = 0;
                            felhasznaloList = new Dictionary<long?, JsonFelhasznalo?>();
                            foreach(var item in jsr.felhasznalo)
                            {
                                long? id = item.id;
                                string? name = item.name;
                                string? role = item.role;
                                long? munkaorakszama = item.munkaorakszama;
                                long? kepesites_id = item.kepesites_id;
                                List<long>? szabadorak = item.szabadorak;
                                string? username = item.username;
                                string? password = item.password;

                                JsonFelhasznalo f = new JsonFelhasznalo
                                {
                                    id = id ?? 0,
                                    name = name ?? "",
                                    role = role ?? "",
                                    munkaorakszama = munkaorakszama ?? 0,
                                    szabadorak = szabadorak ?? new List<Int64>(),
                                    username = username ?? ""
                                };

                                felhasznaloList.Add(id, f) ;
                                i++;
                            }
                            break;

                        case 10:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listKarbantartas(js), options);
                            break;

                        case 11:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listKepesites(js), options);
                            break;

                        case 12:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listMunkaElfogadas(js), options);
                            break;

                        case 13:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listSzerelheti(js), options);
                            break;

                        case 14:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(ujKarbantartas(js), options);
                            break;

                        case 15:
                            Int64 a = getNormaidoByKategoriaID(getKategoriaIdByEszkozID((long)getKarbantartasByID((long)js.karbantartasid).eszkoz_id));
                            Int64 b = (24 - felhasznaloList[js.karbantartoid].szabadorak.Count());
                            Int64 c = (long)felhasznaloList[js.karbantartoid].munkaorakszama;
                            if (a + b > c)
                            {
                                response = "{\"state\":2}";
                                break;
                            }
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(karbantartoKarbantartashozRendeles(js), options);
                            break;

                        case 20:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(ujKepzetseg(js), options);
                            break;

                        case 22:
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listKepzetsegek(js), options);
                            break;

                        default:
                            Console.WriteLine("Nem ismert kérés");
                            break;
                    }

                return response;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.ResetColor();
                dbClose();
                return "{\"state\":1}";
            }
        }
    }
}
