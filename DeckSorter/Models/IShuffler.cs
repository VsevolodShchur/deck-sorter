using System.Collections.Generic;

namespace DeckSorter.Models
{
    public interface IShuffler
    {
        public IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable);
    }
}
