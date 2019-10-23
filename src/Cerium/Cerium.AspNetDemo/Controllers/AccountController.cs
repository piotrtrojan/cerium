using Cerium.AspNetDemo.IdentityImplementations;
using Cerium.AspNetDemo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Cerium.AspNetDemo.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<ExtendedUser> UserManager => HttpContext.GetOwinContext().Get<UserManager<ExtendedUser>>();
        public SignInManager<ExtendedUser, string> SignInManager => HttpContext.GetOwinContext().Get<SignInManager<ExtendedUser, string>>();

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model)
        {
            var signInStatus = await SignInManager.PasswordSignInAsync(model.Username, model.Paswsword, true, true);
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("ChooseProvider");
                default:
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
            }
        }


        [HttpPost]
        public async Task<ActionResult> ChooseProvider(ChooseProviderModel model)
        {
            await SignInManager.SendTwoFactorCodeAsync(model.ChosenProvider);
            return RedirectToAction("TwoFactor", new { Provider = model.ChosenProvider });
        }

        [HttpGet]
        public async Task<ActionResult> TwoFactor(string provider)
        {
            return View(new TwoFactorModel { Provider = provider });
        }

        [HttpPost]
        public async Task<ActionResult> TwoFactor(TwoFactorModel model)
        {
            var signInStatus = SignInManager.TwoFactorSignIn(model.Provider, model.Code, true, model.RememberBrowser);
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid Token");
                    return View(model);
            }
        }

        [HttpGet]
        public async Task<ActionResult> ChooseProvider()
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            var providers = await UserManager.GetValidTwoFactorProvidersAsync(userId);

            return View(new ChooseProviderModel { Providers = providers.ToList() });
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var userInDb = await UserManager.FindByNameAsync(model.Username);
            if (userInDb != null)
            {
                return RedirectToAction("Index", "Home");
            }
            var extendedUser = new ExtendedUser
            {
                UserName = model.Username,
                FullName = model.FullName,
            };
            extendedUser.Addresses.Add(new Address { AddressLine = model.AddressLine, Country = model.Country, UserId = extendedUser.Id });
            var registrationResult = await UserManager.CreateAsync(extendedUser, model.Paswsword);
            if (registrationResult.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", registrationResult.Errors.FirstOrDefault());
            return View(model);
        }
    }
}