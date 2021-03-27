using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Helpers;
using ApiLazyDoc.Models;
using ApiLazyDoc.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiLazyDoc.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<IEnumerable<EntityUser>> GetAll();
        EntityUser GetById(Guid id);
        Task<bool> Add(RegistrationRequest registration);
        Task Delete(Guid id);
        Task<bool> ConfirmAccount(Guid accountId);
        Task SendValidationMail(string email);
        Task<AuthenticateResponse> LoginWithFacebook(FacebookRequest facebookToken);
        Task<string> ChangeUsername(Guid userId, string newUsername);
    }

    public class UserService : IUserService
    {
        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private readonly AppSettings _appSettings;
        private readonly LazyDocContext _context;
        private readonly EmailService _emailService;
        public UserService(IOptions<AppSettings> appSettings, LazyDocContext context, EmailService emailService)
        {
            this._appSettings = appSettings.Value;
            this._context = context;
            this._emailService = emailService;
        }

        public async Task<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(x => x.Email == model.Email);
            if (user == null || !PasswordHelper.VerifyPasswordHash(model.Password, user.Password)) return null;
            else if (user != null && !user.Activated) throw new AppException("Account not activated", 412);
            var token = generateJwtToken(user);

            return new AuthenticateResponse(user, token);
        }

        public async Task<bool> Add(RegistrationRequest registration)
        {
            if (await this._context.Users.AnyAsync(u => u.Email == registration.Email))
                throw new AppException("Un compte est déjà lié à cette adresse email");

            EntityUser user = new EntityUser()
            {
                Username = registration.Username,
                Email = registration.Email,
                Password = PasswordHelper.GetPasswordHash(registration.Password),
                RegistrationDate = DateTime.Now
            };

            await this._context.Users.AddAsync(user);
            await this._context.SaveChangesAsync();

            await this._emailService.SendConfirmationMail(registration.Email, registration.Username, user.Id);
            return true;
        }
        public async Task Delete(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<EntityUser>> GetAll()
        {
            return this._context.Users;
        }

        public EntityUser GetById(Guid id)
        {
            var user = this._context.Users.FirstOrDefault(x => x.Id == id);
            return user;
        }

        private string generateJwtToken(EntityUser user)
        {
            // generate token that is valid for 30 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.UtcNow.AddDays(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<bool> ConfirmAccount(Guid accountId)
        {
            var user = await this._context.Users.FirstOrDefaultAsync(u => u.Id == accountId);
            if (user != null)
            {
                user.Activated = true;
                await this._context.SaveChangesAsync();
                return true;
            }
            else return false;
        }

        public async Task SendValidationMail(string email)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(x => x.Email == email);
            if (user != null && !user.Activated)
                await this._emailService.SendConfirmationMail(user.Email, user.Username, user.Id);
            else throw new AppException("No validation mail to send.");
        }

        public async Task<AuthenticateResponse> LoginWithFacebook(FacebookRequest facebookToken)
        {
            HttpClient client = new HttpClient();
            var appId = this._appSettings.FacebookApp.AppId;
            var appSecret = this._appSettings.FacebookApp.AppSecret;
            // 1.generate an app access token
            var appAccessTokenResponse = await client.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id={appId}&client_secret={appSecret}&grant_type=client_credentials");
            var appAccessToken = JsonConvert.DeserializeObject<FacebookAppAccessToken>(appAccessTokenResponse);
            // 2. validate the user access token
            var userAccessTokenValidationResponse = await client.GetStringAsync($"https://graph.facebook.com/debug_token?input_token={facebookToken.AccessToken}&access_token={appAccessToken.AccessToken}");
            var userAccessTokenValidation = JsonConvert.DeserializeObject<FacebookUserAccessTokenValidation>(userAccessTokenValidationResponse);

            if (!userAccessTokenValidation.Data.IsValid)
            {
                throw new AppException("Invalid facebook token.");
            }

            // 3. we've got a valid token so we can request user data from fb
            var userInfoResponse = await client.GetStringAsync($"https://graph.facebook.com/v2.8/me?fields=id,email,first_name,last_name&access_token={facebookToken.AccessToken}");
            var userInfo = JsonConvert.DeserializeObject<FacebookUserData>(userInfoResponse);

            // 4. ready to create the local user account (if necessary) and jwt
            return await this.CheckFacebookUserIdDB(userInfo);
        }

        private async Task<AuthenticateResponse> CheckFacebookUserIdDB(FacebookUserData facebook)
        {
            var user = await this._context.Users.SingleOrDefaultAsync(x => x.Email == facebook.Email);
            if (user == null)
            {
                user = new EntityUser()
                {
                    FacebookId = facebook.Id.ToString(),
                    Username = $"{facebook.FirstName} {facebook.LastName}",
                    Email = facebook.Email,
                    Activated = true,
                    RegistrationDate = DateTime.Now
                };
                await this._context.Users.AddAsync(user);
                await this._context.SaveChangesAsync();
            }
            return new AuthenticateResponse(user, this.generateJwtToken(user));
        }

        public async Task<string> ChangeUsername(Guid userId, string newUsername)
        {
            var user = this._context.Users.FirstOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Username = newUsername;
                await this._context.SaveChangesAsync();
                return newUsername;
            }
            throw new AppException($"L'utilisateur {newUsername} est introuvable");
        }
    }
}
