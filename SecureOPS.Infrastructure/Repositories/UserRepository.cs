using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using SecureOPS.Applications.RepositoryInterfaces;

namespace SecureOPS.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly SecureOpsDbContext _context;

        public UserRepository(SecureOpsDbContext context) => _context = context;

        public async Task<User> GetByIdAsync(int id) =>
            await _context.Users.FindAsync(id);

        public async Task<User> GetUserWithNotificationsAsync(int userId)
        {
            // Essential for checking if notifications were sent to the right person
            return await _context.Users
                .Include(u => u.Notifications)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(string role)
        {
            return await _context.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        public User? GetByEmail(string email)
        {
            throw new NotImplementedException();
        }

        public User? GetByResetToken(string token)
        {
            throw new NotImplementedException();
        }

        public void Add(User user)
        {
            throw new NotImplementedException();
        }

        public void Update(User user)
        {
            throw new NotImplementedException();
        }
    }
}
