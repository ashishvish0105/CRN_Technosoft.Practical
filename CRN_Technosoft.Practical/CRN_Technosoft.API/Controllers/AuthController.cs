using CRN_Technosoft.Application.Command.Login;
using CRN_Technosoft.Application.Command.Users;
using CRN_Technosoft.Application.Common.Interfaces;
using CRN_Technosoft.Application.DTO;
using CRN_Technosoft.Application.Queries.Users;
using CRN_Technosoft.Domain.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using System.ComponentModel.DataAnnotations;

namespace CRN_Technosoft.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public sealed class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IMediator mediator, IJwtTokenService jwtTokenService)
        {
            _mediator = mediator;
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        [EnableRateLimiting("Authentication")]
        public async Task<ActionResult<ApiResponseModel>> RefreshToken(
            [FromBody] RefreshTokenRequestDto request,
            CancellationToken cancellationToken)
        {
            var tokenResponse = await _jwtTokenService.RefreshTokenAsync(
                request.RefreshToken,
                cancellationToken);

            return Ok(CreateResponse(
                "Tokens refreshed successfully",
                StatusCodes.Status200OK,
                tokenResponse));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [EnableRateLimiting("Authentication")]
        public async Task<ActionResult<ApiResponseModel>> Login(
            [FromBody] LoginRequestDto loginRequestDto,
            CancellationToken cancellationToken)
        {
            var userDetails = await _mediator.Send(new LoginCommand(loginRequestDto), cancellationToken);

            if (userDetails.user.UserId == 0 || string.IsNullOrWhiteSpace(userDetails.AccessToken))
            {
                throw new InvalidOperationException("Authentication completed without a valid token result.");
            }

            var response = CreateResponse("User retrieved successfully", StatusCodes.Status200OK, userDetails);
            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("Authentication")]
        public async Task<ActionResult<ApiResponseModel>> AddUser(
            [FromBody] AddUserDTO addUserDto,
            CancellationToken cancellationToken)
        {
            var userId = await _mediator.Send(new AddUserCommand(addUserDto), cancellationToken);
            if (userId == 0)
            {
                throw new InvalidOperationException("User registration did not produce a user identifier.");
            }

            var response = CreateResponse(
                "User added successfully",
                StatusCodes.Status201Created,
                new { userId });

            return CreatedAtAction(nameof(GetUser), new { userId }, response);
        }

        [HttpPut("{userId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseModel>> EditUser(
            [FromRoute] int userId,
            [FromBody] EditUserDTO editUserDto,
            CancellationToken cancellationToken)
        {
            if (!IsAdministrator() && !IsAuthenticatedUser(userId))
            {
                return Forbid();
            }

            if (editUserDto.Id != userId)
            {
                return BadRequest(CreateResponse(
                    "The route user identifier must match the request body.",
                    StatusCodes.Status400BadRequest,
                    string.Empty));
            }

            var updatedUserId = await _mediator.Send(
                new EditUserCommand(userId, editUserDto),
                cancellationToken);

            if (updatedUserId == 0)
            {
                return NotFound(CreateResponse(
                    "User not found.",
                    StatusCodes.Status404NotFound,
                    string.Empty));
            }

            return Ok(CreateResponse(
                "User edited successfully",
                StatusCodes.Status200OK,
                new { userId = updatedUserId }));
        }

        [HttpGet]
        [Authorize(Roles = "Admin,SuperAdmin")]
        public async Task<ActionResult<ApiResponseModel>> GetUserList(
            [FromQuery, Range(1, int.MaxValue)] int pageIndex = 1,
            [FromQuery, Range(1, 100)] int pageSize = 10,
            CancellationToken cancellationToken = default)
        {
            var users = await _mediator.Send(
                new GetUserListQuery(pageIndex, pageSize),
                cancellationToken);
            return Ok(CreateResponse(
                "Users retrieved successfully",
                StatusCodes.Status200OK,
                users));
        }

        [HttpGet("{userId:int}")]
        [Authorize]
        public async Task<ActionResult<ApiResponseModel>> GetUser(
            [FromRoute] int userId,
            CancellationToken cancellationToken)
        {
            if (!IsAdministrator() && !IsAuthenticatedUser(userId))
            {
                return Forbid();
            }

            var user = await _mediator.Send(
                new GetUserQuery(new GetUserDTO { Id = userId }),
                cancellationToken);

            if (user is null)
            {
                return NotFound(CreateResponse(
                    "User not found.",
                    StatusCodes.Status404NotFound,
                    string.Empty));
            }

            return Ok(CreateResponse(
                "User retrieved successfully",
                StatusCodes.Status200OK,
                user));
        }

        private static ApiResponseModel CreateResponse(string message, int statusCode, object payload)
        {
            return new ApiResponseModel
            {
                message = message,
                statusCode = statusCode,
                payload = payload
            };
        }

        private bool IsAdministrator()
        {
            return User.IsInRole("Admin") || User.IsInRole("SuperAdmin");
        }

        private bool IsAuthenticatedUser(int userId)
        {
            var subject = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

            return int.TryParse(subject, out var authenticatedUserId)
                && authenticatedUserId == userId;
        }
    }

}
