using Microsoft.EntityFrameworkCore;
using Mille.Domain.Entities;
using Mille.Domain.Interfaces;
using Mille.Infrastructure.Persistence.Data;

namespace Mille.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext context) => _context = context;


        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct)
            => await _context.Users
                .AsNoTracking()
                .Include(u => u.Addresses)
                .Where(u => !u.IsDeleted)
                .ToListAsync(ct);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
            => await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted, ct);

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
            => await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted, ct);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
            => await _context.Users
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && !u.IsDeleted, ct);


        public void Add(User entity) => _context.Add(entity);
        public void Update(User entity) => _context.Update(entity);
        public void Remove(User entity) => _context.Remove(entity);
    }
}
