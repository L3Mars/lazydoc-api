using ApiLazyDoc.Entities;
using ApiLazyDoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Mappers
{
    public static class FichierMapper
    {
        public static Fichier ToFichier(this EntityFile entity)
        {
            return new Fichier()
            {
                Id = entity.Id,
                Type = entity.Type,
                File = entity.File,
                Name = entity.Name,
                CreateDate = entity.CreateDate
            };
        }

        public static FichierEntete ToFichierEntete(this EntityFile entity)
        {
            return new FichierEntete()
            {
                Id = entity.Id,
                Type = entity.Type,
                Name = entity.Name,
                CreateDate = entity.CreateDate
            };
        }
    }
}
