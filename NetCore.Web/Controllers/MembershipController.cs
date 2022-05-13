using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Web.Controllers
{
    //15. 권한부여 
    /*
        사용가능 권한 4개 로그인을 해야 MembershipController로 들어온다
        IActionResult여기로 권한을 해도 된다
     */
    [Authorize(Roles = "AssociateUser, GeneralUser, SuperUser, SystemUser")]
    public class MembershipController : Controller
    {
        //1.전역변수로 인터페이스 설정 (4.의존성 주입)
        //2.의존성 주입 - 생성자(다른 방법도 있다)
        /*
          생성자 주입방식은 생성자의 파라미터를 통해 인터페이스를 지정하여
          서비스클래스 인스턴스를 받아온다.
        */
        private IUser _user;
        private IPasswordHasher _hasher;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IPasswordHasher hasher, IUser user)
        {
            _context = accessor.HttpContext;
            _hasher = hasher;
            _user = user;
        }

        //15.
        #region private methods
        /// <summary>
        /// 로컬URL인지 외부URL인지 체크
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(MembershipController.Index), "Membership");
            }
        }
        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost("/{controller}/Login")]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        //Data => Services => Web 
        //Data => Services
        //Data => Web 웹프로젝트는 데이터프로젝트 참조 (4.의존성)
        public async Task<IActionResult> LoginAsync(LoginInfo login, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;

            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //설)비교를 하고 F5를 누르고 넘어가면 성공적으로 이루어짐
                //뷰모델
                //서비스 개념 (수정 4.의존성)
                //if (_user.MatchTheUserInfo(login))

                if (_hasher.MachTheUserInfo(login.UserId, login.Password))
                {

                    //신원보증과 승인권한(14. )
                    //var userTopRole = roles:이렇게 하기 위해서는 GetRolesOwnedByUser()메서드가 데이터가 정렬이 되어 있어야 한다
                    //> 구현으로 이동 클릭(14. )
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();
                    string userDataInfo = userTopRole.UserRole.RoleName + "|" +
                                          userTopRole.UserRole.RolePriority.ToString() + "|" + 
                                          userInfo.UserName + "|" +
                                          userInfo.UserEmail;

                    //_context.User.Identity.Name => 사용자 아이디

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type:ClaimTypes.Name,
                                  value:userInfo.UserId),
                        new Claim(type:ClaimTypes.Role,
                        // 15. "|"파이프 부분은 나중에 번거로워서 삭제 > "|" + userTopRole.UserRole.RoleName + "|" + userTopRole.UserRole.RolePriority.ToString()
                                  value:userTopRole.RoleId),
                        new Claim(type:ClaimTypes.UserData,
                                  value:userDataInfo)
                    //기본쿠키 지정, 계속 반복되는 구문
                    }, authenticationType: CookieAuthenticationDefaults.AuthenticationScheme);

                    await _context.SignInAsync(scheme:CookieAuthenticationDefaults.AuthenticationScheme,
                                               principal:new ClaimsPrincipal(identity:identity),
                                               properties:new AuthenticationProperties() 
                                               {
                                                    //지속여부 지정
                                                    IsPersistent = login.RememberMe,

                                                    //인증쿠키 만료기간
                                                    ExpiresUtc = login.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddMinutes(30)
                                               });

                    //설)1회성으로 1번만 나온다!
                    TempData["Message"] = "로그인이 성공적으로 이루어졌습니다.";

                    //설)Index 페이지로 가서 TempData의 메세지를 화면에 뿌려준다
                    //Index 페이지로 가기 때문에 코드추가 (14. Index에 @if~~~ )
                    //return RedirectToAction("Index", "Membership");
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    message = "로그인되지 않습니다.";
                }
            }
            else
            {
                message = "로그인 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            //View로 View모델 넘길때 어떤 View로 할것인지 정해 줘야함
            return View("Login", login);
        }
        //로그아웃 비동기 메서드(14.)aa
        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br /> 웹사이트를 원활히 하려면 로그인 하세요.";
            //로그아웃이 성공적으로 이루어 지면 Index.cshtml로 간다.
            return RedirectToAction("Index", "Membership");
        }

        //15.    (DataController 설명 참고
        //Forbidden 뷰 추가  Forbidden.cshtml
        /*
            팅겨져 나오면 Forbidden()여기로 오는데 이전에 요청했던 URL은 팅겨나간 그 페이지
            그 페이지를 화면에 표시하기 위해서 returnUrl을 사용
         */
        [HttpGet]
        [Authorize(Roles = "AssociateUser")]
        public IActionResult Forbidden()
        {
            StringValues paramReturnUrl;
            //존재여부  returnUrl에 대해서 paramReturnUrl이 있는지 체크
            bool exists = _context.Request.Query.TryGetValue("returnUrl", out paramReturnUrl);
            paramReturnUrl = exists ? _context.Request.Host.Value + paramReturnUrl[0] : string.Empty;

            ViewData["Message"] = $"귀하는 {paramReturnUrl} 경로로 접근하려고 했습니다만, <br>" +
                                   "인증된 사용자도 접근하지 못하는 페이지가 있습니다. <br>" +
                                   "담당자에게 해당페이지의 접근권한에 대해 문의하세요.";
            return View();
        }
    }
}
