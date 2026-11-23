using CRN_Technosoft.Domain.Entities;

namespace CRN_Technosoft.Domain.IRepositories
{
    public interface IUserRepository
    {
        Task<int> AddUser(UserEntity addUser, CancellationToken cancellationToken);
        Task<int> EditUser(UserEntity addUser, CancellationToken cancellationToken);
        Task<List<UserEntity>> GetUserList(int pageIndex, int pageSize, CancellationToken cancellationToken);
        Task<UserEntity?> GetUserById(int userId, CancellationToken cancellationToken);
    }
}
