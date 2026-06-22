
using SecureOPS.Domain.Enum;
using SecureOPS.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOps.Applications.Interfaces
{
    public interface IAssetRepository
    {
        Task<IEnumerable<Asset>> GetAllAsync();
        Task<Asset?> GetByIdAsync(int id);
        Task<IEnumerable<Asset>> GetByCriticalityAsync(AssetCriticality criticality);
        Task<Asset> AddAsync(Asset asset);
        Task UpdateAsync(Asset asset);
        Task<bool> DeleteAsync(int id);
        Task<(List<Asset> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize);

        Task<bool> ExistsAsync(string hostname, string ip);
    }
}
