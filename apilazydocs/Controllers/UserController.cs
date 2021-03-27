using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Helpers;
using ApiLazyDoc.Models;
using ApiLazyDoc.Models.User;
using ApiLazyDoc.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ApiLazyDoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly EmailService _email;

        public UserController(IUserService userService, EmailService email)
        {
            this._userService = userService;
            this._email = email;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegistrationRequest model)
        {
            try
            {
                if (model == null) return BadRequest("Bad register parameters");

                var success = await _userService.Add(model);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(AuthenticateRequest model)
        {
            var response = await _userService.Authenticate(model);

            if (response == null)
                return BadRequest(new { message = "Incorrect username or password." });

            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("ConfirmAccount/{accountId}")]
        public async Task<IActionResult> ConfirmAccount(Guid accountId)
        {
            await _userService.ConfirmAccount(accountId);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("RequestValidationMail")]
        public async Task<IActionResult> RequestValidationMail(ValidationMailRequest request)
        {
            await _userService.SendValidationMail(request.Email);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("facebookLogin")]
        public async Task<IActionResult> FacebookLogin([FromBody] FacebookRequest facebookToken)
        {
            var response = await this._userService.LoginWithFacebook(facebookToken);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("ChangeUsername")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest request)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            string newusername = await this._userService.ChangeUsername(userId, request.Username);
            return Ok(newusername);
        }
    }
}