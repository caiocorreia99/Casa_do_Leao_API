using CDL.Models.Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Services
{
    public class DbAuthService : IDbAuthService
    {

        public DbAuthService()
        {
            throw new NotImplementedException();
        }

        string IDbAuthService.GetAccessToken()
        {
            throw new NotImplementedException();
        }
    }
}
