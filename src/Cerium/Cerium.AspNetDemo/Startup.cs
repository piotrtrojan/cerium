using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;
using Cerium.AspNetDemo.IdentityImplementations;

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
            app.CreatePerOwinContext<UserManager<ExtendedUser>>((opt, cont) => new UserManager<ExtendedUser>(cont.Get<UserStore<ExtendedUser>>()));
            app.CreatePerOwinContext<SignInManager<ExtendedUser, string>>(
                (opt, cont) => 
                    new SignInManager<ExtendedUser, string>(cont.Get<UserManager<ExtendedUser>>(), cont.Authentication)
            );

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
