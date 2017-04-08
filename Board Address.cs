using System;
using System.Collections.Generic;

namespace LittleOwl {
    public class BoardAddress {
        public char File { get { return (char)('a' + (int)(Position / 8)); } }
        public int Rank { get; }

        private static ulong[] PositionLookup;
        private static Dictionary<ulong, int> IndexLookup;
        static BoardAddress() {
            PositionLookup = new ulong[64];
            for (uint i = 0; i < 64; i++)
                PositionLookup[i] = Utilities.Pow(2, i);

            IndexLookup = new Dictionary<ulong, int>();
            ulong Bitboard = 1;
            for (int i = 0; i < 64; i++) {
                IndexLookup.Add(Bitboard, i);
                Bitboard <<= 1;
            }
        }

        public int Index { get { return IndexLookup[Position]; } set { Position = PositionLookup[value]; } }

        private ulong _Position;
        public ulong Position {
            get { return _Position; }
            set {
                if (Utilities.NumActiveBits(value) != 1) throw new ArgumentException(string.Format("num of active bits in \"{0}\" is not equal to one", value));
                _Position = value;
            }
        }

        public BoardAddress(ulong pos) { Position = pos; }

        public BoardAddress(int i) { Index = i; } // todo is this bad form? (two constructors with single numeric args)

        public BoardAddress(char f, int r) {
            f = char.ToLower(f);
            if (f < 'a' || f > 'h') throw new ArgumentException(string.Format("invalid file \"{0}\"", f));
            if (r < 1 || r > 8) throw new ArgumentException(string.Format("invalid rank \"{0}\"", r));

            ulong FileIndex = (ulong)(f - 'a');
            ulong RankIndex = (ulong)r;

            Position = FileIndex + (RankIndex * 8);
        }

        public override string ToString() { return string.Format("{0}{1}", File, Rank); }
    }
}
