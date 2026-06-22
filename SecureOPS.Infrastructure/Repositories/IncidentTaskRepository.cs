using Microsoft.EntityFrameworkCore;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Infrastructure.Repositories
{
    public class IncidentTaskRepository : IIncidentTaskRepository
    {
        private readonly SecureOpsDbContext _context;
        public IncidentTaskRepository(SecureOpsDbContext context) => _context = context;

        public async Task<IncidentTask> GetByIdAsync(int id) => await _context.IncidentTasks.FindAsync(id);

        public async Task<IEnumerable<IncidentTask>> GetByIncidentIdAsync(int incidentId) =>
            await _context.IncidentTasks.Where(t => t.IncidentId == incidentId).ToListAsync();

        public async Task<IncidentTask> AddAsync(IncidentTask task)
        {
            await _context.IncidentTasks.AddAsync(task);
            await _context.SaveChangesAsync();
            return task;
        }

        public async Task UpdateAsync(IncidentTask task)
        {
            _context.Entry(task).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
