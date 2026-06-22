using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace SecureOPS.Infrastructure.Repositories
{
    public class AlertRepository : IAlertRepository
    {
        private readonly SecureOpsDbContext _context;
        public AlertRepository(SecureOpsDbContext context) => _context = context;

		public async Task<Alert> GetByIdAsync(int id) => await _context.Alerts.FindAsync(id);

		public async Task<IEnumerable<Alert>> GetAllAsync() => await _context.Alerts.ToListAsync();
        public async Task<Alert> AddAsync(Alert entity)
        {
            await _context.Alerts.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(Alert entity)
        {
            _context.Alerts.Update(entity);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null) { _context.Alerts.Remove(entity); await _context.SaveChangesAsync(); }
        }
    }
}
