
using SecureOPS.Domain.Entities;

namespace SecureOps.Application.Interfaces
{
    public interface IThreatIndicatorRepository
    {
        Task<IEnumerable<ThreatIndicator>> GetAllAsync();
        Task<ThreatIndicator?> GetByIdAsync(int id);
        Task<IEnumerable<ThreatIndicator>> GetByCampaignIdAsync(int campaignId);
        Task<ThreatIndicator> AddAsync(ThreatIndicator indicator);
        Task<bool> UpdateAsync(ThreatIndicator indicator);
        Task<bool> DeleteAsync(int id);
    }
}