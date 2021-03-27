using ApiLazyDoc.Entities;
using ApiLazyDoc.Mappers;
using ApiLazyDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Services
{
    public class TagService
    {
        private readonly LazyDocContext _context;
        public TagService(LazyDocContext context)
        {
            this._context = context;
        }

        public IEnumerable<string> GetUserTags(Guid userId)
        {
            return this._context.Tags.Where(t => t.UserId == userId).Select(t => t.Label).Distinct();
        }

        internal void DeleteTag(Guid documentId, int tagId, Guid userId)
        {
            if(this._context.Tags.Any(t => t.DocumentId == documentId && t.UserId == userId && t.Id == tagId))
            {
                this._context.Tags.Remove(this._context.Tags.Single(t => t.DocumentId == documentId && t.UserId == userId && t.Id == tagId));
                this._context.SaveChanges();
            }
        }

        internal Tag Add(Guid documentId, TagRequest tagRequest, Guid userId)
        {
            EntityTag entity = new EntityTag()
            {
                UserId = userId,
                DocumentId = documentId,
                Label = tagRequest.TagLabel,
                CreateDate = DateTime.Now
            };
            this._context.Tags.Add(entity);
            this._context.SaveChanges();
            return entity.Map();
        }
    }
}
