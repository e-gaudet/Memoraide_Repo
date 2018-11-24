using System;
using System.ComponentModel.DataAnnotations;

namespace Memoraide_API.Models
{
    public class Card
    {
        [Key]
        public int Id { get; set; }
        public int DeckId { get; set; }
        public string Question { get; set; }
        public string Answer { get; set; }
        public DateTime? DateCreated { get; set; }
        public Boolean? IsDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }
        public Decimal Rating { get; set; }
    }
}
