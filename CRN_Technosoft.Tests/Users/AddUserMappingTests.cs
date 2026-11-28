using AutoMapper;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Mapping;
using CRN_Technosoft.Domain.Entities;
using Xunit;

namespace CRN_Technosoft.Tests.Users;

public sealed class AddUserMappingTests
{
    [Fact]
    public void MapsRegistrationFieldsToUserEntity()
    {
        var configuration = new MapperConfiguration(
            options => options.AddProfile<AutoMapperProfile>());
        var mapper = configuration.CreateMapper();
        var request = new AddUserDTO
        {
            UserName = "ashish",
            Email = "user@example.com",
            Password = "Ashish@1111"
        };

        var user = mapper.Map<UserEntity>(request);

        Assert.Equal(request.UserName, user.UserName);
        Assert.Equal(request.Email, user.Email);
        Assert.Equal(request.Password, user.Password);
    }
}
