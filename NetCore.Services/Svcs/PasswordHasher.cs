using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;

namespace NetCore.Services.Svcs
{
    public class PasswordHasher : IPasswordHasher
    {
        //데이터베이스에서 불러 올려면 의존성으로
        private DBFirstDbContext _context;

        //생성자 만들기
        public PasswordHasher(DBFirstDbContext context)
        {
            _context = context;
        }

        #region  private methods
        private string GetGUIDSalt()
        {
            return Guid.NewGuid().ToString();
        }
        

        //NetCore.Test.PasswordHasher > Program.cs에서 3가지 복사 16.
        private string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }
        private string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            //Pbkdf2?
            //Password-based key derivation function 2 / 비밀번호를 암호화 시키는데 사용하기 위한 함수
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId + password + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,

                //iterationCount, numBytesRequested의 값들은 고정적으로 한다
                iterationCount: 45000, //10000, 25000, 45000
                numBytesRequested: 256 / 8));
        }

        private bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            //GetPasswordHash를 통해서 ()의 값의 비밀번호를 만들어 미리 만들어진 passwordHash값과 일치하는지
            //일치 : true,  불일치 : false
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
        }


        #endregion
        string IPasswordHasher.GetGUIDSalt()
        {
            return GetGUIDSalt();
        }

        string IPasswordHasher.GetRNGSalt()
        {
            return GetRNGSalt();
        }

        string IPasswordHasher.GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            return GetPasswordHash(userId, password, guidSalt, rngSalt);
        }

        //실질적으로 사용자가 보여질 때는 아이디와 비번만 입력받아서 
        bool IPasswordHasher.MachTheUserInfo(string userId, string password)
        {
            //안쪽에선 데이터베이스로 불러오는 것
            var user = _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
            string guidSalt = user.GUIDSalt;
            string rngSalt = user.RNGSalt;
            string passwordHash = user.PasswordHash;

            return CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash);
        }

    }
}
