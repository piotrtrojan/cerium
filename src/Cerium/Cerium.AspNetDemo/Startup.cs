using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Security.Cookies;

[assembly: OwinStartup(typeof(Cerium.AspNetDemo.Startup))]

namespace Cerium.AspNetDemo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Initial Catalog=Cerium.Ex3;Integrated Security=True";
            app.CreatePerOwinContext(() => new IdentityDbContext(connectionString));
            app.CreatePerOwinContext<UserStore<IdentityUser>>((opt, cont) => new UserStore<IdentityUser>(cont.Get<IdentityDbContext>()));
            app.CreatePerOwinContext<UserManager<IdentityUser>>((opt, cont) => new UserManager<IdentityUser>(cont.Get<UserStore<IdentityUser>>()));
            app.CreatePerOwinContext<SignInManager<IdentityUser, string>>(
                (opt, cont) => 
                    new SignInManager<IdentityUser, string>(cont.Get<UserManager<IdentityUser>>(), cont.Authentication)
            );

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie
            });
        }
    }
}
