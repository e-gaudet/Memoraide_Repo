using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Memoraide_API.Models
{
    public class Card
    {
        public int Id { get; set; }
        private int deckId { get; set; }
        private string question { get; set; }
        private string answer { get; set; }
    }
}
