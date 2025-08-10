using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDL.Models.Helpers
{
    public class Constants
    {
        public enum InternalCode
        {
            Success = 0,
            Catch_ValidateLogin = 1,
            Catch_Generic = 2,
            TokenAccess = 3
        }
    }
}
