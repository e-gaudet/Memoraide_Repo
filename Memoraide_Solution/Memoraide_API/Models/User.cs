using System;
using System.ComponentModel.DataAnnotations;

namespace Memoraide_API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public DateTime? DateCreated { get; set; }
        public Boolean? IsBanned { get; set; }
        public DateTime? DateBanned { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
