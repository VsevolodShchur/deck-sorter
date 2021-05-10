using System.Collections.Generic;

namespace DeckSorter.Models
{
    public interface IDeckStorage
    {
        public Deck Get(string name);
        public bool InsertOrUpdate(Deck deck);
        public bool Delete(string name);
        public IEnumerable<Deck> GetAll();
    }
}
