using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class AESInfo
    {
        [Required(ErrorMessage = "사용자 아이디를 입력하세요.")]
        [MinLength(6, ErrorMessage = "사용자 아이디는 6자 이상 입력")]
        [Display(Name = "사용자 아이디")]
        public string UserId { get; set; }

        //[DataType(DataType)] : LoginInfo의 View모델에서 멤버변수 Password를 Password유형의 데이터로 지정하겠다.(13.)
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "비밀버호를 입력하세요.")]
        [MinLength(6, ErrorMessage = "비밀번호는 6자 이상 입력")]
        [Display(Name = "비밀번호")]
        public string Password { get; set; }

        //13. 암호화, 복호화 문자열 추가  > 필수 값이 아니다.
        //암호화문자열은 데이터타입이 길어질 수 있어서 MultilineText
        [DataType(DataType.MultilineText)]
        [Display(Name ="암호화 정보")]
        public string EncUserInfo { get; set; }
        [Display(Name = "복호화 정보")]
        public string DecUserInfo { get; set; }
    }
}
