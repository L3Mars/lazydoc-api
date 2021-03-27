using ApiLazyDoc.Entities;
using ApiLazyDoc.Helpers;
using ApiLazyDoc.Mappers;
using ApiLazyDoc.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class DocumentService
    {
        private readonly LazyDocContext _context;
        private readonly AppSettings _appSettings;

        public DocumentService(IOptions<AppSettings> appSettings, LazyDocContext context)
        {
            this._context = context;
            this._appSettings = appSettings.Value;
        }

        public Document Get(Guid documentId, Guid userId)
        {
            var query = this._context.Documents.Where(d => d.Id == documentId && d.UserId == userId).Include(d => d.Files).Include(d => d.Tags).FirstOrDefault();
            var document = query.Map();
            foreach(var file in document.Files)
                file.JwtAccess = this.generateJwtToken(file.Id, document.UserId);

            return document;
        }

        public Guid AddDocument(IFormCollection fileCollection, Guid userId)
        {
            if (fileCollection == null) return Guid.Empty;
            var document = new EntityDocument();
            document.UserId = userId;
            document.Files = new List<EntityFile>();
            document.CreateDate = DateTime.Now;

            foreach (var file in fileCollection.Files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var fileExtension = Path.GetExtension(fileName);
                    var newFileName = String.Concat(fileName, fileExtension);

                    var fichier = new EntityFile()
                    {
                        Name = newFileName,
                        Type = fileExtension,
                        UserId = document.UserId,
                        CreateDate = DateTime.Now
                    };

                    using (var target = new MemoryStream())
                    {
                        file.CopyTo(target);
                        fichier.File = target.ToArray();
                    }
                    document.Files.Add(fichier);
                }
            }

            this._context.Documents.Add(document);
            this._context.SaveChanges();
            return document.Id;
        }

        public void TagDocument(DocumentTags documentTags, Guid userId)
        {
            foreach (string tag in documentTags.Tags)
            {
                EntityTag entityTag = new EntityTag
                {
                    Label = tag,
                    UserId = userId,
                    CreateDate = DateTime.Now,
                    DocumentId = documentTags.DocumentId
                };

                this._context.Tags.Add(entityTag);
            }
            this._context.SaveChanges();
        }

        public void Delete(Guid documentId, Guid userId)
        {
            if (this._context.Documents.Any(d => d.Id == documentId && d.UserId == userId))
            {
                this._context.Documents.Remove(this._context.Documents.Single(d => d.Id == documentId && d.UserId == userId));
                this._context.SaveChanges();
            }
            else throw new AppException("No document to delete.");
        }

        public List<DocumentEntete> GetDocumentsEntete(List<string> tags, Guid userId)
        {
            var queryDocuments = this._context.Documents.Include(t => t.Tags).Include(f => f.Files).Where(d => d.UserId == userId);
            queryDocuments = queryDocuments.Where(d => d.Tags.Where(t => tags.Contains(t.Label)).Count() == tags.Count()).OrderByDescending(d => d.CreateDate);
            var documentsEntete = queryDocuments.Select(d => new DocumentEntete()
            {
                DocumentId = d.Id,
                Tags = d.Tags.Select(t => t.Label),
                CreateDate = d.CreateDate,
                FilesCount = d.Files.Count(),
                FilesTypes = d.Files.Select(f => f.Type)
            }).Take(20).ToList();

            return documentsEntete;
        }
        private string generateJwtToken(Guid fileId, Guid userId)
        {
            // generate token that is valid for 30 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.FileSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("fileId", fileId.ToString()), new Claim("userId", userId.ToString()) }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
