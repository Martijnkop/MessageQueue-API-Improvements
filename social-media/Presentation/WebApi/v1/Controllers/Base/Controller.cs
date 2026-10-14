using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services.Base;
using Social.Domain.Models;
using Social.Domain.Models.Base;
using Social.WebApi.Abstractions;
using System.Security.Claims;

namespace Social.WebApi.v1.Controllers.Base;

[ApiController]
[Route("[controller]s")]
public class Controller<TEntity, TResponse, TCreateRequest, TEditRequest, TCreateDTO, TEditDTO> : ControllerBase
    where TEntity : Entity
{
    private readonly IService<TEntity, TCreateDTO, TEditDTO>  _service;
    protected readonly IMapper<TEntity, TResponse, TCreateRequest, TCreateDTO, TEditRequest, TEditDTO> _mapper;
    private readonly ILogger _logger;
    public Controller(IService<TEntity, TCreateDTO, TEditDTO> service, IMapper<TEntity, TResponse, TCreateRequest, TCreateDTO, TEditRequest, TEditDTO> mapper, ILogger logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TResponse>>> GetAllAsync(CancellationToken ct)
    {
        var entities = await _service.GetAllAsync(ct);
        return Ok(entities.Select(_mapper.Map));
    }

    [HttpGet]
    [Route("{id}")]
    public virtual async Task<ActionResult<TResponse>> GetByIdAsync(Guid id, CancellationToken ct)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity == null)
        {
            _logger.LogDebug($"Entity of type {typeof(TEntity).Name} with Id: {id} not found");
            return NotFound();
        }
        return Ok(_mapper.Map(entity));
    }


    [HttpPost]
    [Authorize]
    // Big chance this base implementation is useless lol
    public virtual async Task<ActionResult<TResponse>> CreateAsync([FromBody] TCreateRequest createModel, CancellationToken ct)
    {
        var claimsUser = HttpContext.User;
        
        var sub = claimsUser.FindFirstValue("sub")
              ?? claimsUser.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(sub, out var keycloakId)) throw new NotImplementedException(); // This should not be possible, as we use the same validation in the middleware

        var entity = await _service.CreateAsync(_mapper.Map(createModel), keycloakId, ct);
        return CreatedAtAction(nameof(GetByIdAsync), new { id = entity.Id }, _mapper.Map(entity));
    }

    [HttpPut]
    [Route("{id}")]
    public virtual async Task<ActionResult<TResponse>> EditAsync(Guid id, [FromBody] TEditRequest editModel, CancellationToken ct)
    {
        var entity = await _service.EditAsync(id, _mapper.Map(editModel), ct);
        return Ok(_mapper.Map(entity));
    }

    [HttpDelete]
    [Route("{id}")]
    public virtual async Task<ActionResult> DeleteAsync(Guid id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return NoContent();
    }
}
