using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Items;

public record GetItemListQuery(int ProductId) : IRequest<List<ItemDTO>>;
