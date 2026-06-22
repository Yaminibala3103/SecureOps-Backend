using Microsoft.EntityFrameworkCore;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOPS.Infrastructure.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly SecureOpsDbContext _context;

        public IncidentRepository(SecureOpsDbContext context)
        {
            _context = context;
        }

        public async Task<Incident> GetByIdAsync(int id)
        {
            return await _context.Incidents.FindAsync(id);
        }

        public async Task<IEnumerable<Incident>> GetAllAsync()
        {
            return await _context.Incidents.ToListAsync();
        }

        public async Task<Incident> AddAsync(Incident incident)
        {
            await _context.Incidents.AddAsync(incident);
            await _context.SaveChangesAsync();
            return incident;
        }

        public async Task UpdateAsync(Incident incident)
        {
            _context.Entry(incident).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
