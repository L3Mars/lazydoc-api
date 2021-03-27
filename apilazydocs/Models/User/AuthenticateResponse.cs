using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiLazyDoc.DB.Entities;

namespace ApiLazyDoc.Models.User
{
    public class AuthenticateResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }


        public AuthenticateResponse(EntityUser user, string token)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            Token = token;
        }
    }
}
