using CRN_Technosoft.Domain.Entities;
using CRN_Technosoft.Domain.IRepositories;
using CRN_Technosoft.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRN_Technosoft.Infrastructure.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public UserRepository(ApplicationDbContext dbContext, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            this._dbContext = dbContext;
            this._passwordHasher = passwordHasher;
        }
        public async Task<int> AddUser(
     UserEntity addUser,
     CancellationToken cancellationToken)
        {
            if (addUser == null)
                throw new ArgumentNullException(nameof(addUser));

            if (string.IsNullOrWhiteSpace(addUser.UserName)
                || string.IsNullOrWhiteSpace(addUser.Email)
                || string.IsNullOrWhiteSpace(addUser.Password))
            {
                throw new ArgumentException(
                    "User name, email, and password are required.",
                    nameof(addUser));
            }

            var userName = addUser.UserName;
            var email = addUser.Email;

            if (await _dbContext.Users.AnyAsync(
                u => u.Email == email, cancellationToken))
            {
                throw new InvalidOperationException(
                    "A user with this email address already exists.");
            }

            var addUserDTO = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                NormalizedUserName = userName.ToUpperInvariant(),
                NormalizedEmail = email.ToUpperInvariant(),
                FirstName = addUser.FirstName,
                LastName = addUser.LastName,
                IsActive = addUser.IsActive,
                CreatedAt = DateTime.UtcNow
            };

            addUserDTO.PasswordHash =
                _passwordHasher.HashPassword(addUserDTO, addUser.Password);

            var role = await _dbContext.Roles.FirstOrDefaultAsync(
                existingRole => existingRole.NormalizedName == "CUSTOMER",
                cancellationToken);

            if (role == null)
            {
                throw new InvalidOperationException("Customer role is not configured.");
            }

            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            _dbContext.Users.Add(addUserDTO);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _dbContext.UserRoles.Add(new IdentityUserRole<int>
            {
                UserId = addUserDTO.Id,
                RoleId = role.Id,
            });

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return addUserDTO.Id;
        }

        public async Task<int> EditUser(UserEntity editUser, CancellationToken cancellationToken)
        {
            if (editUser == null || editUser.Id == 0)
            {
                throw new ArgumentNullException(nameof(editUser));
            }
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == editUser.Id, cancellationToken);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found.");
            }

            user.UserName = editUser.UserName;
            user.Email = editUser.Email;
            user.FirstName = editUser.FirstName;
            user.LastName = editUser.LastName;
            user.IsActive = editUser.IsActive;

            await _dbContext.SaveChangesAsync(cancellationToken);
            return editUser.Id;
        }

        public async Task<List<UserEntity>> GetUserList(
            int pageIndex,
            int pageSize,
            CancellationToken cancellationToken)
        {
            var safePageIndex = Math.Max(pageIndex, 1);
            var safePageSize = Math.Clamp(pageSize, 1, 100);
            var skip = (int)Math.Min((long)(safePageIndex - 1) * safePageSize, int.MaxValue);

            var users = await _dbContext.Users
                     .AsNoTracking()
                     .OrderBy(user => user.Id)
                     .Skip(skip)
                     .Take(safePageSize)
                     .Select(u => new UserEntity
                     {
                         Id = u.Id,
                         UserName = u.UserName,
                         Email = u.Email,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         IsActive = u.IsActive
                     })
                     .ToListAsync(cancellationToken);

            return users;
        }

        public async Task<UserEntity?> GetUserById(int userId, CancellationToken cancellationToken)
        {
            return await _dbContext.Users
                     .Where(x => x.Id == userId)
                     .Select(u => new UserEntity
                     {
                         Id = u.Id,
                         UserName = u.UserName,
                         Email = u.Email,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         IsActive = u.IsActive
                     }).FirstOrDefaultAsync(cancellationToken);
        }
    }
}
