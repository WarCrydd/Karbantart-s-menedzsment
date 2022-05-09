
namespace Json_Classes
{
    public class JsonCommunication
    {
        public Int64 code { get; set; }
        public string? password { get; set; }
        public string? username { get; set; }
        public string? hash { get; set; }
        public string? name { get; set; }
        public string? school { get; set; }
        public string? role { get; set; }
        public Int64? parent { get; set; }
        public Int64? karbantartasid { get; set; }
        public Int64? normaido { get; set; }
        public Int64? ido { get; set; }
        public string? karbperiod { get; set; }
        public string? description { get; set; }
        public string? leiras { get; set; }
        public Int64? kategoriaid { get; set; }
        public Int64? eszkozid { get; set; }
        public Int64? karbantartoid { get; set; }
        public Int64? kepesitesid { get; set; }
        public string? tipus { get; set; }
        public string? allapot { get; set; }
        public string? sulyossag { get; set; }
        public DateTime? date { get; set; }
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
        public string? leiras { get; set; }
        public List<JsonKategoria>? kategoria { get; set; }
        public List<JsonEszkoz>? eszkoz { get; set; }
        public List<JsonFelhasznalo>? felhasznalo { get; set; }
        public List<JsonKarbantartas>? karbantartas { get; set; }
        public List<JsonKepesites>? kepesites { get; set; }
        public List<JsonTask>? tasks { get; set; }
        public List<JsonSzerelheti>? szerelheti { get; set; }
        public List<JsonKepzetsegek>? kepzetsegek { get; set; }

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
        public List<Int64>? szabadorak { get; set; }
    }

    public class JsonKarbantartas
    {
        public Int64? id { get; set; }
        public Int64? eszkoz_id { get; set; }
        public String? allapot { get; set; }
        public string? name { get; set; }
        public string? sulyossag { get; set; }
        public string? helyszin { get; set; }
        public DateTime? date { get; set; }
        public string? leiras { get; set; }
        public Int64? karbantartoid { get; set; }
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

    public class JsonKepzetsegek
    {
        public Int64? karbantarto_id { get; set; }
        public Int64? kepesites_id { get; set; }
    }
}
