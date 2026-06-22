using AutoMapper;
using SecureOps.Application.Interfaces;
using SecureOps.Applications.DTOs;
using SecureOps.Applications.Interfaces;
using SecureOps.Domain.Interfaces;
using SecureOPS.Domain;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Enum;

namespace SecureOps.Application.Services
{
    public class AssetService : IAssetService
    {
        private readonly IAssetRepository _repository;
        private readonly IVulnerabilityRepository _vulnerabilityRepo;
        private readonly IRemediationRepository _remediationRepo;
        private readonly IMapper _mapper;

        public AssetService(IAssetRepository repository, IVulnerabilityRepository vulnerabilityRepository, IRemediationRepository remediationRepository, IMapper mapper)
        {
            _repository = repository;
            _vulnerabilityRepo = vulnerabilityRepository;
            _remediationRepo = remediationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AssetReadDto>> GetAllAssetsAsync()
        {
            try
            {
                var assets = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<AssetReadDto>>(assets);
            }
            catch (Exception)
            {
                throw new Exception("We encountered a problem retrieving the asset list. Please try again later.");
            }
        }

        public async Task<AssetReadDto?> GetAssetByIdAsync(int id)
        {
            try
            {
                var asset = await _repository.GetByIdAsync(id);
                return asset == null ? null : _mapper.Map<AssetReadDto>(asset);
            }
            catch (Exception)
            {
                throw new Exception($"We couldn't find the details for Asset ID {id} due to a system error.");
            }
        }

        public async Task<IEnumerable<AssetReadDto>> GetAssetsByCriticalityAsync(AssetCriticality criticality)
        {
            try
            {
                var assets = await _repository.GetByCriticalityAsync(criticality);
                return _mapper.Map<IEnumerable<AssetReadDto>>(assets);
            }
            catch (Exception)
            {
                throw new Exception($"Something went wrong while filtering assets for '{criticality}' criticality.");
            }
        }

        public async Task<AssetReadDto> CreateAssetAsync(AssetCreateDto dto)
        {
            try
            {
                if (await _repository.ExistsAsync(dto.HostName!, dto.Ipaddress!))
                {
                    // Specific business message
                    throw new InvalidOperationException($"The Hostname '{dto.HostName}' or IP Address '{dto.Ipaddress}' is already registered in our system.");
                }

                var asset = _mapper.Map<Asset>(dto);
                var createdAsset = await _repository.AddAsync(asset);

                return _mapper.Map<AssetReadDto>(createdAsset);
            }
            catch (InvalidOperationException) { throw; }
            catch (Exception)
            {
                throw new Exception("We were unable to save the new asset. Please check the information and try again.");
            }
        }

        public async Task<bool> UpdateAssetAsync(int id, AssetCreateDto dto)
        {
            try
            {
                var existingAsset = await _repository.GetByIdAsync(id);
                if (existingAsset == null)
                    throw new KeyNotFoundException($"We couldn't update the asset because an asset with ID {id} does not exist.");

                _mapper.Map(dto, existingAsset);
                await _repository.UpdateAsync(existingAsset);
                return true;
            }
            catch (KeyNotFoundException) { throw; }
            catch (Exception)
            {
                throw new Exception("Changes could not be saved at this time. Please refresh and try again.");
            }
        }
        // paging 
        public async Task<PagedResult<AssetReadDto>> GetAssetsPagedAsync(int pageNumber, int pageSize)
        {
            var (items, totalCount) = await _repository.GetPagedAsync(pageNumber, pageSize);

            var dtos = _mapper.Map<List<AssetReadDto>>(items);

            return new PagedResult<AssetReadDto>(dtos, totalCount, pageNumber, pageSize);
        }

        //public async Task<bool> DeleteAssetAsync(int id)
        //{
        //    try
        //    {
        //        var success = await _repository.DeleteAsync(id);
        //        if (!success)
        //            throw new KeyNotFoundException($"The asset you are trying to delete (ID {id}) was not found.");

        //        return true;
        //    }
        //    catch (KeyNotFoundException) { throw; }
        //    catch (Exception)
        //    {
        //        throw new Exception("The asset could not be deleted. It may be linked to other security incidents.");
        //    }
        //}
        public async Task<bool> DeleteAssetAsync(int id)
        {
            try
            {
                var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var istTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);

                // 1. Update Asset (Grandparent)
                var asset = await _repository.GetByIdAsync(id);
                if (asset == null) return false;

                asset.IsDeleted = true;
                asset.DeletedAt = istTime;
                await _repository.UpdateAsync(asset); // Direct call to repository update

                // 2. Update Vulnerabilities (Children)
                var vulnerabilities = await _vulnerabilityRepo.GetByAssetIdAsync(id);
                foreach (var vuln in vulnerabilities)
                {
                    vuln.IsDeleted = true;
                    vuln.DeletedAt = istTime;
                    // Use the specific repository method you defined
                    await _vulnerabilityRepo.UpdateAsync(vuln);

                    // 3. Update Remediations (Grandchildren)
                    var remediations = await _remediationRepo.GetByAssetIdAsync(vuln.AssetId);
                    foreach (var rem in remediations)
                    {
                        rem.IsDeleted = true;
                        rem.DeletedAt = istTime;
                        await _remediationRepo.UpdateAsync(rem);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Soft delete failed for Asset {id}", ex);
            }
        }


    }
}