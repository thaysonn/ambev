using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetAllSales;
using Ambev.DeveloperEvaluation.Application.Sales.GetSaleById;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

[ApiController]
[Route("api/[controller]")]
public class SalesController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSaleCommand command)
    {
        var result = await _mediator.Send(command);
        if (!result.Success)
            return BadRequest(new { errors = result.Errors });
        return CreatedAtAction(nameof(GetById), new { id = result.SaleId }, result);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllSalesQuery());
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetSaleByIdQuery { Id = id });
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSaleCommand command)
    {
        if (id != command.Id)
            return BadRequest("Id mismatch");
        var result = await _mediator.Send(command);
        if (!result.Success) return BadRequest(new { errors = result.Errors });
        return NoContent();
    }

    [HttpPatch("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var result = await _mediator.Send(new CancelSaleCommand { Id = id });
        if (!result.Success) return BadRequest(new { errors = result.Errors });
        return NoContent();
    }
}
