using NetCore.Data.Classes;
using NetCore.Data.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Interfaces
{
    public interface IUser
    {
        //실제 사용할 서비스메서드 선언
        //MembershipController 의 Login Action메서드에 사용하는 Logininfo View모델을 사용
        //함수를 실질적으로 사용하기 위해 UserService 상속 입력
        bool MatchTheUserInfo(LoginInfo login);

        //UserService.cs에서 가져와서 연결 (14. )
        User GetUserInfo(string userId);

        //UserService.cs 권한부분 가져옴 (14. )
        //이름부분은 수정. GetRolesOwnedbyUser : 사용자 소유의 권한들을 가져오겠다
        IEnumerable<UserRolesByUser> GetRolesOwnedByUser(string userId);
    }
}
