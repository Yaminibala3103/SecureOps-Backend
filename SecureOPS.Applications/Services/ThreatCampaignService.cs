using AutoMapper;
using SecureOps.Application.Interfaces;
using SecureOps.Applications.DTOs;
using SecureOPS.Applications.RepositoryInterfaces;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Enum;

namespace SecureOps.Application.Services
{
    public class ThreatCampaignService : IThreatCampaignService
    {
        private readonly IThreatCampaignRepository _repository;
        private readonly IThreatIndicatorRepository _indicatorRepo;
        private readonly IUserRepository _userRepo;
        private readonly IMapper _mapper;


        public ThreatCampaignService(IThreatCampaignRepository repository,
            IThreatIndicatorRepository threatrepository,IUserRepository userRepository, IMapper mapper)
        {
            _repository = repository;
            _userRepo = userRepository;
            _indicatorRepo = threatrepository;
            _mapper = mapper;
        }


        public async Task<IEnumerable<ThreatCampaignReadDto>> GetAllCampaignsAsync()
        {
            try
            {
                var campaigns = await _repository.GetAllAsync();
                return _mapper.Map<IEnumerable<ThreatCampaignReadDto>>(campaigns);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to retrieve threat campaigns at this time.", ex);
            }
        }


        public async Task<ThreatCampaignReadDto?> GetCampaignByIdAsync(int id)
        {
            try
            {
                var campaign = await _repository.GetByIdAsync(id);
                if (campaign == null) return null;

                return _mapper.Map<ThreatCampaignReadDto>(campaign);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while fetching details for Campaign ID {id}.", ex);
            }
        }


        public async Task<ThreatCampaignReadDto> CreateCampaignAsync(ThreatCampaignCreateDto dto)
        {
            try
            {
                var entity = _mapper.Map<ThreatCampaign>(dto);
                var createdEntity = await _repository.AddAsync(entity);
                return _mapper.Map<ThreatCampaignReadDto>(createdEntity);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to start the new threat campaign. Please check the data and try again.", ex);
            }
        }

        public async Task<bool> AssignToUserAsync(int campaignId, int userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
            {
                throw new Exception("The user you are trying to assign does not exist.");
            }

            var validRoles = new[] { "Threat Hunter", "Security Admin", "L2 Analyst" };

            if (!validRoles.Contains(user.Role))
            {
                throw new Exception("Campaigns can only be assigned to senior security personnel.");
            }

            return await _repository.AssignCampaignAsync(campaignId, userId);
        }
        public async Task<bool> UpdateCampaignAsync(int id, ThreatCampaignCreateDto dto)
        {
            try
            {
                var existingCampaign = await _repository.GetByIdAsync(id);
                if (existingCampaign == null) return false;


                _mapper.Map(dto, existingCampaign);

                await _repository.UpdateAsync(existingCampaign);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to update Campaign ID {id}.", ex);
            }
        }


        public async Task<bool> PatchStatusAsync(int id, CampaignStatus status)
        {
            try
            {
                var success = await _repository.UpdateStatusAsync(id, status);
                return success;
            }
            catch (Exception ex)
            {
                throw new Exception($"Could not update the status for Campaign ID {id}.", ex);
            }
        }


        public async Task<bool> DeleteCampaignAsync(int id)
        {
            // 1. Check if Parent exists
            var campaign = await _repository.GetByIdAsync(id);
            if (campaign == null) return false;

            var istNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
                TimeZoneInfo.FindSystemTimeZoneById("India Standard Time"));

            // 2. Cascade: Find and Soft Delete all related Child Indicators
            var indicators = await _indicatorRepo.GetByCampaignIdAsync(id);

            foreach (var indicator in indicators)
            {
                // Only update if not already deleted
                if (!indicator.IsDeleted)
                {
                    indicator.IsDeleted = true;
                    indicator.DeletedAt = istNow;
                    await _indicatorRepo.UpdateAsync(indicator);
                }
            }

            // 3. Soft Delete the Parent Campaign
            campaign.IsDeleted = true;
            campaign.DeletedAt = istNow;
            await _repository.UpdateAsync(campaign);

            return true;
        }





    }
}