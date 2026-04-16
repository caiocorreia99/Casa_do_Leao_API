using CDL.Models.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Binder
{
    public class UserResponse
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool Admin { get; set; }
        public int IdRole { get; set; }
        public string Role { get; set; } = UserRoles.Simple;
        public bool Active { get; set; }
    }
}
