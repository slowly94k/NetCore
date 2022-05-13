using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Utilities.Utils
{
    public class Enums
    {
        /// <summary>
        /// 암호화 유형
        /// </summary>
        public enum CryptoType
        {
            //관리되지 않는 방식
            Unmanaged = 1,

            //관리되는 방식
            Managed = 2,

            //Cng알고리즘 인데 Cbc모드의 인증을 사용   
            CngCbc = 3,

            //Cng알고리즘 인데 Gcm모드의 인증을 사용 
            CngGcm = 4
        }
    }
}
