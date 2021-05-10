using System;
using System.Collections.Generic;
using System.Linq;


namespace DeckSorter.Models
{
    public class Deck
    {
        public string Name { get; }
        public List<Card> Cards { get; }

        public Deck(string name, IEnumerable<Card> cards)
        {
            Name = name;
            Cards = cards.ToList();
        }

        public static Deck Create52Deck(string name)
        {
            var suits = Enum.GetValues(typeof(Suit)).Cast<Suit>();
            var ranks = Enum.GetValues(typeof(Rank)).Cast<Rank>();
            var cards = suits.SelectMany(suit => ranks.Select(rank => new Card(suit, rank)));
            return new Deck(name, cards);
        }
    }

    public class Card
    {
        public Suit Suit { get; }
        public Rank Rank { get; }

        public Card(Suit suit, Rank rank)
        {
            Suit = suit;
            Rank = rank;
        }
    }

    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    public enum Rank
    {
        Ace = 1,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King
    }
}