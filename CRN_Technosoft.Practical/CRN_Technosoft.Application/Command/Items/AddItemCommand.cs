using CRN_Technosoft.Application.DTO;
using MediatR;

namespace CRN_Technosoft.Application.Command.Items;

public record AddItemCommand(CreateItemDTO Item) : IRequest<ItemDTO>;
