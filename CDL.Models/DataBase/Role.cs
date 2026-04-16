using System.Collections.Generic;

namespace CDL.Models.DataBase
{
    /// <summary>Perfis fixos: Usuario (simple), Editor, Administrador (admin). <see cref="Code"/> alinha com JWT e <see cref="UserRoles"/>.</summary>
    public class Role
    {
        public int IdRole { get; set; }

        /// <summary>Nome exibível (ex.: Usuario, Administrador).</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Código estável: simple | editor | admin</summary>
        public string Code { get; set; } = string.Empty;

        public ICollection<User> Users { get; set; } = new List<User>();
    }
}
