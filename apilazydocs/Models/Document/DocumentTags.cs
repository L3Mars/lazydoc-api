using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Models
{
    public class DocumentTags
    {
        public Guid DocumentId { get; set; }
        public IEnumerable<string> Tags { get; set; }
    }
}
