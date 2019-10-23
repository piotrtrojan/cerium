using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;

namespace Cerium.AspNetDemo.IdentityImplementations
{
    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            Console.WriteLine($"Phone: {message.Destination}");
            Console.WriteLine($"Content: {message.Body}");
            return Task.CompletedTask; 
        }
    }
}