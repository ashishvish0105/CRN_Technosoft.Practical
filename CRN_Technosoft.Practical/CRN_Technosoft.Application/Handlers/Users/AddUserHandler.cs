using AutoMapper;
using CRN_Technosoft.Application.Command.Users;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;
namespace CRN_Technosoft.Application.Handlers.Users
{
    public class AddUserHandler : IRequestHandler<AddUserCommand, int>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public AddUserHandler(IUserRepository userRepository, IMapper mapper) 
        {  
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<int> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var user = _mapper.Map<UserEntity>(request.addUserDTO);
            return await _userRepository.AddUser(user, cancellationToken);
        }
    }
}
