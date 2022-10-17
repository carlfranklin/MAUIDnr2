using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace MAUIDnr1.Models
{
    public class Guest
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string Bio { get; set; } = "";
        public string LastError { get; set; } = "";
        public string PhotoPath { get; set; } = "";
    }
}