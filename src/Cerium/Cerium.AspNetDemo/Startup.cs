using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Cerium.AspNetDemo.IdentityImplementations;
using System;
using Microsoft.Owin.Security.Google;
using System.Configuration;

[assembly: OwinStartup(typeof(Cerium.AspNetDemo.Startup))]

namespace Cerium.AspNetDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Cerium.Ex4;Integrated Security=True";
            app.CreatePerOwinContext(() => new ExtendedUserDbContext(connectionString));
            app.CreatePerOwinContext<UserStore<ExtendedUser>>((opt, cont) => new UserStore<ExtendedUser>(cont.Get<ExtendedUserDbContext>()));
            app.CreatePerOwinContext<UserManager<ExtendedUser>>((opt, cont) =>
            {
                var userManager = new UserManager<ExtendedUser>(cont.Get<UserStore<ExtendedUser>>());
                userManager.RegisterTwoFactorProvider("SMS", 
                    new PhoneNumberTokenProvider<ExtendedUser>() 
                    { 
                        MessageFormat = "Token: {0}" 
                    });
                userManager.SmsService = new SmsService();
                userManager.UserTokenProvider = new DataProtectorTokenProvider<ExtendedUser>(opt.DataProtectionProvider.Create());
                userManager.EmailService = new EmailService();
                return userManager;
            });
            app.CreatePerOwinContext<SignInManager<ExtendedUser, string>>(
                (opt, cont) => 
                    new SignInManager<ExtendedUser, string>(cont.Get<UserManager<ExtendedUser>>(), cont.Authentication)
            );

            
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));
            
            // Line Below enables remembering Two Factor auth cookie if user wants it.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorCookie);
            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);
            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions
            {
                ClientId = ConfigurationManager.AppSettings["GoogleAuth:Id"],
                ClientSecret = ConfigurationManager.AppSettings["GoogleAuth:Secret"],
                Caption = "Login with Google"
            });

        }
    }
}
