using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Items;

public record GetItemListQuery(int ProductId, int PageIndex = 1, int PageSize = 10) : IRequest<List<ItemDTO>>;
