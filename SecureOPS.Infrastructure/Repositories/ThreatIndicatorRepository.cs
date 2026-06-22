using Microsoft.EntityFrameworkCore;
using SecureOps.Application.Interfaces;

using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;


namespace SecureOps.Infrastructure.Repositories
{
    public class ThreatIndicatorRepository : IThreatIndicatorRepository
    {
        private readonly SecureOpsDbContext _context;

        public ThreatIndicatorRepository(SecureOpsDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ThreatIndicator>> GetAllAsync()
        {
            return await _context.ThreatIndicators
                .Include(i => i.Campaign)
                .ToListAsync();
        }

        public async Task<ThreatIndicator?> GetByIdAsync(int id)
        {
            return await _context.ThreatIndicators
                .Include(i => i.Campaign)
                .FirstOrDefaultAsync(i => i.IndicatorId == id);
        }

        public async Task<IEnumerable<ThreatIndicator>> GetByCampaignIdAsync(int campaignId)
        {
            return await _context.ThreatIndicators
                .Where(i => i.CampaignId == campaignId)
                .ToListAsync();
        }

        public async Task<ThreatIndicator> AddAsync(ThreatIndicator indicator)
        {
            await _context.ThreatIndicators.AddAsync(indicator);
            await _context.SaveChangesAsync();
            return indicator;
        }

        public async Task<bool> UpdateAsync(ThreatIndicator indicator)
        {
            _context.Entry(indicator).State = EntityState.Modified;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var indicator = await _context.ThreatIndicators.FindAsync(id);
            if (indicator == null) return false;


            _context.ThreatIndicators.Remove(indicator);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}