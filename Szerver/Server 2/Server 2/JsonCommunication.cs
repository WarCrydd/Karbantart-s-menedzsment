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
        public int? katid { get; set; }
        public string? elhelyezkedes { get; set; }

    }

    public class JsonCommunicationResponse
    {
        public string? hash { get; set; }
        public int? state { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
        public List<JsonKategoria>? kategoria { get; set; }
    }

    public class JsonKategoria
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public int? parent { get; set; }
        public int? normaido { get; set; }
        public int? karbperiod { get; set; }
        public string? leiras { get; set; }

    }
}
