using System;
using System.Collections.Generic;
using System.Linq;

namespace DeckSorter.Models
{
    public class SimpleShuffler : IShuffler
    {
        public IEnumerable<T> Shuffle<T>(IEnumerable<T> enumerable)
        {
            var rnd = new Random();
            var shuffled = enumerable.OrderBy(x => rnd.Next());
            return shuffled;
        }
    }
}
