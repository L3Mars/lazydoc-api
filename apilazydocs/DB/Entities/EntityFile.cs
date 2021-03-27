using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Entities
{
    public class EntityFile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid DocumentId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public byte[] File { get; set; }
        public decimal Size { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Encrypted { get; set; }

        public EntityDocument Document { get; set; }
    }
}
