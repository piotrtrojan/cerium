using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Cerium.AspNetDemo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<IdentityUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<IdentityUser>>();

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async ActionResult Register(RegisterModel model)
        {
            var registrationResult = await UserManager.CreateAsync(new IdentityUser(model.Username), model.Paswsword);
            if (registrationResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", registrationResult.Errors.FirstOrDefault());
            return View(model);
        }

        public class RegisterModel
        {
            public string Username { get; set; }
            public string Paswsword { get; set; }
        }
    }
}