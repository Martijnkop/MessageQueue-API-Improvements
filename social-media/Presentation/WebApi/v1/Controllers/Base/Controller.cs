using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Social.Business.Abstractions.Services.Base;
using Social.Domain.Models.Base;
using Social.WebApi.Abstractions;

namespace Social.WebApi.v1.Controllers.Base;

[ApiController]
[Route("[controller]s")]
public class Controller<TEntity, TResponse, TCreateRequest, TEditRequest, TCreateDTO, TEditDTO> : ControllerBase
    where TEntity : Entity
{
    private readonly IService<TEntity, TCreateDTO, TEditDTO>  _service;
    private readonly IMapper<TEntity, TResponse, TCreateRequest, TCreateDTO, TEditRequest, TEditDTO> _mapper;
    private readonly ILogger _logger;
    public Controller(IService<TEntity, TCreateDTO, TEditDTO> service, IMapper<TEntity, TResponse, TCreateRequest, TCreateDTO, TEditRequest, TEditDTO> mapper, ILogger logger)
    {
        _service = service;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TEntity>>> GetAllAsync(CancellationToken ct)
    {
        var entities = await _service.GetAllAsync(ct);
        return Ok(entities);
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
    public virtual async Task<ActionResult<TResponse>> CreateAsync([FromBody] TCreateRequest createModel, CancellationToken ct)
    {
        var entity = await _service.CreateAsync(_mapper.Map(createModel), ct);
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
