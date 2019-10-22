using Cerium.Demo.Implementations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;

namespace Cerium.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ex1();
            Ex2();
            Console.ReadLine();
        }

        private static void Ex2()
        {
            var username = "Piotr";
            var password = "1234567890";

            var userStore = new CeriumUserStore(new CeriumDbContext());
            var userManager = new UserManager<CeriumUser, int>(userStore);

            var creationResult = userManager.Create(new CeriumUser { UserName = username }, password);
            Console.WriteLine($"Creation result: {creationResult.Succeeded}");

            var user = userManager.FindByName(username);

            var passwordMatching = userManager.CheckPassword(user, password);
            var passwordNonMatching = userManager.CheckPassword(user, "asd");
            Console.WriteLine($"Password 1 matching: {passwordMatching}");
            Console.WriteLine($"Password 2 matching: {passwordNonMatching}");
        }

        static void Ex1()
        {
            var username = "Piotr";
            var password = "1234567890";

            var userStore = new UserStore<IdentityUser>();
            var userManager = new UserManager<IdentityUser>(userStore);

            // var creationResult = userManager.Create(new IdentityUser(username), password);
            // Console.WriteLine($"Creatione result: {creationResult.Succeeded}");

            var user = userManager.FindByName(username);
            // var claimResult = userManager.AddClaim(user.Id, new Claim("given_name", "Admin"));
            // Console.WriteLine($"Claim result: {claimResult.Succeeded}");

            var passwordMatching = userManager.CheckPassword(user, password);
            var passwordNonMatching = userManager.CheckPassword(user, "asd");
            Console.WriteLine($"Password 1 matching: {passwordMatching}");
            Console.WriteLine($"Password 2 matching: {passwordNonMatching}");

        }
    }
}
