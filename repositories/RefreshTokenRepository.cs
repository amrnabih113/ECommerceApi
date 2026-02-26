using System.Security.Cryptography;
using System.Text;
using ECommerce.Data;
using ECommerce.interfaces;
using ECommerce.models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(RefreshToken token)
        {
            // Hash the token before saving
            token.TokenHash = HashToken(token.TokenHash);
            await _context.RefreshTokens.AddAsync(token);
        }

        public async Task<RefreshToken?> GetByHashAsync(string hash)
        {
            var hashed = HashToken(hash);
            return await _context
                .RefreshTokens.Include(x => x.User)
                .FirstOrDefaultAsync(x => x.TokenHash == hashed);
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();

        // Static method to hash tokens consistently
        public static string HashToken(string token) =>
            Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    }
}
