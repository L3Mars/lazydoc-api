using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiLazyDoc.Helpers
{
    public class AppSettings
    {
        public string Secret { get; set; }
        public string FileSecret { get; set; }
        public string AppUrl { get; set; }
        public FacebookSettings FacebookApp { get; set; }
        public EmailSettings EmailSettings { get; set; }
    }

    public class FacebookSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }

    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public int PrimaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
    }
}
