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
    }

    public class JsonCommunicationResponse
    {
        public string? hash { get; set; }
        public int? state { get; set; }
        public string? name { get; set; }
        public string? role { get; set; }
    }
}
