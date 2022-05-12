using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;
using NetCore.Services.Interfaces;
using NetCore.Services.Svcs;
using NetCore.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace NetCore.Web.Controllers
{
    public class MembershipController : Controller
    {
        //1.전역변수로 인터페이스 설정 (4.의존성 주입)
        //2.의존성 주입 - 생성자(다른 방법도 있다)
        /*
          생성자 주입방식은 생성자의 파라미터를 통해 인터페이스를 지정하여
          서비스클래스 인스턴스를 받아온다.
        */
        private IUser _user;
        private HttpContext _context;

        public MembershipController(IHttpContextAccessor accessor, IUser user)
        {
            _context = accessor.HttpContext;
            _user = user;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/Login")]
        [ValidateAntiForgeryToken]
        //Data => Service => Web 
        //Data => Services
        //Data => Web 웹프로젝트는 데이터프로젝트 참조 (4.의존성)
        public async Task<IActionResult> LoginAsync(LoginInfo login)
        {
            string message = string.Empty;

            if (ModelState.IsValid)
            {
                //설)비교를 하고 F5를 누르고 넘어가면 성공적으로 이루어짐
                //뷰모델
                //서비스 개념 (수정 4.의존성)
                if (_user.MatchTheUserInfo(login))
                {

                    //신원보증과 승인권한(14. )
                    //var userTopRole = roles:이렇게 하기 위해서는 GetRolesOwnedByUser()메서드가 데이터가 정렬이 되어 있어야 한다
                    //> 구현으로 이동 클릭(14. )
                    var userInfo = _user.GetUserInfo(login.UserId);
                    var roles = _user.GetRolesOwnedByUser(login.UserId);
                    var userTopRole = roles.FirstOrDefault();

                    var identity = new ClaimsIdentity(claims: new[]
                    {
                        new Claim(type:ClaimTypes.Name,
                                  value:userInfo.UserName),
                        new Claim(type:ClaimTypes.Role,
                                  value:userTopRole.RoleId + "|" + userTopRole.UserRole.RoleName + "|" + userTopRole.UserRole.RolePriority.ToString())
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
                    //Index 페이지로 가기 때문에 코드추가 (14. @if~~~ )
                    return RedirectToAction("Index", "Membership");
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
        //로그아웃 비동기 메서드(14.)
        [HttpGet("/LogOut")]
        public async Task<IActionResult> LogOutAsync()
        {
            await _context.SignOutAsync(scheme: CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["Message"] = "로그아웃이 성공적으로 이루어졌습니다. <br /> 웹사이트를 원활히 하려면 로그인 하세요.";
            //로그아웃이 성공적으로 이루어 지면 Index.cshtml로 간다.
            return RedirectToAction("Index", "Membership");
        }
    }
}
