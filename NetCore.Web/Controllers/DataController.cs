﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public IActionResult AES()
        {
            return View();
        }

        //여기 View모델에서는 페이지를 만들어서 사용자아이디와 비번을 입력을 하고
        //AES 암호 생성하기 버튼을 누르면 "return View()"여기서 암복호화 값을 넣어 줄거다.
        [HttpPost]
        [ValidateAntiForgeryToken]
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
