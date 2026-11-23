using CRN_Technosoft.Domain.Common.Models;
using CRN_Technosoft.Application.Command.Products;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CRN_Technosoft.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponseModel>> GetProductList([FromQuery] int pageIndex, int totalLength,CancellationToken cancellationToken)
    {
        var products = await _mediator.Send(new GetProductListQuery(pageIndex > 0 ? pageIndex : 1, totalLength > 0 ? totalLength : 10), cancellationToken);
        return Ok(new ApiResponseModel(
            "Products retrieved successfully",
            StatusCodes.Status200OK,
            products));
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseModel>> GetProductById(
        [FromRoute, Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(id), cancellationToken);
        if (product is null)
        {
            return NotFound(new ApiResponseModel(
                "Product not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        return Ok(new ApiResponseModel(
            "Product retrieved successfully",
            StatusCodes.Status200OK,
            product));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,ProductManager")]
    public async Task<ActionResult<ApiResponseModel>> AddProduct(
        [FromBody] CreateProductDTO request,
        CancellationToken cancellationToken)
    {
        var product = await _mediator.Send(new AddProductCommand(request), cancellationToken);
        return StatusCode(
            StatusCodes.Status201Created,
            new ApiResponseModel(
                "Product added successfully",
                StatusCodes.Status201Created,
                product));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,ProductManager")]
    public async Task<ActionResult<ApiResponseModel>> EditProduct(
        [FromRoute, Range(1, int.MaxValue)] int id,
        [FromBody] EditProductDTO request,
        CancellationToken cancellationToken)
    {
        var updated = await _mediator.Send(new EditProductCommand(id, request), cancellationToken);
        if (!updated)
        {
            return NotFound(new ApiResponseModel(
                "Product not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        return Ok(new ApiResponseModel(
            "Product updated successfully",
            StatusCodes.Status200OK,
            request));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,ProductManager")]
    public async Task<ActionResult<ApiResponseModel>> DeleteProduct(
        [FromRoute, Range(1, int.MaxValue)] int id,
        CancellationToken cancellationToken)
    {
        var deleted = await _mediator.Send(new DeleteProductCommand(id), cancellationToken);
        if (!deleted)
        {
            return NotFound(new ApiResponseModel(
                "Product not found.",
                StatusCodes.Status404NotFound,
                string.Empty));
        }

        return Ok(new ApiResponseModel(
            "Product deleted successfully",
            StatusCodes.Status200OK,
            true));
    }
}
