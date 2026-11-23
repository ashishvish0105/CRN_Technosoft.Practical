using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Users;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;


namespace CRN_Technosoft.Application.Handlers.Users
{
    public class GetUserListHandler:IRequestHandler<GetUserListQuery, List<GetUserDTO>>
    {

        private readonly IUserRepository _userRepository;

        public GetUserListHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<List<GetUserDTO>>Handle(GetUserListQuery request, CancellationToken cancellationToken)
        {
            // Implement the logic to retrieve the user based on the request.UserId
            // For example, you might query a database or call a service to get the user data.
            // Here, we'll return a dummy user for demonstration purposes.
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userRepository.GetUserList(
                request.PageIndex,
                request.PageSize,
                cancellationToken);
            return user.Select(u => new GetUserDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.IsActive
            }).ToList();
        }
    }
}
