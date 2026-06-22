
using SecureOps.Applications.DTOs;
using SecureOPS.Domain.Enum;
public interface IThreatCampaignService
{
    Task<IEnumerable<ThreatCampaignReadDto>> GetAllCampaignsAsync();
    Task<ThreatCampaignReadDto?> GetCampaignByIdAsync(int id);
    Task<ThreatCampaignReadDto> CreateCampaignAsync(ThreatCampaignCreateDto dto);
    Task<bool> UpdateCampaignAsync(int id, ThreatCampaignCreateDto dto);
    Task<bool> AssignToUserAsync(int campaignId, int userId);
    Task<bool> PatchStatusAsync(int id, CampaignStatus status); // New Method
    Task<bool> DeleteCampaignAsync(int id);
}