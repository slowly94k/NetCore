using Microsoft.EntityFrameworkCore;
using NetCore.Data.Classes;
//using NetCore.Data.DataModels;
using NetCore.Data.ViewModels;
using NetCore.Services.Data;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Services.Svcs
{
    public class UserService : IUser
    {
        
        //의존성 주입 추가 (9.)
        private DBFirstDbContext _context;
        
        public UserService(DBFirstDbContext context)
        {
            _context = context;
        }


        /*
            private IEnumerable<User> GetUserInfos(){}
            User정보를 리스트로 받아오는 메서드를 생성 (4.의존성 주입)
            User정보를 받기 위해서 받으려는 프로젝트 > 종속성 > 프로젝트 참조 > 받으려는 프로젝트 클릭
        */
        #region priavate
        private IEnumerable<User> GetUserInfos()
        {
            //추가 (9.)
            return _context.Users.ToList();
            //return new List<User>()
            //{
            //    new User()
            //    {
            //        UserId = "jadejs",
            //        UserName = "김정수",
            //        UserEmail ="jadejskim@gmail.com",
            //        Password = "123456"
            //    }
            //};
        }

        //From-sql위해 추가 11.
        private User GetUserInfo(string userId, string password)
        {
            User user;

            //Lambda(9. 1먼저했다가)
            //user = _context.Users.Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).FirstOrDefault();


            //FromSql(9~12)
            //Table (9.)
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate From dbo.[User]")
            //                    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                    .FirstOrDefault();

            //VIEW (9.) Table이랑 비슷한테 From테이블쪽 [User] => uvwUser 수정
            //user = _context.Users.FromSqlRaw("SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate From dbo.uvwUser")
            //                    .Where(u => u.UserId.Equals(userId) && u.Password.Equals(password))
            //                    .FirstOrDefault();

            //FUNCTION (9.)
            //FUNCTION와 STORED PROCEDURE은 파라미터 지정가능! where절을 따로 안쓴다 
            //user = _context.Users.FromSqlInterpolated($"SELECT UserId, UserName, UserEmail, Password, IsMembershipWithdrawn, JoinedUtcDate FROM dbo.ufnUser({userId},{password})")
            //                     .FirstOrDefault();

            //STORED PROCEDURE(11.)
            //FromSqlRaw()메서드 뒤에 .AsEnumerable() 메서드를 추가
            //파라메터 @p3 등 추가 가능
            //user = _context.Users.FromSqlRaw("dbo.uspCheckLoginByUserId @p0, @p1", new[] { userId, password })
            //                        .AsEnumerable().FirstOrDefault();
            user = _context.Users.FromSqlInterpolated($"dbo.uspCheckLoginByUserId {userId}, {password}")
                                  .AsEnumerable().FirstOrDefault();

            //사용자 정보가 없을 경우(12.)
            //비밀번호가 틀려야 이 쪽으로 넘어온다
            if (user == null)
            {
                //접속실패횟수에 대한 증가
                int rowAffected;


                //SQL문 직접 작성 12.(1. 쿼리 문으로도)
                //ExecuteSqlInterpolated는 테이블리스트(Users)를 받아오는 것이 아닌
                //Database에서 직접 연결되는 메서드!
                //rowAffected = _context.Database.ExecuteSqlInterpolated($"Update dbo.[User] SET AccessFailedCount += 1 WHERE UserId={userId}");

                //STORED PROCEDURE 12.(2. 프로시져로)
                //rowAffected = _context.Database.ExecuteSqlRaw("dbo.FailedLoginByUserId @p0", parameters: new[] { userId });
                rowAffected = _context.Database.ExecuteSqlInterpolated($"dbo.FailedLoginByUserId {userId}");
            }

            return user;

        }

        //checkTheUserInfo에서 id와 password입력받은 것을
        //GetUserInfos()에서 id와 password를 람다식으로
        //데이터가 있으면 true 없으면 false로 (9.)
        private bool checkTheUserInfo(string userId, string password)
        {
            //return GetUserInfos().Where(u => u.UserId.Equals(userId) && u.Password.Equals(password)).Any();
            return GetUserInfo(userId, password) != null ? true : false;
        }

        //GetUserInfo() 메서드 추가 (14. )
        private User GetUserInfo(string userId)
        {
            return _context.Users.Where(u => u.UserId.Equals(userId)).FirstOrDefault();
        }

        //권한 부분 (14. )
        private IEnumerable<UserRolesByUser> GetUserRolesByUserInfos(string userId)
        {
            var userRolesByUserInfos = _context.UserRolesByUsers.Where(uru => uru.UserId.Equals(userId)).ToList();

            //foreach : 권한에 대한 이름과 우선순위를 가져오기위해 사용
            //사용자 소유 권한, 권한이 아이디만 있는데
            foreach (var role in userRolesByUserInfos)
            {
                //사용자 소유 권한 안에 있는 사용자 권한정보에 값이 들어간다
                role.UserRole = GetUserRole(role.RoleId);
            }
            //GetRolesOwnedByUser : 구현으로 이동 > OrderByDescending : 내림차순
            return userRolesByUserInfos.OrderByDescending(uru => uru.UserRole.RolePriority);
        }

        //UserRole을 만들기 위한 메서드 추가 (14. )
        private UserRole GetUserRole(string roleId)
        {
            return _context.UserRoles.Where(ur => ur.RoleId.Equals(roleId)).FirstOrDefault();
        }

        #endregion
        //인터페이스를 상속받은 후에 명시적으로 인터페이스 구현 
        bool IUser.MatchTheUserInfo(LoginInfo login)
        {
            return checkTheUserInfo(login.UserId, login.Password);
        }

        User IUser.GetUserInfo(string userId)
        {
            return GetUserInfo(userId);
        }


        //명시적으로 구현 (14. )
        IEnumerable<UserRolesByUser> IUser.GetRolesOwnedByUser(string userId)
        {
            return GetUserRolesByUserInfos(userId);
        }

    }
}
