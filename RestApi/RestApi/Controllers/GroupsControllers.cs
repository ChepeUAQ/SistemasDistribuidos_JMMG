using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RestApi.Dtos;
using RestApi.Mappers;
using RestApi.Services;
using RestApi.Exceptions;
using System.Net;

namespace RestApi.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class GroupsController : ControllerBase {
    private readonly IGroupService _groupService;

    public GroupsController (IGroupService groupService) {
        _groupService = groupService;
    }

    // localhost:port/groups/28728723
    [HttpGet("{id}")]
    public async Task<ActionResult<GroupResponse>> GetGroupById(string id, CancellationToken cancellationToken) {
        try
        {
            var group = await _groupService.GetGroupByIdAsync(id, cancellationToken);
            return Ok(group.ToDto());
        }
        catch (GroupNotFoundException)
        {
            return NotFound();
        }
    }

//localhost/gorups?name=fjsisjefiesjfij&date=2202023303&var3=2jlisjfs
    [HttpGet]
    public async Task<ActionResult<List<GroupResponse>>> GetGroupByName([FromQuery] string name, [FromQuery] int pages, [FromQuery] int pageSize, [FromQuery] string orderBy, CancellationToken cancellationToken) {
        try {
            var groups = await _groupService.GetGroupByNameAsync(name, pages, pageSize, orderBy, cancellationToken);
            return Ok(groups.Select(group => group.ToDto()).ToList());
        } catch (GroupNotFoundException) {
            return NotFound();
        }

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
        catch(UserNotFoundException)
        {
            return NotFound(NewValidationProblemDetails("Error", HttpStatusCode.NotFound, new Dictionary<string, string[]>{
                {"Users", ["User not found"]}
            }));
        }
        catch (InvalidGroupRequestFormatException)
        {
            return BadRequest(NewValidationProblemDetails("Error", HttpStatusCode.BadRequest, new Dictionary<string, string[]>{
                {"Groups", ["User array is empty"]}
            }));
        }
        catch (GroupAlreadyExistsException) 
        {
            return Conflict(NewValidationProblemDetails("Error", HttpStatusCode.Conflict, new Dictionary<string, string[]>{
                {"Groups", ["Group with same name already exists"]}
            }));
        }
    }

    // localhost:8080/groups/sioaud90
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateGroup(string id, [FromBody] UpdateGroupRequest groupRequest, CancellationToken cancellationToken) {
        try
        {
            await _groupService.UpdateGroupAsync(id, groupRequest.Name, groupRequest.Users, cancellationToken);
            return NoContent();
        }
        catch (UserNotFoundException) {
            return NotFound(NewValidationProblemDetails("Error", HttpStatusCode.NotFound, new Dictionary<string, string[]>{
                {"Users", ["User not found"]}
            }));
        }
        catch (GroupNotFoundException)
        {
            return NotFound();
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