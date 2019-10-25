using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;

namespace Cerium.AspNetDemo.IdentityImplementations
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            Console.WriteLine($"Message to: {message.Destination}.");
            Console.WriteLine("Content:");
            Console.WriteLine(message.Body);
            return Task.CompletedTask;
        }
    }
}