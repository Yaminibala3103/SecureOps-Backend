using AutoMapper;

using SecureOps.Application.Interfaces;
using SecureOps.Applications.DTOs;
using SecureOps.Applications.Interfaces;
using SecureOPS.Domain.Enum;
using SecureOps.Domain.Interfaces;
using SecureOPS.Domain.Entities;
using SecureOPS.Applications.ServiceInterfaces;

public class RemediationService : IRemediationService
{
    private readonly IRemediationRepository _repo;
    private readonly IVulnerabilityRepository _vulnRepo;
    private readonly IAssetRepository _assetRepo;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public RemediationService(IRemediationRepository repo, IVulnerabilityRepository vulnRepo,
                              IAssetRepository assetRepo, IMapper mapper, IUserService userService)
    {
        _repo = repo;
        _vulnRepo = vulnRepo;
        _assetRepo = assetRepo;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<RemediationResponseDto> CreateRemediationAsync(CreateRemediationDto dto)
    {
        try
        {
            var asset = await _assetRepo.GetByIdAsync(dto.AssetId);
            if (asset == null) throw new KeyNotFoundException($"Asset {dto.AssetId} not found.");

            var vuln = await _vulnRepo.GetByIdAsync(dto.VulnerabilityId);
            if (vuln == null) throw new KeyNotFoundException($"Vulnerability {dto.VulnerabilityId} not found.");

            // Role Validation Logic
            if (dto.AssignedToUserId.HasValue)
            {
                var user = await _userService.GetByIdAsync(dto.AssignedToUserId.Value); //
                var validRoles = new[] { "Threat Hunter", "Security Admin", "L2 Analyst" }; //

                if (user == null || !validRoles.Contains(user.Role)) //
                {
                    throw new Exception("Remediation can only be assigned to authorized personnel."); //
                }
            }

            // SLA Logic: Auto-calculate based on Severity enum
            int slaDays = vuln.Severity switch
            {
                SeverityLevel.Critical => 7,
                SeverityLevel.High => 15,
                SeverityLevel.Medium => 30,
                SeverityLevel.Low => 60,
                _ => 100
            };

            var remediation = new Remediation
            {
                VulnerabilityId = dto.VulnerabilityId,
                AssetId = dto.AssetId,
                // Use assigned user or default to asset ID as per your logic
                AssignedToUserId = dto.AssignedToUserId ?? asset.AssetId,
                // Fix for CS0029: Cast int to Enum
                Status = (RemediationStatus)dto.Status, //
                TargetDate = (vuln.PublishedDate ?? DateTime.Now).AddDays(slaDays),
                IsDeleted = false
            };

            // Fix for CS0029: Ensure repository returns the entity or its ID correctly
            var savedEntity = await _repo.AddAsync(remediation); //

            // Use Mapper to return the Response DTO
            return _mapper.Map<RemediationResponseDto>(savedEntity);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to create remediation: {ex.Message}");
        }
    }
    public async Task<IEnumerable<RemediationResponseDto>> GetAllAsync() =>
       _mapper.Map<IEnumerable<RemediationResponseDto>>(await _repo.GetAllActiveAsync());

    public async Task<RemediationResponseDto?> GetByIdAsync(int id) =>
        _mapper.Map<RemediationResponseDto>(await _repo.GetByIdAsync(id));

    public async Task<bool> UpdateStatusAsync(int id, RemediationStatus statusId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            entity.Status = (RemediationStatus)statusId; // Cast int to Enum
            await _repo.UpdateAsync(entity);
            return true;
        }
        catch (Exception) { return false; }
    }
    public async Task<bool> UpdateOwnerAsync(int id, int newOwner)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);

            if (entity == null) return false;


            entity.AssignedToUserId = newOwner;


            await _repo.UpdateAsync(entity);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to reassign owner for task {id}: {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(int id)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;
            entity.IsDeleted = true; // Soft Delete logic
            entity.DeletedAt = DateTime.Now;
            await _repo.UpdateAsync(entity);
            return true;
        }
        catch { return false; }
    }


}