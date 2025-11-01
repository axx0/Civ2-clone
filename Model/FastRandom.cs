using System;
using System.Collections.Generic;
using System.Linq;

namespace Civ2engine
{
    /// <summary>
    /// An implementation of a Lahmer random number generator 
    /// </summary>
    public class FastRandom
    {
        public const int MaxSeed = 2147483647;
        
        /// <summary>
        /// This is a long to avoid overflows when multiplying 
        /// </summary>
        /// 
        private const long MultiplicationConstant = 48271L;
        
        private int _seed;

        public FastRandom() : this(new Random().Next())
        {
        }

        public FastRandom(int seed)
        {
            _seed = seed % MaxSeed;
            if (_seed <= 0)
            {
                _seed += MaxSeed - 1;
            }
        }
        
        public int Next()
        {
            return _seed = (int) ((_seed * MultiplicationConstant) % MaxSeed);
        }

        public double NextFloat()
        {
            return (Next() - 1.0) / (MaxSeed - 1);
        }

        public bool NextBool()
        {
            return Next() % 2 == 0;
        }

        public int State()
        {
            return (int)_seed;
        }

        /// <summary>
        /// Produce a number from 0 to max -1
        /// </summary>
        /// <param name="max">The number of values to chose from max is exclusive</param>
        /// <returns></returns>
        public int Next(int max)
        {
            return Next() % max;
        }

        public int Next(int min, int max)
        {
            return min + Next() % (max - min);
        }

        public T ChooseFrom<T>(IList<T> items)
        {
            return items.Count == 1 ? items[0] : items[Next(items.Count)];
        }

        public T ChooseFrom<T>(ISet<T> items)
        {
            return items.Count == 1 ? items.First() : items.ElementAt(Next(items.Count));
        }
        
        /// <summary>
        /// Shuffles a list in-place using the Fisher-Yates shuffle algorithm
        /// </summary>
        /// <typeparam name="T">The type of elements in the list</typeparam>
        /// <param name="items">The list to shuffle</param>
        /// <returns>The same list, shuffled in-place</returns>
        public IList<T> Shuffle<T>(IList<T> items)
        {
            if (items.Count <= 1) return items;
            
            for (var i = items.Count - 1; i > 0; i--)
            {
                var j = Next(i + 1);
                (items[i], items[j]) = (items[j], items[i]);
            }
            
            return items;
        }

        /// <summary>
        /// Shuffles an enumerable collection using the Fisher-Yates shuffle algorithm.
        /// If the input is an IList, it shuffles in-place; otherwise, it creates a new list.
        /// </summary>
        /// <typeparam name="T">The type of elements in the collection</typeparam>
        /// <param name="items">The enumerable to shuffle</param>
        /// <returns>The shuffled collection (same instance if IList, new list otherwise)</returns>
        public IEnumerable<T> Shuffle<T>(IEnumerable<T> items)
        {
            // If it's already an IList, shuffle in-place using the more specific overload
            if (items is IList<T> list)
            {
                return Shuffle(list);
            }
            
            // Otherwise, convert to list and shuffle
            var newList = items.ToList();
            
            if (newList.Count <= 1) return newList;
            
            for (var i = newList.Count - 1; i > 0; i--)
            {
                var j = Next(i + 1);
                (newList[i], newList[j]) = (newList[j], newList[i]);
            }
            
            return newList;
        }
    }
}