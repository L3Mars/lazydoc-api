using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Models
{
    public class Fichier
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] File { get; set; }
        public string Type { get; set; }
        public DateTime CreateDate { get; set; }

        public string GetContentType()
        {
            switch (this.Type)
            {
                case ".txt": return "text/plain";
                case ".doc": return "application/vnd.ms-word";
                case ".docx": return "application/vnd.ms-word";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformatsofficedocument.spreadsheetml.sheet";
                case ".csv": return "text/csv";
                case ".pdf": return "application/pdf";
                case ".jpg": return "image/jpeg";
                default: return $"image/{this.Type?.TrimStart('.')}";
            }
        }
    }
}
