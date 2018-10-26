using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Memoraide_WebApp.Models
{
    public class CardViewModel
    {
        public int ID;

        [Required]
        [Display(Name = "Card Front")]
        [MaxLength(1024)]
        public string CardFront { get; set; }

        [Required]
        [Display(Name = "Card Back")]
        [MaxLength(1024)]
        public string CardBack { get; set; }

        [Display(Name = "Tags")]
        public string CardTags {
            get {
                if (_cardTags != null)
                    return String.Join(", ", _cardTags);
                else
                    return "";
            }

            set {
                if (value != null)
                {
                    _cardTags = new List<string>(value.Split(","));
                    _cardTags = _cardTags.Select(i => i.Trim()).ToList();
                }
            }
        }

        public int DeckId { get; set; }
        private List<string> _cardTags;
    }
}
