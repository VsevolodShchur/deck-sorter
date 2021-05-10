using System.Collections.Generic;

namespace DeckSorter.Models
{
    public class InMemoryDeckStorage : IDeckStorage
    {
        private readonly Dictionary<string, Deck> decks;

        public InMemoryDeckStorage()
        {
            decks = new Dictionary<string, Deck>();
        }

        public bool InsertOrUpdate(Deck deck)
        {
            decks.Add(deck.Name, deck);
            return true;
        }

        public bool Delete(string name)
        {
            return decks.Remove(name);
        }

        public Deck Get(string name)
        {
            var success = decks.TryGetValue(name, out var deck);
            return success ? deck : null;
        }

        public IEnumerable<Deck> GetAll()
        {
            return decks.Values;
        }
    }
}