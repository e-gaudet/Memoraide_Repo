using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Memoraide_API.Models
{
    public class Deck
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public Decimal Rating { get; set; }
    }
}
