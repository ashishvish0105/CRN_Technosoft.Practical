using CRN_Technosoft.Application.Command.Items;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Items;

public sealed class DeleteItemHandler(IItemRepository itemRepository)
    : IRequestHandler<DeleteItemCommand, bool>
{
    public Task<bool> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
    {
        return itemRepository.DeleteItem(request.ItemId, cancellationToken);
    }
}
