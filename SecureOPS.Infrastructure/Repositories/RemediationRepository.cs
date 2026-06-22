using Microsoft.EntityFrameworkCore;

using SecureOps.Application.Interfaces;
using SecureOps.Applications.DTOs;
using SecureOps.Domain.Interfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
// Assume your DbContext is here

namespace SecureOps.Infrastructure.Repositories;

public class RemediationRepository : IRemediationRepository
{
    private readonly SecureOpsDbContext _context;
    public RemediationRepository(SecureOpsDbContext context) => _context = context;

    public async Task<IEnumerable<Remediation>> GetAllActiveAsync()
    {
        return await _context.Remediations
            .Include(r => r.Vulnerability)
            .Include(r => r.Asset)
            .Where(r => !r.IsDeleted)
            .ToListAsync();
    }

    public async Task<Remediation?> GetByIdAsync(int id)
    {
        return await _context.Remediations
            .Include(r => r.Vulnerability)
            .Include(r => r.Asset)
            .FirstOrDefaultAsync(r => r.RemediationId == id && !r.IsDeleted);
    }
    public async Task<List<Remediation>> GetByAssetIdAsync(int assetId)
    {
        return await _context.Remediations
            .Where(v => v.AssetId == assetId && !v.IsDeleted)
            .ToListAsync();
    }
    public async Task<Remediation> AddAsync(Remediation entity)
    {
        _context.Remediations.Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task<IEnumerable<Remediation>> GetByVulnerabilityIdAsync(int vulnerabilityId)
    {
        // Returns active tasks for the specific Vulnerability ID
        return await _context.Remediations
            .Where(r => r.VulnerabilityId == vulnerabilityId && !r.IsDeleted)
            .ToListAsync();
    }
    public async Task UpdateAsync(Remediation entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}