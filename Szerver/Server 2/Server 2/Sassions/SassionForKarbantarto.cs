using Json_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Server_2.Sassions
{
    internal class SassionForKarbantarto : Sassion
    {
        public SassionForKarbantarto(string _hash) : base(_hash)
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

                    case 10:
                        JsonCommunicationResponse jsr = listKarbantartas(js);
                        List<JsonKarbantartas> karbantartasok = new List<JsonKarbantartas>(jsr.karbantartas);
                        foreach(var i in karbantartasok)
                        {
                            if(i.karbantartoid != this.id)
                            {
                                jsr.karbantartas.Remove(i);
                            }
                        }
                        response = JsonSerializer.Serialize<JsonCommunicationResponse>(jsr, options);
                        break;

                    case 16:
                        js.karbantartoid = id;
                        response = JsonSerializer.Serialize<JsonCommunicationResponse>(karbantartasElfogadasElutasitas(js, true));
                        break;

                    case 17:
                        js.karbantartoid = id;
                        response = JsonSerializer.Serialize<JsonCommunicationResponse>(karbantartasElfogadasElutasitas(js, false));
                        break;

                    case 18:
                        js.karbantartoid = id;
                        response = JsonSerializer.Serialize<JsonCommunicationResponse>(karbantartasElkezdese(js), options);
                        break;

                    case 19:
                        js.karbantartoid = id;
                        response = JsonSerializer.Serialize<JsonCommunicationResponse>(karbantartasBefejezese(js), options);
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
