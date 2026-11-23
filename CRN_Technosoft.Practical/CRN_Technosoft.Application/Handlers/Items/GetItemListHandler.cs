using AutoMapper;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Items;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Items;

public sealed class GetItemListHandler(IItemRepository itemRepository, IMapper mapper)
    : IRequestHandler<GetItemListQuery, List<ItemDTO>>
{
    public async Task<List<ItemDTO>> Handle(GetItemListQuery request, CancellationToken cancellationToken)
    {
        var items = await itemRepository.GetItemList(request.ProductId, cancellationToken);
        return mapper.Map<List<ItemDTO>>(items);
    }
}
