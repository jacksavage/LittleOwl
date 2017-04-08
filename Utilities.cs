using System;
using System.Collections.Generic;

namespace LittleOwl {
    // helper methods not specific to chess engine functionality
    internal static class Utilities {
        private static Random Random = new Random(); // todo Utilities.Random thread safe

        // get single bit ulongs that make up a multibit ulong
        public static IEnumerable<ulong> BitSplit(ulong source) {
            while (source != 0) {
                yield return source & (~source + 1);
                source &= source - 1;
            }
        }

        // get single bit ulongs that make up a multibit ulong in a random order
        public static IEnumerable<ulong> RandomBitSplit(ulong source) {
            var Buffer = new List<ulong>(BitSplit(source));
            foreach (ulong result in RandomComprehensiveAccess<ulong>.Access(Buffer))
                yield return result;
        }

        // count of the number of active bits in a ulong
        public static int NumActiveBits(ulong source) {
            int Result = 0;
            foreach (ulong position in BitSplit(source))
                Result++;
            return Result;
        }

        // generic class wrapper for random and comprehensive array access
        public static class RandomComprehensiveAccess<T> {
            public static IEnumerable<T> Access(T[] source) {
                var Buffer = new List<T>(source);
                foreach (T result in Access(Buffer))
                    yield return result;
            }

            public static IEnumerable<T> Access(List<T> source) {
                // get all of the singles
                var Buffer = new List<T>(source);
                int NumRemaining = Buffer.Count;

                // is there anything to return?
                if (NumRemaining != 0) { // yes
                    int RandomIndex;

                    // while unaccessed bits remain
                    do {
                        // get a random index
                        RandomIndex = Random.Next(0, --NumRemaining);

                        // yield single bit ulong at this index and remove it from the buffer
                        yield return Buffer[RandomIndex];
                        Buffer.RemoveAt(RandomIndex);
                    } while (NumRemaining != 0);
                }
            }
        }
    }
}
