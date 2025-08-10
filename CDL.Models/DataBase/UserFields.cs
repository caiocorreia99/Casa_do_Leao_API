using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.DataBase
{
    public class UserFields
    {
        public int IdUserFields { get; set; }
        public string CPF { get; set; }
        public string DataNascimento { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public string CEP { get; set; }
        public string Cidade { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Chave estrangeira
        public int IdUser { get; set; }
        public virtual User User { get; set; }
    }
}
