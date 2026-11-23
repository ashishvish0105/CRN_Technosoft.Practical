

using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Users;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;

namespace CRN_Technosoft.Application.Handlers.Users
{
    public class GetUserHandler : IRequestHandler<GetUserQuery, GetUserDTO?>
    {
        private readonly IUserRepository _userRepository;

        public GetUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<GetUserDTO?> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var user = await _userRepository.GetUserById(request.getUserDTO.Id, cancellationToken);       
            if (user == null)
            {
                return null;
            }
            return  new GetUserDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive
            };
        }
    }
}