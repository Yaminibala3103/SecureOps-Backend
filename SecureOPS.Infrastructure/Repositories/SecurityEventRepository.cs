using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SecureOPS.Infrastructure.Repositories
{
    public class SecurityEventRepository : ISecurityEventRepository
    {
        private readonly SecureOpsDbContext _context;
        public SecurityEventRepository(SecureOpsDbContext context) => _context = context;

        public async Task<SecurityEvent> GetByIdAsync(int id) => await _context.SecurityEvents.FindAsync(id);

        public async Task<IEnumerable<SecurityEvent>> GetAllAsync() => await _context.SecurityEvents.ToListAsync();

        public async Task<SecurityEvent> AddAsync(SecurityEvent entity)
        {
            await _context.SecurityEvents.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task UpdateAsync(SecurityEvent entity)
        {
            _context.SecurityEvents.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _context.SecurityEvents.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
