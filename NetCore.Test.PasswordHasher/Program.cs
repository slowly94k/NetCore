﻿using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace NetCore.Test.PasswordHasher
{
    class Program
    {
        //Password => GUIDSalt, RNGSalt, PasswordHash
        static void Main(string[] args)
        {
            Console.Write("아이디를 입력하세요: ");
            string userId = Console.ReadLine();

            Console.Write("비밀번호를 입력하세요: ");
            string password = Console.ReadLine();

            string guidSalt = Guid.NewGuid().ToString();

            string rngSalt = GetRNGSalt();

            string passwordHash = GetPasswordHash(userId, password, guidSalt, rngSalt);

            //데이터베이스의 비밀번호정보와 지금 입력한 비밀번호정보를 비교해서 같은 해시값이 나오면 로그인이 되도록 처리
            bool check = CheckThePasswordInfo(userId, password, guidSalt, rngSalt, passwordHash);

            Console.WriteLine($"UserId:{userId}");
            Console.WriteLine($"Password: {password}");
            Console.WriteLine($"GUIDSalt : {guidSalt}");
            Console.WriteLine($"RNGSalt: {rngSalt}");
            Console.WriteLine($"passwordHash: {passwordHash}");
            Console.WriteLine($"check: {(check ? "비밀번호 정보가 일치" : "불일치")}");

            //창이 닫혀서 추가 (16. )
            Console.ReadLine();
        }

        private static string GetRNGSalt()
        {
            // generate a 128-bit salt using a secure PRNG
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }
        private static string GetPasswordHash(string userId, string password, string guidSalt, string rngSalt)
        {
            // derive a 256-bit subkey (use HMACSHA1 with 10,000 iterations)
            //Pbkdf2?
            //Password-based key derivation function 2 / 비밀번호를 암호화 시키는데 사용하기 위한 함수
            /*
            GUIDSalt
            => 사용자 정보의 복잡성을 위해 사용
            RNGSalt
            => 비밀번호해시 생성시 Salt의 복잡성을 위해 사용
            PasswrodHash
            =>Iterations : 10000, 25000, 45000
             */
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: userId + password + guidSalt,
                salt: Encoding.UTF8.GetBytes(rngSalt),
                prf: KeyDerivationPrf.HMACSHA512,

                //iterationCount, numBytesRequested의 값들은 고정적으로 한다
                iterationCount: 45000, //10000, 25000, 45000
                numBytesRequested: 256 / 8));
        }

        private static bool CheckThePasswordInfo(string userId, string password, string guidSalt, string rngSalt, string passwordHash)
        {
            //GetPasswordHash를 통해서 ()의 값의 비밀번호를 만들어 미리 만들어진 passwordHash값과 일치하는지
            //일치 : true,  불일치 : false
            return GetPasswordHash(userId, password, guidSalt, rngSalt).Equals(passwordHash);
        }
    }
}
