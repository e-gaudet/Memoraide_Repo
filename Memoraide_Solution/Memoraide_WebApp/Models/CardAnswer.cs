using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memoraide_WebApp.Models
{
    public class CardAnswer
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int CardId { get; set; }
        public bool CorrectAnswer { get; set; }
        public DateTime NextReviewDate { get; set; }
        public DateTime? DateAnswered { get; set; }
    }
}
