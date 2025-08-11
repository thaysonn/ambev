using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ambev.DeveloperEvaluation.Application.SalesItem.CreateSalesItem;
using Ambev.DeveloperEvaluation.Application.SalesItem.UpdateSalesItem;
using Ambev.DeveloperEvaluation.Application.SalesItem.RemoveSalesItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales
{
    /// <summary>
    /// Controller for managing sale items operations
    /// </summary>
    [ApiController]
    [Route("api/sales/{saleId}/items")]
    public class SalesItemController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of SalesItemController
        /// </summary>
        /// <param name="mediator">The mediator instance</param>
        public SalesItemController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Adds an item to a sale
        /// </summary>
        /// <param name="saleId">The unique identifier of the sale</param>
        /// <param name="command">The add sale item request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>The created sale item details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AddSaleItemResult), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddItem([FromRoute] Guid saleId, [FromBody] AddSaleItemCommand command, CancellationToken cancellationToken)
        {
            command.SaleId = saleId;
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success)
                return BadRequest(result.Errors);
            return CreatedAtAction(nameof(AddItem), new { saleId, itemId = result.ItemId }, result);
        }

        /// <summary>
        /// Updates an item in a sale
        /// </summary>
        /// <param name="saleId">The unique identifier of the sale</param>
        /// <param name="itemId">The unique identifier of the sale item</param>
        /// <param name="command">The update sale item request</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        [HttpPatch("{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateItem([FromRoute] Guid saleId, [FromRoute] Guid itemId, [FromBody] UpdateSaleItemCommand command, CancellationToken cancellationToken)
        {
            command.SaleId = saleId;
            command.ItemId = itemId;
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success)
                return BadRequest(result.Errors);
            return NoContent();
        }

        /// <summary>
        /// Removes an item from a sale
        /// </summary>
        /// <param name="saleId">The unique identifier of the sale</param>
        /// <param name="itemId">The unique identifier of the sale item</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(string[]), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RemoveItem([FromRoute] Guid saleId, [FromRoute] Guid itemId, CancellationToken cancellationToken)
        {
            var command = new RemoveSaleItemCommand { SaleId = saleId, ItemId = itemId };
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.Success)
                return BadRequest(result.Errors);
            return NoContent();
        }
    }
}
