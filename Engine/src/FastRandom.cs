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
        public const int maxSeed = 2147483647;
        
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
            _seed = seed % maxSeed;
            if (_seed <= 0)
            {
                _seed += maxSeed - 1;
            }
        }
        
        public int Next()
        {
            return _seed = (int) ((_seed * MultiplicationConstant) % maxSeed);
        }

        public double nextFloat()
        {
            return (Next() - 1.0) / (maxSeed - 1);
        }

        public int state()
        {
            return (int)_seed;
        }

        public int Next(int max)
        {
            return Next() % (max -1);
        }

        public int Next(int min, int max)
        {
            return min + Next() % (max - min + 1);
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