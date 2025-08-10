using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Services.Interface
{
    public interface IDbAuthService
    {
        string GetAccessToken();
    }
} 
