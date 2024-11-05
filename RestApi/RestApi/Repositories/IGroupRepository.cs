using System.Text.RegularExpressions;
using RestApi.Models;

namespace RestApi.Repositories;

public interface IGroupRepository {
    Task<GroupModel> GetByIdAsync(string id, CancellationToken cancellationToken);
    Task<IList<GroupModel>> GetByNameAsync(string name, int pages, int pageSize, string orderBy, CancellationToken cancellationToken);
    Task<GroupModel> GetByExactNameAsync(string name, CancellationToken cancellationToken);
    Task DeleteByIdAsync(string id, CancellationToken cancellationToken);
    Task<GroupModel> CreateAsync(string name, Guid[] users, CancellationToken cancellationToken);
    Task UpdateGroupAsync(string id, string name, Guid[] users, CancellationToken cancellationToken);
}