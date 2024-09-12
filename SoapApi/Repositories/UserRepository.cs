using Microsoft.EntityFrameworkCore;
using SoapApi.Infrastructure;
using SoapApi.Models;
using SoapApi.Mappers;
using System.ServiceModel;


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

    public async Task DeleteByIdAsync(UserModel user, CancellationToken cancellationToken)
    {   
        var userEntity = user.ToEntity();
        
        _dbContext.Users.Remove(userEntity);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken)
    {
        var userEntity = user.ToEntity();
        userEntity.Id = Guid.NewGuid();
        await _dbContext.AddAsync(userEntity, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return userEntity.ToModel();
    }

    public async Task<UserModel> UpdateAsync(UserModel user, CancellationToken cancellationToken)
    {
        var userEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

        if (userEntity is null) {
            throw new FaultException("User not found");
        }

        userEntity.FirstName = user.FirstName;        
        userEntity.LastName = user.LastName;        
        userEntity.Birthday = user.BirthDate;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return userEntity.ToModel();
    }

    // public async Task<UserModel> CreateAsync(UserModel user, CancellationToken cancellationToken)
    // {
    //     user.Id = Guid.NewGuid();
    //     await _dbContext.AddAsync(user.ToEntity(), cancellationToken);

    //     await _dbContext.SaveChangesAsync(cancellationToken);

    //     return user;
    // }

}