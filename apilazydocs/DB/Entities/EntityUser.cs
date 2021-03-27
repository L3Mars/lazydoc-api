using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ApiLazyDoc.DB.Entities
{
    public class EntityUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string FacebookId { get; set; }
        public bool Activated { get; set; }

        [JsonIgnore]
        public string Password { get; set; }
    }
}
