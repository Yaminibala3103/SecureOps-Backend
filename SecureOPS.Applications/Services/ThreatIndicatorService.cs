using AutoMapper;

using SecureOps.Application.Interfaces;
using SecureOps.Application.ServiceInterfaces;
using SecureOps.Applications.DTOs;
using SecureOPS.Domain.Entities;

namespace SecureOps.Application.Services
{
    public class ThreatIndicatorService : IThreatIndicatorService
    {
        private readonly IThreatIndicatorRepository _repository;
        private readonly IMapper _mapper;

        public ThreatIndicatorService(IThreatIndicatorRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ThreatIndicatorReadDto>> GetAllIndicatorsAsync()
        {
            try
            {
                var indicators = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<ThreatIndicatorReadDto>>(indicators);
            }
            catch (Exception)
            {
                throw new Exception("We are currently unable to load the threat indicators. Please check back shortly.");
            }
        }

        public async Task<ThreatIndicatorReadDto?> GetIndicatorByIdAsync(int id)
        {
            try
            {
                var indicator = await _repository.GetByIdAsync(id);
                return indicator == null ? null : _mapper.Map<ThreatIndicatorReadDto>(indicator);
            }
            catch (Exception)
            {
                throw new Exception($"An error occurred while trying to find information for indicator ID {id}.");
            }
        }

        public async Task<ThreatIndicatorReadDto> CreateIndicatorAsync(ThreatIndicatorCreateDto dto)
        {
            try
            {
                var entity = _mapper.Map<ThreatIndicator>(dto);
                var result = await _repository.AddAsync(entity);
                return _mapper.Map<ThreatIndicatorReadDto>(result);
            }
            catch (Exception)
            {
                throw new Exception("The system couldn't save this indicator. Please ensure the Campaign ID is correct and all fields are filled.");
            }
        }

        public async Task<bool> UpdateIndicatorAsync(int id, ThreatIndicatorCreateDto dto)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(id);
                if (existing == null) return false;

                _mapper.Map(dto, existing);
                await _repository.UpdateAsync(existing);
                return true;
            }
            catch (Exception)
            {
                throw new Exception("We couldn't update the indicator details. Your changes have not been saved.");
            }
        }

        public async Task<bool> DeleteIndicatorAsync(int id)
        {
            try
            {

                var indicator = await _repository.GetByIdAsync(id);


                if (indicator == null)
                {
                    return false;
                }


                var istTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
                var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, istTimeZone);

                indicator.IsDeleted = true;
                indicator.DeletedAt = istNow;


                return await _repository.UpdateAsync(indicator);
            }
            catch (Exception)
            {
                throw new Exception("A problem occurred while trying to remove the indicator from the active list.");
            }
        }
    }
}