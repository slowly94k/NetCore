using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.Extensions.DependencyInjection;

//13.
namespace NetCore.Utilites.Utils
{
    public static class Common

    {
        /// <summary>
        /// Data Protection 지정하기
        /// </summary>
        /// <param name="services">등록할 서비스</param>
        /// <param name="keyPath">키 경로</param>
        /// <param name="applicationName">애플리케이션 이름</param>
        /// <param name="cryptoType">암호화 유형</param>
        //메서드 추가
        public static void SetDataProtection(IServiceCollection services, string keyPath, string applicationName, Enum cryptoType)
        {
            //ConfigureServices()메서드에서 DataProtection을 사용하기 위해 서비스로 등록 (12. ) 
            //PersistKeysToFileSystem() : 키를 파일시스템으로 유지 
            //SetDefaultKeyLifetime() : 키 만료기간
            //SetApplicationName() : 어플리케이션 이름 지정
            var builder = services.AddDataProtection()
                    .PersistKeysToFileSystem(new DirectoryInfo(keyPath))
                    .SetDefaultKeyLifetime(TimeSpan.FromDays(7))
                    .SetApplicationName(applicationName);

            switch (cryptoType)
            {
                case Enums.CryptoType.Unmanaged:
                    //builder가 있는 곳에 추가 되는 것(연결)
                    //AES : Advanced Encyption Standard, Two-way방식 : 암호화, 복호화, 256이 보완에 뛰어남
                    //SHA : Secure Hash Algorithm,  One-way방식 : 암호화,  512 숫자가 클수록 보완에 좋다
                    builder.UseCryptographicAlgorithms(
                        new AuthenticatedEncryptorConfiguration() {
                            EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                            ValidationAlgorithm = ValidationAlgorithm.HMACSHA512
                        });
                    break;

                case Enums.CryptoType.Managed:
                    builder.UseCustomCryptographicAlgorithms(
                        new ManagedAuthenticatedEncryptorConfiguration() {
                            // A type that subclasses SymmetricAlgorithm
                            EncryptionAlgorithmType = typeof(Aes),

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // A type that subclasses KeyedHashAlgorithm
                            ValidationAlgorithmType = typeof(HMACSHA512)
                        });
                    break;

                case Enums.CryptoType.CngCbc:
                    //Windows CNG algorithm using CBC-mode encryption
                    //CNG algorithm
                    //Cryptography API : Next Gneration
                    //CBC-mode
                    //Cipher Bolck Chaining
                    builder.UseCustomCryptographicAlgorithms(
                        new CngCbcAuthenticatedEncryptorConfiguration() 
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256,

                            // Passed to BCryptOpenAlgorithmProvider
                            HashAlgorithm = "SHA512",
                            HashAlgorithmProvider = null
                        });
                    break;

                case Enums.CryptoType.CngGcm:
                    //Windows CNG algorithm using Galois/Counter Mode encryption
                    //Galois/Counter Mode
                    //GCM
                    builder.UseCustomCryptographicAlgorithms(
                        new CngGcmAuthenticatedEncryptorConfiguration() 
                        {
                            // Passed to BCryptOpenAlgorithmProvider
                            EncryptionAlgorithm = "AES",
                            EncryptionAlgorithmProvider = null,

                            // Specified in bits
                            EncryptionAlgorithmKeySize = 256
                        });
                    break;

            }
        }
    }
}
