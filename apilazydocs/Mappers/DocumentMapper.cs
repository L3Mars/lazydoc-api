using ApiLazyDoc.Entities;
using ApiLazyDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Mappers
{
    public static class DocumentMapper
    {
        public static Document Map(this EntityDocument entity)
        {
            return new Document()
            {
                CreateDate = entity.CreateDate,
                Id = entity.Id,
                UserId = entity.UserId,
                Files = entity.Files.Select(f => f.ToFichierEntete()).ToList(),
                Tags = entity.Tags.Select(t => t.Map())
            };
        }
    }
}
