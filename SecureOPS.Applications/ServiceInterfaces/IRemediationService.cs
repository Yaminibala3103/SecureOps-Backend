
using SecureOps.Applications.DTOs;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Enum;

namespace SecureOps.Application.Interfaces
{
    public interface IRemediationService
    {
        Task<IEnumerable<RemediationResponseDto>> GetAllAsync();
        Task<RemediationResponseDto?> GetByIdAsync(int id);
        Task<RemediationResponseDto> CreateRemediationAsync(CreateRemediationDto dto);
        Task<bool> UpdateStatusAsync(int id, RemediationStatus statusId);
        Task<bool> UpdateOwnerAsync(int id, int newOwner);
        Task<bool> DeleteAsync(int id);
    }
}