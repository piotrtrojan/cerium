using Cerium.AspNetDemo.IdentityImplementations;
using Cerium.AspNetDemo.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
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
                case SignInStatus.LockedOut:
                    var user = await UserManager.FindByNameAsync(model.Username);
                    if (user != null && await UserManager.CheckPasswordAsync(user, model.Paswsword))
                    {
                        ModelState.AddModelError("", "Account Locked");
                        return View(model);
                    }
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
                default:
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
            }
        }
        
        [HttpGet]
        public ActionResult TwoFactor(string provider)
        {
            return View(new TwoFactorModel { Provider = provider });
        }

        [HttpPost]
        public ActionResult TwoFactor(TwoFactorModel model)
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

        public ActionResult ExternalAuthentication(string provider)
        {
            SignInManager.AuthenticationManager.Challenge(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("ExternalCallback", new { provider }),
                }, provider);
            return new HttpUnauthorizedResult();
        }

        public async Task<ActionResult> ExternalCallback(string provider)
        {
            var loginInfo = await SignInManager.AuthenticationManager.GetExternalLoginInfoAsync();
            var signInStatus = await SignInManager.ExternalSignInAsync(loginInfo, true);
            switch (signInStatus)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    var user = await UserManager.FindByEmailAsync(loginInfo.Email);
                    if (user != null)
                    {
                        var result = await UserManager.AddLoginAsync(user.Id, loginInfo.Login);
                        if (result.Succeeded)
                        {
                            return await ExternalCallback(provider);
                        }
                    }
                    return View("Error");
            }

        }

        [HttpGet]
        public async Task<ActionResult> ChooseProvider()
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            var providers = await UserManager.GetValidTwoFactorProvidersAsync(userId);

            return View(new ChooseProviderModel { Providers = providers.ToList() });
        }

        [HttpPost]
        public async Task<ActionResult> ChooseProvider(ChooseProviderModel model)
        {
            await SignInManager.SendTwoFactorCodeAsync(model.ChosenProvider);
            return RedirectToAction("TwoFactor", new { Provider = model.ChosenProvider });
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
                Email = model.Username,
                UserName = model.Username,
                FullName = model.FullName,
            };
            extendedUser.Addresses.Add(new Address { AddressLine = model.AddressLine, Country = model.Country, UserId = extendedUser.Id });
            var registrationResult = await UserManager.CreateAsync(extendedUser, model.Paswsword);
            if (registrationResult.Succeeded)
            {
                var token = await UserManager.GenerateEmailConfirmationTokenAsync(extendedUser.Id);
                var confirmUrl = Url.Action("ConfirmEmail", "Account", new { userid = extendedUser.Id, token }, Request.Url.Scheme);
                await UserManager.SendEmailAsync(extendedUser.Id, "Email Confirmation", $"Use link to confirm email: {confirmUrl}");
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", registrationResult.Errors.FirstOrDefault());
            return View(model);
        }

        [HttpGet]
        public ActionResult PasswordReset(string userId, string token)
        {
            return View(new PasswordResetModel { UserId = userId, Token = token });
        }

        [HttpPost]
        public async Task<ActionResult> PasswordReset(PasswordResetModel model)
        {
            var resetResult = await UserManager.ResetPasswordAsync(model.UserId, model.Token, model.Password);
            if (!resetResult.Succeeded)
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordModel model)
        {
            var userInDb = await UserManager.FindByNameAsync(model.Username);
            if (userInDb != null)
            {
                var token = await UserManager.GeneratePasswordResetTokenAsync(userInDb.Id);
                var url = Url.Action("PasswordReset", "Account", new { userid = userInDb.Id, token }, Request.Url.Scheme);
                await UserManager.SendEmailAsync(userInDb.Id, "Password reser", $"Reset password here {url}");
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string userId, string token)
        {
            var confirmationResult = await UserManager.ConfirmEmailAsync(userId, token);
            if (!confirmationResult.Succeeded)
            {
                return View("Error");
            }
            return RedirectToAction("Index", "Home");
        }
    }
}