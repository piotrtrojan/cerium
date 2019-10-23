using System;

namespace Cerium.AspNetDemo.IdentityImplementations
{
    public class Address
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
    }
}