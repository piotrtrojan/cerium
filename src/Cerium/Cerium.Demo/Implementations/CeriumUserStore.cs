using Microsoft.AspNet.Identity;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Cerium.Demo.Implementations
{
    public class CeriumUserStore : IUserPasswordStore<CeriumUser, int>
    {
        private readonly CeriumDbContext _context;

        public CeriumUserStore(CeriumDbContext context)
        {
            _context = context;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
        
        public Task CreateAsync(CeriumUser user)
        {
            _context.Users.Add(user);
            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync(CeriumUser user)
        {
            _context.Users.Attach(user);
            return _context.SaveChangesAsync();
        }

        public Task DeleteAsync(CeriumUser user)
        {
            _context.Users.Remove(user);
            return _context.SaveChangesAsync();
        }
        
        public Task<CeriumUser> FindByIdAsync(int userId)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        }

        public Task<CeriumUser> FindByNameAsync(string userName)
        {
            return _context.Users.FirstOrDefaultAsync(x => x.UserName == userName);
        }

        public Task SetPasswordHashAsync(CeriumUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(CeriumUser user)
        {
            return Task.FromResult(user.PasswordHash);    
        }
         
        public Task<bool> HasPasswordAsync(CeriumUser user)
        {
            return Task.FromResult(user.PasswordHash != null);
        }
    }
}
