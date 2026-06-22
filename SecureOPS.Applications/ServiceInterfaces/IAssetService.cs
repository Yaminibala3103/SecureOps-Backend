
using SecureOps.Applications.DTOs;
using SecureOPS.Domain;
using SecureOPS.Domain.Enum;

namespace SecureOps.Application.Interfaces
{
    public interface IAssetService
    {

        Task<IEnumerable<AssetReadDto>> GetAllAssetsAsync();

        Task<AssetReadDto?> GetAssetByIdAsync(int id);

        Task<IEnumerable<AssetReadDto>> GetAssetsByCriticalityAsync(AssetCriticality criticality);

        Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto);

        Task<bool> UpdateAssetAsync(int id, AssetCreateDto dto);
        Task<PagedResult<AssetReadDto>> GetAssetsPagedAsync(int pageNumber, int pageSize);


        Task<bool> DeleteAssetAsync(int id);
    }
}