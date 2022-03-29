using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    }

    public class JsonCommunicationResponse
    {
        public string? hash { get; set; }
        public Int64? state { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
        public List<JsonKategoria>? kategoria { get; set; }
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
        public int? id { get; set; }
        public string? name { get; set; }
        public int? kategoriaid { get; set; }
        public string? leiras { get; set; }
        public string? elhelyezkedes { get; set; }
    }

    public class JsonFelhasznalo
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
        public string? username { get; set; }
        public string? password { get; set; }
        public string? kepesitesid { get; set; }
        public int? munkaorakszama { get; set; }
    }

    public class JsonKarbantartas
    {
        public int? id { get; set; }
        public int? eszkozid { get; set; }
        public string? tipus { get; set; }
        public string? allapot { get; set; }
        public string? sulyossag { get; set; }
        public DateTime? mettol { get; set; }
        public DateTime? meddig { get; set; }
    }

    public class JsonVegzetseg
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public List<int> kategoriak { get; set; }
    }
}
