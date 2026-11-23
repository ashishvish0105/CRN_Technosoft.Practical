using AutoMapper;
using CRN_Technosoft.Application.Command.Items;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Items;

public sealed class EditItemHandler(IItemRepository itemRepository, IMapper mapper)
    : IRequestHandler<EditItemCommand, bool>
{
    public Task<bool> Handle(EditItemCommand request, CancellationToken cancellationToken)
    {
        var item = mapper.Map<ItemEntity>(request.Item);
        item.Id = request.ItemId;

        return itemRepository.EditItem(request.ItemId, item, cancellationToken);
    }
}
