using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Models
{
    public class FichierEntete
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }
        public string JwtAccess { get; set; }
    }
}
