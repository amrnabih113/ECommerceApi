using ECommerce.Data;
using ECommerce.Interfaces.Repositories;
using ECommerce.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Repositories
{
    public class AddressesRepository : BaseRepository<Address>, IAddressesRepository
    {
        public AddressesRepository(AppDbContext context) : base(context) { }

        public async Task<List<Address>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<Address?> GetByIdAndUserIdAsync(int id, string userId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        }
    }
}
