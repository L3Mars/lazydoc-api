using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Entities
{
    public class EntityTag
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DocumentId { get; set; }
        public string Label { get; set; }
        public DateTime CreateDate { get; set; }

        public EntityDocument Document { get; set; }
    }
}
