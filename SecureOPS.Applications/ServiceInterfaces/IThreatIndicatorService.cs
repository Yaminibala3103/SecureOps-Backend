
using SecureOps.Applications.DTOs;

using System;
using System.Collections.Generic;
using System.Text;

namespace SecureOps.Application.ServiceInterfaces
{
    public interface IThreatIndicatorService
    {
        Task<IEnumerable<ThreatIndicatorReadDto>> GetAllIndicatorsAsync();
        Task<ThreatIndicatorReadDto?> GetIndicatorByIdAsync(int id);
        Task<ThreatIndicatorReadDto> CreateIndicatorAsync(ThreatIndicatorCreateDto dto);
        Task<bool> UpdateIndicatorAsync(int id, ThreatIndicatorCreateDto dto);
        Task<bool> DeleteIndicatorAsync(int id);
    }
}
