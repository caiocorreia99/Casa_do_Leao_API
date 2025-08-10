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
        public bool Active { get; set; }
    }
}
