﻿
namespace Server_2
{
    public class JsonCommunication
    {
        public int code { get; set; }
        public string? password { get; set; }
        public string? username { get; set; }
        public string? hash { get; set; }
        public string? name { get; set; }
        public string? school { get; set; }
        public string? role { get; set; }
        public int? parent { get; set; }
        public int? normaido { get; set; }
        public string? karbperiod { get; set; }
        public string? leiras { get; set; }
        public int? kategoriaid { get; set; }
        public string? elhelyezkedes { get; set; }
        public Int64? id { get; set; }
        public List<string>? kategoriaaz { get; set; }

    }

    public class JsonCommunicationResponse
    {
        public string? hash { get; set; }
        public Int64? state { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
        public List<JsonKategoria>? kategoria { get; set; }
        public List<JsonEszkoz>? eszkoz { get; set; }
        public List<JsonFelhasznalo>? felhasznalo { get; set; }
        public List<JsonKarbantartas>? karbantartas { get; set; }
        public List<JsonKepesites>? kepesites { get; set; }
        public List<JsonTask>? tasks { get; set; }
        public List<JsonSzerelheti>? szerelhetis { get; set; }

    }

    public class JsonKategoria
    {
        public Int64? id { get; set; }
        public string? name { get; set; }
        public Int64? parent { get; set; }
        public Int64? normaido { get; set; }
        public Int64? karbperiod { get; set; }
        public string? leiras { get; set; }
    }

    public class JsonEszkoz
    {
        public Int64? id { get; set; }
        public string? name { get; set; }
        public Int64? kategoria_id { get; set; }
        public string? leiras { get; set; }
        public string? elhelyezkedes { get; set; }
    }

    public class JsonFelhasznalo
    {
        public Int64? id { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public Int64? kepesites_id { get; set; }
        public Int64? munkaorakszama { get; set; }
    }

    public class JsonKarbantartas
    {
        public Int64? id { get; set; }
        public Int64? eszkoz_id { get; set; }
        public string? tipus { get; set; }
        public string? allapot { get; set; }
        public string? sulyossag { get; set; }
        public DateTime? mettol { get; set; }
        public DateTime? meddig { get; set; }
    }

    public class JsonKepesites
    {
        public Int64? id { get; set; }
        public string? name { get; set; }
    }

    public class JsonTask
    {
        public Int64? id { get; set; }
        public Int64? karbantartas_id { get; set; }
        public Int64? karbantarto_id { get; set; }
        public string? allapot { get; set; }
        public string? indoklas { get; set; }
    }

    public class JsonSzerelheti
    {
        public Int64? kategoria_id { get; set; }
        public Int64? kepesites_id { get; set; }
    }
}
