using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Binder
{
    public class UserRequest
    {
        public int? IdUser { get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool Admin { get; set; }
        public bool Active { get; set; }

        // Campos adicionais
        public string CPF { get; set; }
        public string DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
    }
}
