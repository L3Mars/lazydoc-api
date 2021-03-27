using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Entities
{
    public class EntityDocument
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreateDate { get; set; }

        public ICollection<EntityFile> Files { get; set; }
        public ICollection<EntityTag> Tags { get; set; }
    }
}
