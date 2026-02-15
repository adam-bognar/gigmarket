using GigMarket.Application.Features.Auth.Commands.Login;
using GigMarket.Application.Features.Auth.Commands.Logout;
using GigMarket.Application.Features.Auth.Commands.Register;
using GigMarket.Application.Features.Auth.Models;
using GigMarket.Application.Features.Auth.Queries.Me;
using GigMarket.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace GigMarket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthUserDto>> Register([FromBody] RegisterRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new RegisterCommand(request.Email, request.Password), ct);
            return Ok(result);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthUserDto>> Login([FromBody] LoginRequest request, CancellationToken ct)
        {
            var result = await mediator.Send(new LoginCommand(request.Email, request.Password, request.RememberMe), ct);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Logout(CancellationToken ct)
        {
            await mediator.Send(new LogoutCommand(), ct);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(AuthUserDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AuthUserDto>> Me(CancellationToken ct)
        {
            var result = await mediator.Send(new MeQuery(), ct);
            return Ok(result);
        }
    }


    public sealed record RegisterRequest(string Email, string Password);
    public sealed record LoginRequest(string Email, string Password, bool RememberMe);
}
