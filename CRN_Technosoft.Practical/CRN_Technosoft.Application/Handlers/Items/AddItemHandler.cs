using AutoMapper;
using CRN_Technosoft.Application.Command.Items;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Items;

public sealed class AddItemHandler(IItemRepository itemRepository, IMapper mapper)
    : IRequestHandler<AddItemCommand, ItemDTO>
{
    public async Task<ItemDTO> Handle(AddItemCommand request, CancellationToken cancellationToken)
    {
        var item = mapper.Map<ItemEntity>(request.Item);
        var savedItem = await itemRepository.AddItem(item, cancellationToken);
        return mapper.Map<ItemDTO>(savedItem);
    }
}
