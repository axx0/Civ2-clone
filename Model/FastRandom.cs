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
    }
}