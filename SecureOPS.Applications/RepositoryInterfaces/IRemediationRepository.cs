
using SecureOPS.Domain.Entities;

namespace SecureOps.Domain.Interfaces;

public interface IRemediationRepository
{

    Task<IEnumerable<Remediation>> GetAllActiveAsync();


    Task<Remediation?> GetByIdAsync(int id);
    Task<List<Remediation>> GetByAssetIdAsync(int assetId);


    Task<Remediation> AddAsync(Remediation remediation);


    Task<IEnumerable<Remediation>> GetByVulnerabilityIdAsync(int vulnerabilityId);
    Task UpdateAsync(Remediation entity);
   // Task GetByIdAsync(int? assignedToUserId);
}