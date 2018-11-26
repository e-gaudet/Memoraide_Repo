using System;
using System.ComponentModel.DataAnnotations;

namespace Memoraide_API.Models
{
    public class CardAnswer
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CardId { get; set; }
        public bool CorrectAnswer { get; set; }
        public DateTime NextReviewDate { get; set; }
        public DateTime? DateAnswered { get; set; }
    }
}
