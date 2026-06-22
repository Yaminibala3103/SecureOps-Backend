using Microsoft.EntityFrameworkCore;
using SecureOps.Applications.Interfaces;
using SecureOPS.Domain.Data;
using SecureOPS.Domain.Entities;
using SecureOPS.Domain.Enum;

public class AssetRepository : IAssetRepository
{
    private readonly SecureOpsDbContext _context;
    public AssetRepository(SecureOpsDbContext context) => _context = context;

    public async Task<IEnumerable<Asset>> GetAllAsync() =>
        //await _context.Assets.ToListAsync();
        await _context.Assets.Where(a => !a.IsDeleted).ToListAsync();

    public async Task<Asset?> GetByIdAsync(int id) =>
        //await _context.Assets.FindAsync(id);
        await _context.Assets.FirstOrDefaultAsync(a => a.AssetId == id && !a.IsDeleted);

    public async Task<IEnumerable<Asset>> GetByCriticalityAsync(AssetCriticality criticality)
    {
        return await _context.Assets
            .Where(a => a.Criticality == criticality)
            .ToListAsync();
    }

    public async Task<Asset> AddAsync(Asset asset)
    {
        await _context.Assets.AddAsync(asset);
        await _context.SaveChangesAsync();
        return asset;
    }

    public async Task UpdateAsync(Asset asset)
    {
        _context.Entry(asset).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
    // paging 
    public async Task<(List<Asset> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize)
    {
        var query = _context.Assets.Where(a => !a.IsDeleted);

        // 1. Get total count before skipping/taking (for the UI)
        int totalCount = await query.CountAsync();

        // 2. Get the specific page of data
        var items = await query
            .OrderBy(a => a.AssetId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    //public async Task<bool> DeleteAsync(int id)
    //{
    //    var asset = await _context.Assets.FindAsync(id);
    //    if (asset == null) return false;

    //    asset.IsDeleted = true;
    //    asset.DeletedAt = DateTime.UtcNow;
    //    await _context.SaveChangesAsync();
    //    return true;
    //}


    public async Task<bool> DeleteAsync(int id)
    {
        var asset = await _context.Assets.FindAsync(id);
        if (asset != null)
        {
            _context.Assets.Remove(asset);
            await _context.SaveChangesAsync();
            return true;
        }
        return false;
    }
    public async Task<bool> ExistsAsync(string hostname, string ip) =>
        await _context.Assets.AnyAsync(a => a.HostName == hostname || a.Ipaddress == ip);
}