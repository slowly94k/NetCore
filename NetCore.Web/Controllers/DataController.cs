using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using NetCore.Data.ViewModels;

//13.
namespace NetCore.Web.Controllers
{
    public class DataController : Controller
    {
        //의존성 주입으로 데이터보호 부분을 가져온다
        private IDataProtector _protector;

        public DataController(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("NetCore.Data.v1");
        }


        [HttpGet]
        [Authorize(Roles = "GeneralUser, SuperUser, SystemUser")]
        /*
            15.
            AssociateUser의 권한만 가지고 있는 준사용자는 여기로 들어 올 수 없다.
            AES()메서드로 들어왔을 때 팅겨내는 페이지 필요
            그래서 Startup.cs > ConfigureServices() 메서드 > AddAuthentication()에서  .AddCookie 기능을 더해서 
            options.AccessDeniedPath 권한이 없어서 가는 페이지를
            "/Membership/Forbidden";  Membership의 밑에 Forbidden으로 페이지 지정 
         */
        public IActionResult AES()
        {
            return View();
        }

        //여기 View모델에서는 페이지를 만들어서 사용자아이디와 비번을 입력을 하고
        //AES 암호 생성하기 버튼을 누르면 "return View()"여기서 암복호화 값을 넣어 줄거다.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "GeneralUser, SuperUser, SystemUser")]
        public IActionResult AES(AESInfo aes)
        {
            string message = string.Empty;
            if (ModelState.IsValid)
            {
                string userInfo = aes.UserId + aes.Password;
                aes.EncUserInfo = _protector.Protect(userInfo);//암호화 정보
                aes.DecUserInfo = _protector.Unprotect(aes.EncUserInfo);//복호화 정보

                ViewData["Message"] = "암복호화가 성공적으로 이루어졌습니다.";

                return View(aes);
            }
            else
            {
                message = "복호화를 위한 정보를 올바르게 입력하세요.";
            }

            ModelState.AddModelError(string.Empty, message);
            return View();
        }
    }
}
