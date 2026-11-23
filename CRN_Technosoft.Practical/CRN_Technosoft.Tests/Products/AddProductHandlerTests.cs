using AutoMapper;
using CRN_Technosoft.Application.Command.Products;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Handlers.Products;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using Moq;
using Xunit;

namespace CRN_Technosoft.Tests.Products;

public sealed class AddProductHandlerTests
{
    [Fact]
    public async Task Handle_SavesCreatorAndReturnsPersistedProduct()
    {
        var productRepository = new Mock<IProductRepository>();
        var mapper = new Mock<IMapper>();
        var requestDto = new CreateProductDTO { ProductName = "Keyboard" };
        var savedProduct = new ProductEntity
        {
            Id = 42,
            ProductName = "Keyboard",
            CreatedBy = "sample-user"
        };
        var responseDto = new GetProductDTO
        {
            Id = 42,
            ProductName = "Keyboard",
            CreatedBy = "sample-user"
        };

        mapper.Setup(item => item.Map<ProductEntity>(requestDto))
            .Returns(new ProductEntity { ProductName = "Keyboard", CreatedBy = string.Empty });
        productRepository.Setup(repository => repository.AddProduct(
                It.IsAny<ProductEntity>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(savedProduct);
        mapper.Setup(item => item.Map<GetProductDTO>(savedProduct))
            .Returns(responseDto);

        var handler = new AddProductHandler(productRepository.Object, mapper.Object);
        var result = await handler.Handle(
            new AddProductCommand(requestDto, "sample-user"),
            CancellationToken.None);

        Assert.Equal(42, result.Id);
        Assert.Equal("Keyboard", result.ProductName);
        productRepository.Verify(repository => repository.AddProduct(
            It.Is<ProductEntity>(product => product.CreatedBy == "sample-user"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
