using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Data.ViewModels
{
    public class LoginInfo
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

        //내정보 기억 체크박스 생성 (14.) await 부분에 IsPersistent
        [Display(Name = "내정보 기억")]
        public bool RememberMe { get; set; }
    }
}