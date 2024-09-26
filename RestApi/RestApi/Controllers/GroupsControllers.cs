using Microsoft.AspNetCore.Mvc;
using RestApi.Dtos;
using RestApi.Mappers;
using RestApi.Services;
using RestApi.Exceptions;
using System.Net;

namespace RestApi.Controllers;

[ApiController]
[Route("[controller]")]
public class GroupsController : ControllerBase {
    private readonly IGroupService _groupService;

    public GroupsController (IGroupService groupService) {
        _groupService = groupService;
    }

    // localhost:port/groups/28728723
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupResponse>> GetGroupById(string id, CancellationToken cancellationToken) {
        var group = await _groupService.GetGroupByIdAsync(id, cancellationToken);

        if (group is null) {
            return NotFound();
        }

        return Ok(group.ToDto());
    }

//localhost/gorups?name=fjsisjefiesjfij&date=2202023303&var3=2jlisjfs
    [HttpGet("like-name")]
    public async Task<ActionResult<List<GroupResponse>>> GetGroupByName([FromQuery] string name, [FromQuery] int pages, [FromQuery] int pageSize, [FromQuery] string orderBy, CancellationToken cancellationToken) {
        var groups = await _groupService.GetGroupByNameAsync(name, pages, pageSize, orderBy, cancellationToken);

        if (groups is null) {
            return NotFound();
        }

        return Ok(groups.Select(group => group.ToDto()).ToList());
    }

    [HttpGet("exact-name")]
    public async Task<ActionResult<GroupResponse>> GetGroupByExactName(string name, CancellationToken cancellationToken) {
        var group = await _groupService.GetGroupByExactNameAsync(name, cancellationToken);

        if (group is null) {
            return NotFound();
        }

        return Ok(group.ToDto());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteGroup(string id, CancellationToken cancellationToken) {
        try {
            await _groupService.DeleteGroupByIdAsync(id, cancellationToken);
            return NoContent();
        } catch(GroupNotFoundException){
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<GroupResponse>> CreateGroup([FromBody] CreateGroupRequest groupRequest, CancellationToken cancellationToken) {
        try
        {
            var group = await _groupService.CreateGroupAsync(groupRequest.Name, groupRequest.Users, cancellationToken);
            return CreatedAtAction(nameof(GetGroupById), new {id = group.Id}, group.ToDto());
        }
        catch (InvalidGroupRequestFormatException)
        {
            return BadRequest(NewValidationProblemDetails("La cagaste we", HttpStatusCode.BadRequest, new Dictionary<string, string[]>{
                {"Groups", ["User array is empty"]}
            }));
        }
        catch (GroupAlreadyExistsException) 
        {
            return Conflict(NewValidationProblemDetails("La cagaste we", HttpStatusCode.Conflict, new Dictionary<string, string[]>{
                {"Groups", ["Group with same name already exists"]}
            }));
        }
    }

    private static ValidationProblemDetails NewValidationProblemDetails(string title, HttpStatusCode statusCode, Dictionary<string, string[]> errors) {
        return new ValidationProblemDetails {
            Title = title,
            Status = (int) statusCode,
            Errors = errors
        };
    }

}

// localhost/groups
// GET localhost/groups/ID
    // 200 - Retomamos el objeto (bueno)
    // 404 - No existe el objeto
    // 400 - Bad Request (Error del cliente)
// PAGINACIÓN
// GET ALL localhost/groups?name=7678isds
    // 200 - retornar el listado de objetos (bueno)
    // 200 - retomar listado vacío
    // 400 - Algún campo del query parameter es inválido
// DELETE localhost/groups/Id
    // 404 - No existe el recurso
    // 204 - No content
// POST localhost/groups {koskfire}
    // 400 - Bad request
    // 409 - Conflict
    // 200 - Respuesta del objeto actualizado (bueno)
    // 204