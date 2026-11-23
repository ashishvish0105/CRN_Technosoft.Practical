using CRN_Technosoft.Application.Command.Items;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Products;
using CRN_Technosoft.Application.Queries.Items;
using CRN_Technosoft.Domain.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CRN_Technosoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ItemsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ApiResponseModel>> GetItemList(
        [FromQuery, Range(1, int.MaxValue)] int productId,
        [FromQuery, Range(1, int.MaxValue)] int pageIndex = 1,
        [FromQuery, Range(1, 100)] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var items = await mediator.Send(
            new GetItemListQuery(productId, pageIndex, pageSize),
            cancellationToken);
        return Ok(new ApiResponseModel(
            "Items retrieved successfully",
            StatusCodes.Status200OK,
            items));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseModel>> GetItemById(
        [FromRoute, Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var item = await mediator.Send(new GetItemByIdQuery(id), cancellationToken);
        if (item is null)
        {
            return NotFound(new ApiResponseModel(
                "Item not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        return Ok(new ApiResponseModel(
            "Item retrieved successfully",
            StatusCodes.Status200OK,
            item));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,ProductManager")]
    public async Task<ActionResult<ApiResponseModel>> AddItem(
        [FromBody] CreateItemDTO request,
        CancellationToken cancellationToken)
    {
        var product = await mediator.Send(new GetProductByIdQuery(request.ProductId), cancellationToken);
        if (product is null)
        {
            return NotFound(new ApiResponseModel(
                "Product not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        var item = await mediator.Send(new AddItemCommand(request), cancellationToken);
        var response = new ApiResponseModel(
            "Item added successfully",
            StatusCodes.Status201Created,
            item);

        return CreatedAtAction(nameof(GetItemById), new { id = item.Id }, response);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,ProductManager")]
    public async Task<ActionResult<ApiResponseModel>> EditItem(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] EditItemDTO request,
        CancellationToken cancellationToken)
    {
        var product = await mediator.Send(new GetProductByIdQuery(request.ProductId), cancellationToken);
        if (product is null)
        {
            return NotFound(new ApiResponseModel(
                "Product not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        var updated = await mediator.Send(new EditItemCommand(id, request), cancellationToken);
        if (!updated)
        {
            return NotFound(new ApiResponseModel(
                "Item not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        return Ok(new ApiResponseModel(
            "Item updated successfully",
            StatusCodes.Status200OK,
            new ItemDTO { Id = id, ProductId = request.ProductId, Quantity = request.Quantity }));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,ProductManager")]
    public async Task<ActionResult<ApiResponseModel>> DeleteItem(
        [FromRoute, Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var deleted = await mediator.Send(new DeleteItemCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ApiResponseModel(
                "Item not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        return Ok(new ApiResponseModel(
            "Item deleted successfully",
            StatusCodes.Status200OK,
            true));
    }
}
