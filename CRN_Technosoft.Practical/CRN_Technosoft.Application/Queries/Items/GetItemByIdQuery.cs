using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Queries.Items;

public record GetItemByIdQuery(int ItemId) : IRequest<ItemDTO?>;
