using System;
using System.Collections.Generic;

namespace CDL.Models.DataBase
{
    public partial class User
    {
        public int IdUser { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool Active { get; set; } = true;
        /// <summary>Legado: sincronizado com <see cref="Role"/> (admin = true).</summary>
        public bool Admin { get; set; } = false;

        public int IdRole { get; set; }
        public virtual Role Role { get; set; } = null!;

        public string? Token { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }

        public virtual UserFields Fields { get; set; }

        public virtual ICollection<EventRegistration> EventRegistrations { get; set; } = new List<EventRegistration>();
    }
}
