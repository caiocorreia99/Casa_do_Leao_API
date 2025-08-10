using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CDL.Models.Binder
{
    public class LoginRequest
    {
        [JsonPropertyName("Login")]
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
