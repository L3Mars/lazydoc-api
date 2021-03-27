using ApiLazyDoc.DB.Entities;
using ApiLazyDoc.Entities;
using ApiLazyDoc.Helpers;
using ApiLazyDoc.Mappers;
using ApiLazyDoc.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiLazyDoc.Services
{
    public class FichierService
    {
        private readonly LazyDocContext _context;
        private readonly IDataProtectionProvider _protectorProvider;
        public FichierService(LazyDocContext context, IDataProtectionProvider dataProtectionProvider)
        {
            this._context = context;
            this._protectorProvider = dataProtectionProvider;
        }

        public Fichier Get(Guid fichierId, Guid userId)
        {
            var query = this._context.Files.Where(f => f.Id == fichierId && f.UserId == userId).FirstOrDefault();
            var fichier = query?.ToFichier();
            if (fichier != null && query.Encrypted)
            {
                var protector = _protectorProvider.CreateProtector("LazyDocs.Fichier", userId.ToString());
                fichier.File = protector.Unprotect(fichier.File);
            }

            return fichier;
        }

        internal void Delete(Guid fileId, Guid userId)
        {
            if (this._context.Files.Any(d => d.UserId == userId && d.Id == fileId))
            {
                var query = this._context.Files.Remove(this._context.Files.Single(d => d.UserId == userId && d.Id == fileId));
                this._context.SaveChanges();
            }
        }

        public FichierEntete Add(IFormFile file, Guid documentId, Guid userId)
        {
            if (file.Length > 0)
            {
                var fileName = Path.GetFileName(file.FileName);
                var fileExtension = Path.GetExtension(fileName);
                var newFileName = String.Concat(fileName, fileExtension);
                var protector = _protectorProvider.CreateProtector("LazyDocs.Fichier", userId.ToString());

                var fichier = new EntityFile()
                {
                    DocumentId = documentId,
                    Name = newFileName,
                    Type = fileExtension,
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    Size = file.Length,
                    Encrypted = true
                };

                using (var target = new MemoryStream())
                {
                    file.CopyTo(target);
                    fichier.File = protector.Protect(target.ToArray());
                }

                this._context.Files.Add(fichier);
                this._context.SaveChanges();
                return fichier.ToFichierEntete();
            } return null;
        }
    }
}
