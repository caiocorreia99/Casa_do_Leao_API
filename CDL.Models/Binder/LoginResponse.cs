using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Binder
{
    public class LoginResponse
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; } = true;
        public bool Admin { get; set; } = false;
        public string? Token { get; set; }

    }
}
