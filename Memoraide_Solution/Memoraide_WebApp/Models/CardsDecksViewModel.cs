﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Memoraide_WebApp.Models
{
    //container for pages requiring access of all data from cards and decks.
    public class CardsDecksViewModel
    {
        public CardViewModel cardViewModel { get; set; }
        public List<CardViewModel> cardsViewModel { get; set; }
        public List<DeckViewModel> deckViewModel { get; set; }
    }
}