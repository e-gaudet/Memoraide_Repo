using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Memoraide_WebApp.Models
{
    public class CardViewModel
    {
        public int ID;
        //public int ID { get; set; }

        [Required]
        [Display(Name = "Card Front")]
        [MaxLength(1024)]
        public string Question { get; set; }

        [Required]
        [Display(Name = "Card Back")]
        [MaxLength(1024)]
        public string Answer { get; set; }

        //[Display(Name = "Tags")]
        //public string CardTags {
        //    get {
        //        if (_cardTags != null)
        //            return String.Join(", ", _cardTags);
        //        else
        //            return "";
        //    }

        //    set {
        //        if (value != null)
        //        {
        //            _cardTags = new List<string>(value.Split(","));
        //            _cardTags = _cardTags.Select(i => i.Trim()).ToList();
        //        }
        //    }
        //}

        [Display(Name = "Deck Id")]
        public int DeckId { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }

       // public List<DeckViewModel> decks { get; set; }
       // private List<string> _cardTags;
    }
}
