using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace masz.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class AuthenticationController : ControllerBase
    {
        private readonly ILogger<AuthenticationController> _logger;

        public AuthenticationController(ILogger<AuthenticationController> logger)
        {
            _logger = logger;
        }

        [HttpGet("login")]
        public IActionResult Login([FromQuery] string ReturnUrl)
        {
            if (String.IsNullOrEmpty(ReturnUrl)) {
                ReturnUrl = "/guilds";
            }

            var properties = new AuthenticationProperties()
            {
                RedirectUri = ReturnUrl,
                Items =
                {
                    { "LoginProvider", "Discord" },
                    { "scheme", "Discord" }
                },
                AllowRefresh = true,
            };
            return this.Challenge(properties, "Discord");
        }

        [HttpGet("logout")]
        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            var properties = new AuthenticationProperties()
            {
                RedirectUri = "/",
                Items =
                {
                    { "LoginProvider", "Discord" },
                    { "scheme", "Discord" }
                },
                AllowRefresh = true,
            };
            return this.SignOut(properties, CookieAuthenticationDefaults.AuthenticationScheme);
        }

    }
}