using Microsoft.EntityFrameworkCore;
using SecureOps.Application.Interfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Enum;

namespace SecureOPS.Infrastructure.Repositories
{
	public class ThreatCampaignRepository : IThreatCampaignRepository
	{
		private readonly SecureOpsDbContext _context;
		public ThreatCampaignRepository(SecureOpsDbContext context) => _context = context;

		public async Task<IEnumerable<ThreatCampaign>> GetAllAsync() =>
			await _context.ThreatCampaigns.ToListAsync();

		// Fix for CS8603: Added '?' to allow null return if campaign is not found
		public async Task<ThreatCampaign?> GetByIdAsync(int id) =>
			await _context.ThreatCampaigns
				.Include(x => x.AssignedTo) // Eager loading for test consistency
				.FirstOrDefaultAsync(x => x.CampaignId == id && !x.IsDeleted);

		public async Task<bool> AssignCampaignAsync(int campaignId, int userId)
		{
			var campaign = await _context.ThreatCampaigns.FindAsync(campaignId);
			if (campaign is null) return false;

			campaign.AssignedToUserId = userId;

			_context.ThreatCampaigns.Update(campaign);
			return await _context.SaveChangesAsync() > 0;
		}

		public async Task<ThreatCampaign> AddAsync(ThreatCampaign campaign)
		{
			await _context.ThreatCampaigns.AddAsync(campaign);
			await _context.SaveChangesAsync();
			return campaign;
		}

		public async Task<bool> UpdateStatusAsync(int id, CampaignStatus status)
		{
			var campaign = await _context.ThreatCampaigns.FindAsync(id);

			// Fix for CS8602: Explicit null check before dereferencing
			if (campaign is null)
			{
				return false;
			}

			campaign.Status = status;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task UpdateAsync(ThreatCampaign campaign)
		{
			_context.Entry(campaign).State = EntityState.Modified;
			await _context.SaveChangesAsync();
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var campaign = await _context.ThreatCampaigns.FindAsync(id);
			if (campaign is null) return false;

			_context.ThreatCampaigns.Remove(campaign);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}