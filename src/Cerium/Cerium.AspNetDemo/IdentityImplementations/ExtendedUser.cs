using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;

namespace Cerium.AspNetDemo.IdentityImplementations
{
    public class ExtendedUser : IdentityUser
    {
        public ExtendedUser()
        {
            Addresses = new List<Address>();
        }
        public string FullName { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }
    }
}