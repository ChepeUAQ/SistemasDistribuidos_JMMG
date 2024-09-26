using MongoDB.Bson;
using MongoDB.Bson;
using MongoDB.Driver;
using RestApi.Infrastructure.Mongo;
using RestApi.Mappers;
using RestApi.Models;

namespace RestApi.Repositories;

public class GroupRepository : IGroupRepository
{
    private readonly IMongoCollection<GroupEntity> _groups;

    public GroupRepository(IMongoClient mongoClient, IConfiguration configuration) {
        var database = mongoClient.GetDatabase(configuration.GetValue<string>("MongoDb:Groups:DatabaseName"));
        _groups = database.GetCollection<GroupEntity>(configuration.GetValue<string>("MongoDb:Groups:CollectionName"));
    }


    public async Task<GroupModel> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        try {
            var filter = Builders<GroupEntity>.Filter.Eq(x => x.Id, id);
            var group = await _groups.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return group.ToModel();
        } catch(FormatException) {
            return null;
        }
    }

    public async Task<IList<GroupModel>> GetByNameAsync(string name, int pages, int pageSize, string orderBy, CancellationToken cancellationToken)
    {
            var filter = Builders<GroupEntity>.Filter.Regex(x => x.Name, new BsonRegularExpression(name, "i"));

            var query = _groups.Find(filter).Sort(orderBy == "CreatedAt" ? Builders<GroupEntity>.Sort.Descending(e => e.CreatedAt) :
                Builders<GroupEntity>.Sort.Ascending(e => e.Name)
            ).Skip((pages - 1) * pageSize).Limit(pageSize);

            var groups = await query.ToListAsync(cancellationToken);

            return groups.Select(group => group.ToModel()).ToList();
    }
    
    public async Task<GroupModel> GetByExactNameAsync(string name, CancellationToken cancellationToken)
    {
        try {
            var filter = Builders<GroupEntity>.Filter.Eq(x => x.Name, name);
            var group = await _groups.Find(filter).FirstOrDefaultAsync(cancellationToken);

            return group.ToModel();
        } catch(FormatException) {
            return null;
        }
    }

    public async Task DeleteByIdAsync(string id, CancellationToken cancellationToken)
    {
        var filter = Builders<GroupEntity>.Filter.Eq(x => x.Id, id);
        await _groups.DeleteOneAsync(filter, cancellationToken);
    }

    public async Task<GroupModel> CreateAsync(string name, Guid[] users, CancellationToken cancellationToken)
    {
        var group = new GroupEntity {
            Name = name,
            Users = users,
            CreatedAt = DateTime.UtcNow,
            Id = ObjectId.GenerateNewId().ToString()
        };

        await _groups.InsertOneAsync(group, new InsertOneOptions(), cancellationToken);

        return group.ToModel();
    }
}