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
public class Controller<TEntity, TResponse> : ControllerBase
    where TEntity : Entity
{
    private readonly IService<TEntity>  _service;
    protected readonly IMapper<TEntity, TResponse> _mapper;
    private readonly ILogger _logger;
    public Controller(IService<TEntity> service, IMapper<TEntity, TResponse> mapper, ILogger logger)
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
}
