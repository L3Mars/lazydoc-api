using ApiLazyDoc.Entities;
using ApiLazyDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Mappers
{
    public static class TagMapper
    {
        public static Tag Map(this EntityTag entity)
        {
            return new Tag()
            {
                Id = entity.Id,
                UserId = entity.UserId,
                DocumentId = entity.DocumentId,
                Label = entity.Label,
                CreateDate = entity.CreateDate
            };
        }
    }
}
