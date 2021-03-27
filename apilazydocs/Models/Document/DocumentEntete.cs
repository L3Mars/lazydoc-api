using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Models
{
    public class DocumentEntete
    {
        public Guid DocumentId { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public DateTime CreateDate { get; set; }
        public IEnumerable<string> FilesTypes { get; set; }
        public int FilesCount { get; set; }
    }
}
