using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Interfaces
{
    public interface IPasswordHasher
    {
        string GetGUIDSalt();
        string GetRNGSalt();  
        string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt);
        bool MachTheUserInfo(string userId, string password);



    }
}
