using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Models
{
    public class Document
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime CreateDate { get; set; }
        public IEnumerable<FichierEntete> Files { get; set; }
        public IEnumerable<Tag> Tags { get; set; }
    }
}
