using Microsoft.EntityFrameworkCore;
using SoapApi.Infrastructure;
using SoapApi.Models;
using SoapApi.Mappers;

namespace SoapApi.Repositories;

public class UserRespository : IUserRepository {
    private readonly RelationalDbContext _dbContext;
    public UserRespository(RelationalDbContext dbContext) {
        _dbContext = dbContext;
    }
    public async Task<UserModel> GetByIdAsync(Guid id, CancellationToken cancellationToken) {
        var user = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
        return user.ToModel();
    }

    public async Task<IList<UserModel>> GetAllAsync(CancellationToken cancellationToken) {
        var users = await _dbContext.Users.AsNoTracking().ToListAsync(cancellationToken);
        return users.Select(user => user.ToModel()).ToList();
    }

    public async Task<IList<UserModel>> GetAllByEmailAsync(string email, CancellationToken cancellationToken) {
        var users = await _dbContext.Users.AsNoTracking().Where(u => EF.Functions.Like(u.Email, $"%{email}%")).ToListAsync(cancellationToken);
        return users.Select(user => user.ToModel()).ToList();
    }
}