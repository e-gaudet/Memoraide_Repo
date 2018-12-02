using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace Memoraide_WebApp.Models
{
    public class DeckViewModel
    {
        public int ID;
        
        [Required]
        [Display(Name = "Deck Name")]
        [MaxLength(1024)]
        public string Name{ get; set; }
        public int UserId { get; set; }
        public DateTime? DateCreated { get; set; }
        public bool? isDeleted { get; set; }
        public DateTime? DateDeleted { get; set; }

        public bool isSubbed { get; set; }
    }
}
