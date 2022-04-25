using Json_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server_2.Sassions
{
    internal class SassionForEszkozfelelos : Sassion
    {
        public SassionForEszkozfelelos(string _hash) : base(_hash)
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
                            response = JsonSerializer.Serialize<JsonCommunicationResponse>(listFelhasznalo(js), options);
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
