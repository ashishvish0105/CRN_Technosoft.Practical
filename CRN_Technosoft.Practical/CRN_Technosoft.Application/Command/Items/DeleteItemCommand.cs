using MediatR;

namespace CRN_Technosoft.Application.Command.Items;

public record DeleteItemCommand(int ItemId) : IRequest<bool>;
