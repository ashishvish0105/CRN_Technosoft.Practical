using AutoMapper;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Items;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Items;

public sealed class GetItemByIdHandler(IItemRepository itemRepository, IMapper mapper)
    : IRequestHandler<GetItemByIdQuery, ItemDTO?>
{
    public async Task<ItemDTO?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var item = await itemRepository.GetItemById(request.ItemId, cancellationToken);
        return item is null ? null : mapper.Map<ItemDTO>(item);
    }
}
