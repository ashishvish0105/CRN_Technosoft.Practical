using CRN_Technosoft.Application.Command.Users;
using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using MediatR;


namespace CRN_Technosoft.Application.Handlers.Users
{
    public class EditUserHandler:IRequestHandler<EditUserCommand, int>
    {
        private readonly IUserRepository _userRepository;

        public EditUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<int> Handle(EditUserCommand request, CancellationToken cancellationToken)
        {
            var userEntity = new UserEntity
            {
                Id = request.userId,
                UserName = request.editUserDTO.UserName,
                Email = request.editUserDTO.Email,
            };

            return await _userRepository.EditUser(userEntity, cancellationToken);
        }
    }
}