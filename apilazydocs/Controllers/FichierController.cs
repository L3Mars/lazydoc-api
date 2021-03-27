using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Helpers;
using ApiLazyDoc.Models;
using ApiLazyDoc.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiLazyDoc.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FichierController : ControllerBase
    {
        private readonly ILogger<FichierController> _logger;
        private readonly FichierService _fichierService;
        private readonly AppSettings _appSettings;

        public FichierController(ILogger<FichierController> logger, FichierService fichierService, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            this._fichierService = fichierService;
            this._appSettings = appSettings.Value;
        }

        [HttpGet("{jwtAccess}")]
        public ActionResult Get(string jwtAccess)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.FileSecret);
            tokenHandler.ValidateToken(jwtAccess, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var fileId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "fileId").Value);
            var userId = Guid.Parse(jwtToken.Claims.First(x => x.Type == "userId").Value);

            var fichier = this._fichierService.Get(fileId, userId);
            if (fichier?.File == null) return NotFound();
            Stream stream = new MemoryStream(fichier.File);
            return File(stream, fichier.GetContentType());
        }

        [Authorize]
        [HttpDelete("{fileId}")]
        public ActionResult Delete(Guid fileId)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            this._fichierService.Delete(fileId, userId);
            return Ok();
        }

        [Authorize]
        [HttpPost("{documentId}")]
        public ActionResult<FichierEntete> Post(IFormCollection files, Guid documentId)
        {
            var userId = (this.HttpContext.Items["User"] as EntityUser).Id;
            var fichierEntete = this._fichierService.Add(files.Files.FirstOrDefault(), documentId, userId);
            return Ok(fichierEntete);
        }
    }
}
