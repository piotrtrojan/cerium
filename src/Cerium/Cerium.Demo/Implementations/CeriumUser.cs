using Microsoft.AspNet.Identity;

namespace Cerium.Demo.Implementations
{
    public class CeriumUser : IUser<int>
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}
