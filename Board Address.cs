using System;

namespace LittleOwl {
    public class BoardAddress {
        public char File { get; }
        public int Rank { get; }

        private ulong _Position;
        public ulong Position {
            get { return _Position; }
            set {
                if (Utilities.NumActiveBits(value) != 1) throw new ArgumentException(string.Format("num of active bits in \"{0}\" is not equal to one", value));
                _Position = value;
            }
        }

        public BoardAddress(ulong pos) { Position = pos; }

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
