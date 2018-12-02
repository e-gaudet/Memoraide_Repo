using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memoraide_WebApp.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime? DateCreated { get; set; }
        public Boolean? IsBanned { get; set; }
        public DateTime? DateBanned { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
    }
}
