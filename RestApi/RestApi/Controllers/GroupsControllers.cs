using Microsoft.AspNetCore.Mvc;
using RestApi.Dtos;
using RestApi.Mappers;
using RestApi.Services;

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
    [HttpGet]
    public async Task<ActionResult<List<GroupResponse>>> GetGroupByName([FromQuery] string name, [FromQuery] int pages, [FromQuery] int pageSize, [FromQuery] string orderBy, CancellationToken cancellationToken) {
        var groups = await _groupService.GetGroupByNameAsync(name, pages, pageSize, orderBy, cancellationToken);

        if (groups is null) {
            return NotFound();
        }

        return Ok(groups.Select(group => group.ToDto()).ToList());
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