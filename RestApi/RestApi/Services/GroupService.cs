using System.ComponentModel;
using System.Runtime.CompilerServices;
using RestApi.Exceptions;
using RestApi.Models;
using RestApi.Repositories;
using RestApi.Services;

namespace RestApi.Services;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IUserRepository _userRepository;

    public GroupService (IGroupRepository groupRepository, IUserRepository userRepository) {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task<GroupUserModel> GetGroupByIdAsync(string id, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(id, cancellationToken);

        if (group is null) {
           throw new GroupNotFoundException();
        }
    
        return new GroupUserModel {
            Id = group.Id,
            Name = group.Name,
            CreationDate = group.CreationDate,
            Users = (await Task.WhenAll(
                group.Users.Select(userId => _userRepository.GetByIdAsync(
                    userId, cancellationToken)))).Where(user => user != null)
                    .ToList()
        };
    }

    public async Task<IList<GroupUserModel>> GetGroupByNameAsync(string name, int pages, int pageSize, string orderBy, CancellationToken cancellationToken)
    {
        var groups = await _groupRepository.GetByNameAsync(name, pages, pageSize, orderBy, cancellationToken); 

        if (!groups.Any()) {
            throw new GroupNotFoundException();
        }

        var groupUserModels = await Task.WhenAll(groups.Select(async group => new GroupUserModel{
            Id = group.Id,
            Name = group.Name,
            CreationDate = group.CreationDate,
            Users = (await Task.WhenAll(
                group.Users.Select(async user => await _userRepository.GetByIdAsync(
                    user, cancellationToken))))
                    .ToList()
        }));

        return groupUserModels.ToList();
    }

    public async Task<GroupUserModel> GetGroupByExactNameAsync(string name, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByExactNameAsync(name, cancellationToken);

        if (group is null) {
           throw new GroupNotFoundException();
        }
    
        return new GroupUserModel {
            Id = group.Id,
            Name = group.Name,
            CreationDate = group.CreationDate,
            Users = (await Task.WhenAll(
                group.Users.Select(userId => _userRepository.GetByIdAsync(
                    userId, cancellationToken)))).Where(user => user != null)
                    .ToList()
        };
    }

    public async Task DeleteGroupByIdAsync(string id, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(id, cancellationToken);

        if (group is null) {
            throw new GroupNotFoundException();
        }

        await _groupRepository.DeleteByIdAsync(id, cancellationToken);
    }

    public async Task<GroupUserModel> CreateGroupAsync(string name, Guid[] users, CancellationToken cancellationToken)
    {
        if (users.Length == 0) {
            throw new InvalidGroupRequestFormatException();
        }

        var groups = await _groupRepository.GetByNameAsync(name, 1, 1, "Name", cancellationToken);

        if (groups.Any()) {
            throw new GroupAlreadyExistsException();
        }

        var group = await _groupRepository.CreateAsync(name, users, cancellationToken);

        if (group is null) {
           return null;
        }

        var usersGroup = await Task.WhenAll(users.Select(userId => _userRepository.GetByIdAsync(userId, cancellationToken)));

        if (usersGroup.Any(user => user is null)) {
            throw new UserNotFoundException();
        }
    
        return new GroupUserModel {
            Id = group.Id,
            Name = group.Name,
            CreationDate = group.CreationDate,
            Users = (await Task.WhenAll(
                group.Users.Select(userId => _userRepository.GetByIdAsync(
                    userId, cancellationToken)))).Where(user => user != null)
                    .ToList()
        };
    }

    public async Task UpdateGroupAsync(string id, string name, Guid[] users, CancellationToken cancellationToken)
    {
        if (users.Length == 0) {
            throw new InvalidGroupRequestFormatException();
        }

        var group = await _groupRepository.GetByIdAsync(id, cancellationToken);

        if (group is null) {
            throw new GroupNotFoundException();
        }

        var groups = await _groupRepository.GetByExactNameAsync(name, cancellationToken);

        if (groups is not null && groups.Id != id) {
            throw new GroupAlreadyExistsException();
        }

        var usersGroup = await Task.WhenAll(users.Select(userId => _userRepository.GetByIdAsync(userId, cancellationToken)));

        if (usersGroup.Any(user => user is null)) {
            throw new UserNotFoundException();
        }

        await _groupRepository.UpdateGroupAsync(id, name, users, cancellationToken);
    }
}