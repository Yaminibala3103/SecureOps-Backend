
using SecureOPS.Domain.Enum;
using SecureOPS.Domain.Entities;

public interface IThreatCampaignRepository
{
    Task<IEnumerable<ThreatCampaign>> GetAllAsync();
    Task<ThreatCampaign?> GetByIdAsync(int id);
    Task<ThreatCampaign> AddAsync(ThreatCampaign campaign);
    Task<bool> AssignCampaignAsync(int campaignId, int userId);
    Task<bool> UpdateStatusAsync(int id, CampaignStatus status);
    Task UpdateAsync(ThreatCampaign campaign);
    Task<bool> DeleteAsync(int id);
}